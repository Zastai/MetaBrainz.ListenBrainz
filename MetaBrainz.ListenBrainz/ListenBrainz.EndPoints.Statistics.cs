using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using MetaBrainz.Common;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Json;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz;

public sealed partial class ListenBrainz {

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

  #region /1/stats/artist/xxx/listeners

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
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IArtistListeners, ArtistListeners>($"stats/artist/{mbid}/listeners", options, cancellationToken);
  }

  #endregion

  #region /1/stats/release-group/xxx/listeners

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

  #region /1/stats/sitewide/artist-activity

  /// <summary>Gets listening statistics for the top artists (and their albums) across all of ListenBrainz.</summary>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistActivity?> GetArtistActivityAsync(StatisticsRange? range = null,
                                                       CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IArtistActivity, ArtistActivity>("stats/sitewide/artist-activity", options, cancellationToken);
  }

  #endregion

  #region /1/stats/sitewide/artist-evolution-activity

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
    const string address = "stats/sitewide/artist-evolution-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IArtistEvolutionActivity, ArtistEvolutionActivity>(address, options, cancellationToken);
  }

  #endregion

  #region /1/stats/sitewide/artist-map

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

  #endregion

  #region /1/stats/sitewide/artists

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
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IArtistStatistics, ArtistStatistics>("stats/sitewide/artists", options, cancellationToken);
  }

  #endregion

  #region /1/stats/sitewide/era-activity

  /// <summary>
  /// Gets information about how many listens have been recorded across all of ListenBrainz, grouped by their release year.
  /// </summary>
  /// <param name="range">
  /// The range of data to include in the information.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listening activity, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IEraActivity?> GetEraActivityAsync(StatisticsRange? range = null, CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IEraActivity, EraActivity>("stats/sitewide/era-activity", options, cancellationToken);
  }

  #endregion

  #region /1/stats/sitewide/listening-activity

  /// <summary>Gets information about how many listens have been submitted to ListenBrainz over a period of time.</summary>
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

  #endregion

  #region /1/stats/sitewide/recordings

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

  #endregion

  #region /1/stats/sitewide/release-groups

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

  #endregion

  #region /1/stats/sitewide/releases

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
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IReleaseStatistics, ReleaseStatistics>("stats/sitewide/releases", options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/artist-activity

  /// <summary>Gets listening statistics for a user's top artists (and their albums).</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IArtistActivity?> GetArtistActivityAsync(string user, StatisticsRange? range = null,
                                                       CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IArtistActivity, ArtistActivity>($"stats/user/{user}/artist-activity", options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/artist-evolution-activity

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

  #region /1/stats/user/xxx/artist-map

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

  #region /1/stats/user/xxx/artists

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
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IArtistStatistics, ArtistStatistics>($"stats/user/{user}/artists", options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/daily-activity

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

  #region /1/stats/user/xxx/era-activity

  /// <summary>Gets information about how many listens have been recorded for a user, grouped by their release year.</summary>
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
  public Task<IEraActivity?> GetEraActivityAsync(string user, StatisticsRange? range = null,
                                                 CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IEraActivity, EraActivity>($"stats/user/{user}/era-activity", options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/genre-activity

  /// <summary>
  /// Gets information about how many listens have been recorded for a user, grouped by the genre and the hour of the day.
  /// </summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the information.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listening activity, if available.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IGenreActivity?> GetGenreActivityAsync(string user, StatisticsRange? range = null,
                                                 CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IGenreActivity, GenreActivity>($"stats/user/{user}/genre-activity", options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/listening-activity

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

  #region /1/stats/user/xxx/recordings

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

  #region /1/stats/user/xxx/release-groups

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

  #region /1/stats/user/xxx/releases

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
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IReleaseStatistics, ReleaseStatistics>($"stats/user/{user}/releases", options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/year-in-music

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

}
