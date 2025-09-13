using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MetaBrainz.Common;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Json;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz;

public sealed partial class ListenBrainz {

  #region /1/latest-import

  private static Dictionary<string, string> OptionsForLatestImport(string user) => new() { ["user_name"] = user };

  /// <summary>Get the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is requested.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>An object providing the user's ID and latest import timestamp.</returns>
  /// <remarks>This will access the <c>GET /1/latest-import</c> endpoint.</remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<ILatestImport> GetLatestImportAsync(string user, CancellationToken cancellationToken = default)
    => this.GetAsync<ILatestImport, LatestImport>("latest-import", ListenBrainz.OptionsForLatestImport(user), cancellationToken);

  /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data should be modified.</param>
  /// <param name="timestamp">The timestamp to set.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/latest-import</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SetLatestImportAsync(string user, DateTimeOffset timestamp, CancellationToken cancellationToken = default)
    => this.SetLatestImportAsync(user, timestamp.ToUnixTimeSeconds(), cancellationToken);

  /// <summary>Set the timestamp of the newest listen submitted by a user in previous imports to ListenBrainz.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="timestamp">
  /// The timestamp to set, expressed as the number of seconds since <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/latest-import</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SetLatestImportAsync(string user, long timestamp, CancellationToken cancellationToken = default)
    => this.PostAsync("latest-import", $"{{ ts: {timestamp} }}", ListenBrainz.OptionsForLatestImport(user), cancellationToken);

  #endregion

  #region /1/stats

  private static IDictionary<string, string> OptionsForGetArtistMap(StatisticsRange? range) {
    var options = new Dictionary<string, string>(1);
    if (range is not null) {
      options.Add("range", range.Value.ToJson());
    }
    return options;
  }

  private static IDictionary<string, string> OptionsForGetStatistics(int? count, int? offset, StatisticsRange? range) {
    var options = new Dictionary<string, string>(3);
    if (count is not null) {
      options.Add("count", count.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (offset is not null) {
      options.Add("offset", offset.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (range is not null) {
      options.Add("range", range.Value.ToJson());
    }
    return options;
  }

  #region artist-activity

  /// <summary>Gets listening statistics for the top artists (and their albums) across all of ListenBrainz.</summary>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistActivity?> GetArtistActivityAsync(StatisticsRange? range = null,
                                                       CancellationToken cancellationToken = default) {
    var address = $"stats/sitewide/artist-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IArtistActivity, ArtistActivity>(address, options, cancellationToken);
  }

  /// <summary>Gets listening statistics for a user's top artists (and their albums).</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistActivity?> GetArtistActivityAsync(string user, StatisticsRange? range = null,
                                                       CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/artist-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IArtistActivity, ArtistActivity>(address, options, cancellationToken);
  }

  #endregion

  #region artist-evolution-activity

  /// <summary>Gets listening statistics for artists over time across all of ListenBrainz.</summary>
  /// <param name="range">
  /// The range of data to include in the statistics. This impacts the time units used by the returned information.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistEvolutionActivity?> GetArtistEvolutionActivityAsync(StatisticsRange? range = null,
                                                                         CancellationToken cancellationToken = default) {
    var address = $"stats/sitewide/artist-evolution-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IArtistEvolutionActivity, ArtistEvolutionActivity>(address, options, cancellationToken);
  }

  /// <summary>Gets a user's listening statistics for artists over time.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">
  /// The range of data to include in the statistics. This impacts the time units used by the returned information.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistEvolutionActivity?> GetArtistEvolutionActivityAsync(string user, StatisticsRange? range = null,
                                                                         CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/artist-evolution-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IArtistEvolutionActivity, ArtistEvolutionActivity>(address, options, cancellationToken);
  }

  #endregion

  #region artist-map

  /// <summary>
  /// Gets information about the number of times artists are listened to across all of ListenBrainz, grouped by their country.
  /// </summary>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistMap?> GetArtistMapAsync(StatisticsRange? range = null, CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetArtistMap(range);
    return this.GetOptionalAsync<IArtistMap, ArtistMap>("stats/sitewide/artist-map", options, cancellationToken);
  }

  /// <summary>Gets information about the number of artists a user has listened to, grouped by their country.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistMap?> GetArtistMapAsync(string user, StatisticsRange? range = null,
                                             CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetArtistMap(range);
    return this.GetOptionalAsync<IArtistMap, ArtistMap>($"stats/user/{user}/artist-map", options, cancellationToken);
  }

  #endregion

  #region artists

  /// <summary>Gets statistics about the most listened-to artists across all of ListenBrainz.</summary>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return for each time range. If not specified (or specified
  /// as zero or <see langword="null"/>), the top most listened-to artists will be returned. Note that at most 1000 artists will
  /// be included in the statistics for each time range.
  /// </param>
  /// <param name="range">
  /// The range of data to include in the statistics.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested artist statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistStatistics?> GetArtistStatisticsAsync(int? count = null, int? offset = null, StatisticsRange? range = null,
                                                           CancellationToken cancellationToken = default) {
    const string address = "stats/sitewide/artists";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IArtistStatistics, ArtistStatistics>(address, options, cancellationToken);
  }

  /// <summary>Gets statistics about a user's most listened-to artists.</summary>
  /// <param name="user">The user for whom the statistics are requested.</param>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
  /// <see langword="null"/>), the top most listened-to artists will be returned.
  /// </param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested artist statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistStatistics?> GetArtistStatisticsAsync(string user, int? count = null, int? offset = null,
                                                           StatisticsRange? range = null,
                                                           CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/artists";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IArtistStatistics, ArtistStatistics>(address, options, cancellationToken);
  }

  #endregion

  #region daily-activity

  /// <summary>Gets information about how a user's listens are spread across the days of the week.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the information.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested daily activity, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserDailyActivity?> GetDailyActivityAsync(string user, StatisticsRange? range = null,
                                                         CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/daily-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IUserDailyActivity, UserDailyActivity>(address, options, cancellationToken);
  }

  #endregion

  #region era-activity

  #endregion

  #region genre-activity

  #endregion

  #region listeners

  /// <summary>Gets information about the top listeners for a particular artist.</summary>
  /// <param name="mbid">The MusicBrainz ID for the artist.</param>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return for each time range. If not specified (or specified
  /// as zero or <see langword="null"/>), the top most listened-to artists will be returned. Note that at most 1000 artists will
  /// be included in the statistics for each time range.
  /// </param>
  /// <param name="range">
  /// The range of data to include in the statistics.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listener statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistListeners?> GetArtistListenersAsync(Guid mbid, int? count = null, int? offset = null,
                                                         StatisticsRange? range = null,
                                                         CancellationToken cancellationToken = default) {
    var address = $"stats/artist/{mbid}/listeners";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IArtistListeners, ArtistListeners>(address, options, cancellationToken);
  }

  /// <summary>Gets information about the top listeners for a particular release group.</summary>
  /// <param name="mbid">The MusicBrainz ID for the release group.</param>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return for each time range. If not specified (or specified
  /// as zero or <see langword="null"/>), the top most listened-to artists will be returned. Note that at most 1000 artists will
  /// be included in the statistics for each time range.
  /// </param>
  /// <param name="range">
  /// The range of data to include in the statistics.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listener statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IReleaseGroupListeners?> GetReleaseGroupListenersAsync(Guid mbid, int? count = null, int? offset = null,
                                                                     StatisticsRange? range = null,
                                                                     CancellationToken cancellationToken = default) {
    var address = $"stats/release-group/{mbid}/listeners";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IReleaseGroupListeners, ReleaseGroupListeners>(address, options, cancellationToken);
  }

  #endregion

  #region listening-activity

  /// <summary>Gets information about how many listens have been submitted across to ListenBrainz over a period of time.</summary>
  /// <param name="range">
  /// The range of data to include in the information.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listening activity, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IListeningActivity?> GetListeningActivityAsync(StatisticsRange? range = null,
                                                             CancellationToken cancellationToken = default) {
    const string address = "stats/sitewide/listening-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IListeningActivity, ListeningActivity>(address, options, cancellationToken);
  }

  /// <summary>Gets information about how many listens a user has submitted to ListenBrainz over a period of time.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">
  /// The range of data to include in the information.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listening activity, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IListeningActivity?> GetListeningActivityAsync(string user, StatisticsRange? range = null,
                                                             CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/listening-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IListeningActivity, ListeningActivity>(address, options, cancellationToken);
  }

  #endregion

  #region recordings

  /// <summary>Gets statistics about the most listened-to recordings ("tracks") across all of ListenBrainz.</summary>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
  /// <see langword="null"/>), the top most listened-to recordings will be returned.
  /// </param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested recording statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IRecordingStatistics?> GetRecordingStatisticsAsync(int? count = null, int? offset = null,
                                                                 StatisticsRange? range = null,
                                                                 CancellationToken cancellationToken = default) {
    const string address = "stats/sitewide/recordings";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IRecordingStatistics, RecordingStatistics>(address, options, cancellationToken);
  }

  /// <summary>Gets statistics about a user's most listened-to recordings ("tracks").</summary>
  /// <param name="user">The user for whom the statistics are requested.</param>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
  /// <see langword="null"/>), the top most listened-to recordings will be returned.
  /// </param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested recording statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IRecordingStatistics?> GetRecordingStatisticsAsync(string user, int? count = null, int? offset = null,
                                                                 StatisticsRange? range = null,
                                                                 CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/recordings";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IRecordingStatistics, RecordingStatistics>(address, options, cancellationToken);
  }

  #endregion

  #region release-groups

  /// <summary>
  /// Gets statistics about the most listened-to release groups (sets of all editions of an "album") across all of ListenBrainz.
  /// </summary>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
  /// <see langword="null"/>), the top most listened-to release groups will be returned.
  /// </param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested releases statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IReleaseGroupStatistics?> GetReleaseGroupStatisticsAsync(int? count = null, int? offset = null,
                                                                       StatisticsRange? range = null,
                                                                       CancellationToken cancellationToken = default) {
    const string address = "stats/sitewide/release-groups";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IReleaseGroupStatistics, ReleaseGroupStatistics>(address, options, cancellationToken);
  }

  /// <summary>Gets statistics about a user's most listened-to release groups (sets of all editions of an "album").</summary>
  /// <param name="user">The user for whom the statistics are requested.</param>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
  /// <see langword="null"/>), the top most listened-to release groups will be returned.
  /// </param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested releases statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IReleaseGroupStatistics?> GetReleaseGroupStatisticsAsync(string user, int? count = null, int? offset = null,
                                                                       StatisticsRange? range = null,
                                                                       CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/release-groups";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IReleaseGroupStatistics, ReleaseGroupStatistics>(address, options, cancellationToken);
  }

  #endregion

  #region releases

  /// <summary>Gets statistics about the most listened-to releases ("albums") across all of ListenBrainz.</summary>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
  /// <see langword="null"/>), the top most listened-to releases will be returned.
  /// </param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested releases statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IReleaseStatistics?> GetReleaseStatisticsAsync(int? count = null, int? offset = null,
                                                                 StatisticsRange? range = null,
                                                                 CancellationToken cancellationToken = default) {
    const string address = "stats/sitewide/releases";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IReleaseStatistics, ReleaseStatistics>(address, options, cancellationToken);
  }

  /// <summary>Gets statistics about a user's most listened-to releases ("albums").</summary>
  /// <param name="user">The user for whom the statistics are requested.</param>
  /// <param name="count">
  /// The (maximum) number of entries to return. If not specified (or <see langword="null"/>), all available information will be
  /// returned.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the statistics to return. If not specified (or specified as zero or
  /// <see langword="null"/>), the top most listened-to releases will be returned.
  /// </param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested releases statistics, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IReleaseStatistics?> GetReleaseStatisticsAsync(string user, int? count = null, int? offset = null,
                                                                 StatisticsRange? range = null,
                                                                 CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/releases";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IReleaseStatistics, ReleaseStatistics>(address, options, cancellationToken);
  }

  #endregion

  #region year-in-music

  // While the endpoint is listed in the docs, its payload is not.
  // The top level has:
  // - user_name: the name of the user
  // - data: the year-in-music data, with the following fields:
  //   - artist_map: same contents as the artist-map entry point (with the artist data limited to name, mbid and listen count)
  //   - day_of_week: name of a day of the week; presumably the day the most listens are recorded
  //   - listens_per_day: list of objects (one per day of the year):
  //     - time_range: string containing the date (e.g. "02 October 2022")
  //     - from_ts/to_ts: Unix timestamps
  //     - listen_count
  //   - most_listened_year: dictionary mapping a year (as a string) to the number of listens for recordings from that year
  //   - new_releases_of_top_artists: list of release group info
  //     - similar to what's in the RG stats, but "title" instead of "release_group_name", and no listen count
  //   - playlist-top-discoveries-for-year
  //     - ListenBrainz Troi playlist data
  //   - playlist-top-discoveries-for-year-coverart: map of mbid to CAA URL
  //   - playlist-top-missed-recordings-for-year
  //     - ListenBrainz Troi playlist data
  //   - playlist-top-missed-recordings-for-year-coverart: map of mbid to CAA URL
  //   - similar_users: map of user names (string) to a decimal number (0-1)
  //   - top_artists: list of artist info (similar to what's in the artist map: name, mbid, listen count)
  //   - top_recordings: list of record info (similar to what's in the recording stats)
  //   - top_releases: list of release info (similar to what's in the release stats)
  //   - total_artists_count (an integer)
  //   - total_listen_count (an integer)
  //   - total_listening_time (a decimal number; presumably expressed in minutes)
  //   - total_new_artists_discovered (an integer)
  //   - total_recordings_count (an integer)
  //   - total_releases_count (an integer)
  //   - yim_artist_map: identical to artist_map

  #endregion

  #endregion

  #region /1/submit-listens

  private Task SubmitListensAsync<T>(T payload, CancellationToken cancellationToken)
    => this.PostAsync("submit-listens", payload, null, cancellationToken);

  private Task SubmitListensAsync(string payload, CancellationToken cancellationToken)
    => this.PostAsync("submit-listens", payload, null, cancellationToken);

  #region Import Listens

  /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="listens">The listens to import.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on <a href="https://listenbrainz.org/profile/">their profile page</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensPerRequest"/> listens. As such, one call to this method may result in
  /// multiple web service requests, which may affect rate limiting.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public async Task ImportListensAsync(IAsyncEnumerable<ISubmittedListen> listens, CancellationToken cancellationToken = default) {
    var payload = new ImportPayload();
    await foreach (var listen in listens.ConfigureAwait(false).WithCancellation(cancellationToken)) {
      payload.Listens.Add(listen);
      if (payload.Listens.Count >= ListenBrainz.MaxListensPerRequest) {
        await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
        payload.Listens.Clear();
      }
    }
    if (payload.Listens.Count != 0) {
      await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
    }
  }

  /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="listens">The listens to import.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on <a href="https://listenbrainz.org/profile/">their profile page</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensPerRequest"/> listens. As such, one call to this method may result in
  /// multiple web service requests, which may affect rate limiting.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public async Task ImportListensAsync(IEnumerable<ISubmittedListen> listens, CancellationToken cancellationToken = default) {
    var payload = new ImportPayload();
    foreach (var listen in listens) {
      payload.Listens.Add(listen);
      if (payload.Listens.Count >= ListenBrainz.MaxListensPerRequest) {
        await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
        payload.Listens.Clear();
      }
    }
    if (payload.Listens.Count != 0) {
      await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
    }
  }

  private async Task ImportListensAsync(IEnumerable<string> serializedListens, CancellationToken cancellationToken = default) {
    foreach (var json in serializedListens) {
      await this.SubmitListensAsync(json, cancellationToken).ConfigureAwait(false);
    }
  }

  /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <param name="listens">The listens to import.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on <a href="https://listenbrainz.org/profile/">their profile page</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensPerRequest"/> listens. As such, one call to this method may result in
  /// multiple web service requests, which may affect rate limiting.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task ImportListensAsync(CancellationToken cancellationToken, params ISubmittedListen[] listens)
    => this.ImportListensAsync(listens, cancellationToken);

  /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="listens">The listens to import.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on <a href="https://listenbrainz.org/profile/">their profile page</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensPerRequest"/> listens. As such, one call to this method may result in
  /// multiple web service requests, which may affect rate limiting.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task ImportListensAsync(params ISubmittedListen[] listens)
    => this.ImportListensAsync((IEnumerable<ISubmittedListen>) listens);

  private IEnumerable<string> SerializeImport(ListenSubmissionPayload<ISubmittedListen> payload) {
    var json = JsonSerializer.Serialize(payload, ListenBrainz.JsonWriterOptions);
    // If it's small enough, or we can't split up the listens, we're done.
    if (json.Length <= ListenBrainz.MaxListenPayloadSize || payload.Listens.Count <= 1) {
      yield return json;
      yield break;
    }
    // Otherwise, split the list of listens in half.
    var halfWayPoint = payload.Listens.Count / 2;
    { // Recurse over first half.
      var partialPayload = new ImportPayload {
        Listens = payload.Listens[..halfWayPoint],
      };
      foreach (var part in this.SerializeImport(partialPayload)) {
        yield return part;
      }
    }
    { // Recurse over second half.
      var partialPayload = new ImportPayload {
        Listens = payload.Listens[halfWayPoint..],
      };
      foreach (var part in this.SerializeImport(partialPayload)) {
        yield return part;
      }
    }
  }

  #endregion

  #region Set "Now Playing"

  /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="listen">The listen data to send.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SetNowPlayingAsync(ISubmittedListenData listen, CancellationToken cancellationToken = default)
    => this.SubmitListensAsync(new PlayingNowPayload(listen), cancellationToken);

  /// <summary>Sets the "now playing" information for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  [Obsolete("Create a SubmittedListenData and pass it to the overload taking an ISubmittedListenData instead.")]
  public Task SetNowPlayingAsync(string track, string artist, string? release = null,
                                 CancellationToken cancellationToken = default) {
    return this.SetNowPlayingAsync(new SubmittedListenData {
      Track = new SubmittedTrackInfo {
        Artist = artist,
        Name = track,
        Release = release,
      },
    }, cancellationToken);
  }

  #endregion

  #region Submit Single Listen

  /// <summary>
  /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
  /// </summary>
  /// <param name="listen">The listen data to send.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SubmitSingleListenAsync(ISubmittedListen listen, CancellationToken cancellationToken = default)
    => this.SubmitListensAsync(new SingleListenPayload(listen), cancellationToken);

  /// <summary>
  /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
  /// </summary>
  /// <param name="timestamp">The date and time at which the track was listened to.</param>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  [Obsolete("Create a SubmittedListen and pass it to the overload taking an ISubmittedListen instead.")]
  public Task SubmitSingleListenAsync(DateTimeOffset timestamp, string track, string artist, string? release = null,
                                      CancellationToken cancellationToken = default) {
    return this.SubmitSingleListenAsync(new SubmittedListen {
      Timestamp = timestamp,
      Track = new SubmittedTrackInfo {
        Artist = artist,
        Name = track,
        Release = release,
      },
    }, cancellationToken);
  }

  /// <summary>
  /// Submits a single listen (typically one that has just completed) for the user whose token is set in <see cref="UserToken"/>.
  /// </summary>
  /// <param name="timestamp">
  /// The date and time at which the track was listened to, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>.
  /// </param>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  [Obsolete("Create a SubmittedListen and pass it to the overload taking an ISubmittedListen instead.")]
  public Task SubmitSingleListenAsync(long timestamp, string track, string artist, string? release = null,
                                      CancellationToken cancellationToken = default) {
    return this.SubmitSingleListenAsync(new SubmittedListen {
      Track = new SubmittedTrackInfo {
        Artist = artist,
        Name = track,
        Release = release,
      },
      UnixTimestamp = timestamp,
    }, cancellationToken);
  }

  /// <summary>
  /// Submits a single listen for the user whose token is set in <see cref="UserToken"/>, using the current (UTC) date and time as
  /// timestamp.
  /// </summary>
  /// <param name="track">The name of the track being listened to.</param>
  /// <param name="artist">The name of the artist performing the track being listened to.</param>
  /// <param name="release">The name of the release containing the track being listened to.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  [Obsolete("Create a SubmittedListen and pass it to the overload taking an ISubmittedListen instead.")]
  public Task SubmitSingleListenAsync(string track, string artist, string? release = null,
                                      CancellationToken cancellationToken = default) {
    return this.SubmitSingleListenAsync(new SubmittedListen {
      Track = new SubmittedTrackInfo {
        Artist = artist,
        Name = track,
        Release = release,
      }
    }, cancellationToken);
  }

  #endregion

  #endregion

  #region /1/user/xxx/listen-count

  /// <summary>Gets the number of listens submitted to ListenBrainz by a particular user.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose listen count is requested.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>An object providing the number of listens submitted by <paramref name="user"/>.</returns>
  /// <remarks>This will access the <c>GET /1/user/USER/listen-count</c> endpoint.</remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IListenCount> GetListenCountAsync(string user, CancellationToken cancellationToken = default)
    => this.GetAsync<IListenCount, ListenCount>($"user/{user}/listen-count", null, cancellationToken);

  #endregion

  #region /1/user/xxx/listens

  #region Internal Helpers

  private Task<IFetchedListens> PerformGetListensAsync(string user, long? after, long? before, int? count = null,
                                                       int? timeRange = null, CancellationToken cancellationToken = default) {
    var options = new Dictionary<string, string>(4);
    if (count is not null) {
      options.Add("count", count.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (before is not null) {
      options.Add("max_ts", before.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (after is not null) {
      options.Add("min_ts", after.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (timeRange is not null) {
      options.Add("time_range", timeRange.Value.ToString(CultureInfo.InvariantCulture));
    }
    return this.GetAsync<IFetchedListens, FetchedListens>($"user/{user}/listens", options, cancellationToken);
  }

  #endregion

  #region No Timestamps

  /// <summary>Gets the most recent listens for a user.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="timeRange">
  /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
  /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensAsync(string user, int? count = null, int? timeRange = null,
                                               CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, null, null, count, timeRange, cancellationToken);

  #endregion

  #region Minimum Timestamp

  /// <summary>Gets the most recent listens for a user, starting from a particular timestamp.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="after">
  /// The timestamp to start from, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp greater than, but not
  /// including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="timeRange">
  /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
  /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensAfterAsync(string user, long after, int? count = null, int? timeRange = null,
                                                    CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, after, null, count, timeRange, cancellationToken);

  /// <summary>Gets the most recent listens for a user, starting from a particular timestamp.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="after">
  /// The timestamp to start from (with a precision of seconds).
  /// Returned listens will have a timestamp greater than, but not including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="timeRange">
  /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
  /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensAfterAsync(string user, DateTimeOffset after, int? count = null,
                                                    int? timeRange = null, CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, after.ToUnixTimeSeconds(), null, count, timeRange, cancellationToken);

  #endregion

  #region Maximum Timestamp

  /// <summary>Gets historical listens for a user, starting from a particular timestamp.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="before">
  /// The timestamp to start from, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp less than, but not
  /// including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="timeRange">
  /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
  /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensBeforeAsync(string user, long before, int? count = null, int? timeRange = null,
                                                     CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, null, before, count, timeRange, cancellationToken);

  /// <summary>Gets historical listens for a user, starting from a particular timestamp.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="before">
  /// The timestamp to start from (with a precision of seconds).
  /// Returned listens will have a timestamp less than, but not including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="timeRange">
  /// The time range to search, in sets of 5 days; must be no greater than <see cref="MaxTimeRange"/>.<br/>
  /// If not specified, <see cref="DefaultTimeRange"/> will be used as time range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensBeforeAsync(string user, DateTimeOffset before, int? count = null,
                                                     int? timeRange = null, CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, null, before.ToUnixTimeSeconds(), count, timeRange, cancellationToken);

  #endregion

  #region Both Timestamps

  /// <summary>Gets the listens for a user in a specific timespan.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="after">
  /// The timestamp to start from, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp greater than, but not
  /// including, this value.
  /// </param>
  /// <param name="before">
  /// The timestamp to end at, expressed as the number of seconds since
  /// <see cref="DateTimeOffset.UnixEpoch">the Unix time epoch</see>. Returned listens will have a timestamp less than, but not
  /// including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensBetweenAsync(string user, long after, long before, int? count = null,
                                                      CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, after, before, count, null, cancellationToken);

  /// <summary>Gets the listens for a user in a specific timespan.</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="after">
  /// The timestamp to start from (with a precision of seconds).
  /// Returned listens will have a timestamp greater than, but not including, this value.
  /// </param>
  /// <param name="before">
  /// The timestamp to end at (with a precision of seconds).
  /// Returned listens will have a timestamp less than, but not including, this value.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of listens to return; must be no greater than <see cref="MaxItemsPerGet"/>.<br/>
  /// If not specified, this will return up to <see cref="DefaultItemsPerGet"/> listens.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens, in descending timestamp order.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFetchedListens> GetListensBetweenAsync(string user, DateTimeOffset after, DateTimeOffset before, int? count = null,
                                                      CancellationToken cancellationToken = default)
    => this.PerformGetListensAsync(user, after.ToUnixTimeSeconds(), before.ToUnixTimeSeconds(), count, null, cancellationToken);

  #endregion

  #endregion

  #region /1/user/xxx/playing-now

  /// <summary>Gets a user's currently-playing listen(s).</summary>
  /// <param name="user">The MusicBrainz ID of the user whose data is needed.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listens (typically 0 or 1).</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IPlayingNow> GetPlayingNowAsync(string user, CancellationToken cancellationToken = default)
    => this.GetAsync<IPlayingNow, PlayingNow>($"user/{user}/playing-now", null, cancellationToken);

  #endregion

  #region /1/validate-token

  /// <summary>Validates a given user token.</summary>
  /// <param name="token">The user token to validate.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The result of the validation.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<ITokenValidationResult> ValidateTokenAsync(string token, CancellationToken cancellationToken = default) {
    var options = new Dictionary<string, string> { ["token"] = token };
    return this.GetAsync<ITokenValidationResult, TokenValidationResult>("validate-token", options, cancellationToken);
  }

  #endregion

}
