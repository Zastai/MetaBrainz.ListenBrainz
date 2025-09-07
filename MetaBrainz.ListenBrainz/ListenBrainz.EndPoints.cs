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

  #region /1/stats/sitewide

  #region /1/stats/sitewide/artists

  /// <summary>Gets statistics about the most listened-to artists.</summary>
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
  /// <returns>The requested artist statistics.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<ISiteArtistStatistics?> GetArtistStatisticsAsync(int? count = null, int? offset = null,
                                                               StatisticsRange? range = null,
                                                               CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<ISiteArtistStatistics, SiteArtistStatistics>("stats/sitewide/artists", options, cancellationToken);
  }

  #endregion

  #endregion

  #region /1/stats/user/xxx

  #region /1/stats/user/xxx/artist-map

  /// <summary>Gets information about the number of artists a user has listened to, grouped by their country.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the statistics.</param>
  /// <param name="forceRecalculation">Indicates whether recalculation of the data should be requested.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested information, or <see langword="null"/> if it has not yet been computed for the user.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserArtistMap?> GetArtistMapAsync(string user, StatisticsRange? range = null, bool forceRecalculation = false,
                                                 CancellationToken cancellationToken = default) {
    var options = new Dictionary<string, string>(2);
    if (range is not null) {
      options.Add("range", range.Value.ToJson());
    }
    if (forceRecalculation) {
      options.Add("force_recalculate", "true");
    }
    return this.GetOptionalAsync<IUserArtistMap, UserArtistMap>($"stats/user/{user}/artist-map", options, cancellationToken);
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
  /// <returns>
  /// The requested artist statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
  /// </returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserArtistStatistics?> GetArtistStatisticsAsync(string user, int? count = null, int? offset = null,
                                                               StatisticsRange? range = null,
                                                               CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/artists";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IUserArtistStatistics, UserArtistStatistics>(address, options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/daily-activity

  /// <summary>Gets information about how a user's listens are spread across the days of the week.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">The range of data to include in the information.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested daily activity.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserDailyActivity?> GetDailyActivityAsync(string user, StatisticsRange? range = null,
                                                         CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/daily-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IUserDailyActivity, UserDailyActivity>(address, options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/listening-activity

  /// <summary>Gets information about how many listens a user has submitted over a period of time.</summary>
  /// <param name="user">The user for whom the information is requested.</param>
  /// <param name="range">
  /// The range of data to include in the information.<br/>
  /// If this is unspecified or <see cref="StatisticsRange.AllTime"/>, information is returned about all years with at least one
  /// recorded listen. Otherwise, information is returned about both the current and the previous range.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested listening activity.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserListeningActivity?> GetListeningActivityAsync(string user, StatisticsRange? range = null,
                                                                 CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/listening-activity";
    var options = ListenBrainz.OptionsForGetStatistics(null, null, range);
    return this.GetOptionalAsync<IUserListeningActivity, UserListeningActivity>(address, options, cancellationToken);
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
  /// <returns>
  /// The requested recording statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
  /// </returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserRecordingStatistics?> GetRecordingStatisticsAsync(string user, int? count = null, int? offset = null,
                                                                     StatisticsRange? range = null,
                                                                     CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/recordings";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IUserRecordingStatistics, UserRecordingStatistics>(address, options, cancellationToken);
  }

  #endregion

  #region /1/stats/user/xxx/release-groups

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
  /// <returns>
  /// The requested releases statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
  /// </returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserReleaseGroupStatistics?> GetReleaseGroupStatisticsAsync(string user, int? count = null, int? offset = null,
                                                                           StatisticsRange? range = null,
                                                                           CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/release-groups";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IUserReleaseGroupStatistics, UserReleaseGroupStatistics>(address, options, cancellationToken);
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
  /// <returns>
  /// The requested releases statistics, or <see langword="null"/> if statistics have not yet been computed for the user.
  /// </returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IUserReleaseStatistics?> GetReleaseStatisticsAsync(string user, int? count = null, int? offset = null,
                                                                 StatisticsRange? range = null,
                                                                 CancellationToken cancellationToken = default) {
    var address = $"stats/user/{user}/releases";
    var options = ListenBrainz.OptionsForGetStatistics(count, offset, range);
    return this.GetOptionalAsync<IUserReleaseStatistics, UserReleaseStatistics>(address, options, cancellationToken);
  }

  #endregion

  #endregion

  #endregion

  #region /1/submit-listens

  private Task SubmitListensAsync<T>(T payload, CancellationToken cancellationToken)
    => this.PostAsync("submit-listens", payload, null, cancellationToken);

  private Task SubmitListensAsync(string payload, CancellationToken cancellationToken)
    => this.PostAsync("submit-listens", payload, null, cancellationToken);

  #region Import Listens

  /// <summary>The maximum number of listens that can fit into a single API request.</summary>
  /// <remarks>
  /// A JSON listen payload contains:
  /// <list type="bullet">
  /// <item>
  ///   <term>a minimum of 37 characters of fixed overhead: </term>
  ///   <description><c>{"listen_type":"import","payload":[]}</c></description>
  /// </item>
  /// <item>
  ///   <term>a minimum of 71 characters for the listen data: </term>
  ///   <description><c>{"listened_at":0,"track_metadata":{"artist_name":"?","track_name":"?"}}</c></description>
  /// </item>
  /// </list>
  /// The listens are comma-separated, so we need to add one to the listen size and subtract one from the fixed overhead.<br/>
  /// So the maximum listens that can be submitted at once is <c>(<see cref="MaxListenSize"/> - 36) / 72</c> (currently 141).
  /// </remarks>
  private const int MaxListensInOnePayload = (ListenBrainz.MaxListenSize - 36) / 72;

  /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="listens">The listens to import.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
  /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
  /// call to this method may result in multiple web service requests, which may affect rate limiting.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public async Task ImportListensAsync(IAsyncEnumerable<ISubmittedListen> listens, CancellationToken cancellationToken = default) {
    var payload = SubmissionPayload.CreateImport();
    await foreach (var listen in listens.ConfigureAwait(false).WithCancellation(cancellationToken)) {
      payload.Listens.Add(listen);
      if (payload.Listens.Count < ListenBrainz.MaxListensInOnePayload) {
        continue;
      }
      await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
      payload.Listens.Clear();
    }
    if (payload.Listens.Count == 0) {
      return;
    }
    await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
  }

  /// <summary>Imports a set of listens for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="listens">The listens to import.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/submit-listens</c> endpoint.<br/>
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
  /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
  /// call to this method may result in multiple web service requests, which may affect rate limiting.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public async Task ImportListensAsync(IEnumerable<ISubmittedListen> listens, CancellationToken cancellationToken = default) {
    var payload = SubmissionPayload.CreateImport();
    foreach (var listen in listens) {
      payload.Listens.Add(listen);
      if (payload.Listens.Count < ListenBrainz.MaxListensInOnePayload) {
        continue;
      }
      await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
      payload.Listens.Clear();
    }
    if (payload.Listens.Count == 0) {
      return;
    }
    await this.ImportListensAsync(this.SerializeImport(payload), cancellationToken).ConfigureAwait(false);
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
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
  /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
  /// call to this method may result in multiple web service requests, which may affect rate limiting.
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
  /// Users can find their token on their profile page:
  /// <a href="https://listenbrainz.org/profile/">https://listenbrainz.org/profile/</a>.<br/>
  /// Submissions will happen every <see cref="MaxListensInOnePayload"/> listens, and if a submission's listen data would exceed
  /// <see cref="MaxListenSize"/>, this will split them up and submit them in chunks to avoid hitting that limit. As such, one
  /// call to this method may result in multiple web service requests, which may affect rate limiting.
  /// </remarks>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task ImportListensAsync(params ISubmittedListen[] listens)
    => this.ImportListensAsync((IEnumerable<ISubmittedListen>) listens);

  private IEnumerable<string> SerializeImport(SubmissionPayload<ISubmittedListen> payload) {
    var json = JsonSerializer.Serialize(payload, ListenBrainz.JsonWriterOptions);
    // If it's small enough, or we can't split up the listens, we're done
    if (json.Length <= ListenBrainz.MaxListenSize || payload.Listens.Count <= 1) {
      yield return json;
      yield break;
    }
    // Otherwise, split the list of listens in half
    var firstHalf = payload.Listens.Count / 2;
    var secondHalf = payload.Listens.Count - firstHalf;
    { // Recurse over first half
      var partialPayload = SubmissionPayload.CreateImport();
      partialPayload.Listens.AddRange(payload.Listens.GetRange(0, firstHalf));
      foreach (var part in this.SerializeImport(partialPayload)) {
        yield return part;
      }
    }
    { // Recurse over second half
      var partialPayload = SubmissionPayload.CreateImport();
      partialPayload.Listens.AddRange(payload.Listens.GetRange(firstHalf, secondHalf));
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
    => this.SubmitListensAsync(SubmissionPayload.CreatePlayingNow(listen), cancellationToken);

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
  public Task SetNowPlayingAsync(string track, string artist, string? release = null, CancellationToken cancellationToken = default)
    => this.SetNowPlayingAsync(new SubmittedListenData(track, artist, release), cancellationToken);

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
    => this.SubmitListensAsync(SubmissionPayload.CreateSingle(listen), cancellationToken);

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
  public Task SubmitSingleListenAsync(DateTimeOffset timestamp, string track, string artist, string? release = null,
                                      CancellationToken cancellationToken = default)
    => this.SubmitSingleListenAsync(new SubmittedListen(timestamp, track, artist, release), cancellationToken);

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
  public Task SubmitSingleListenAsync(long timestamp, string track, string artist, string? release = null,
                                      CancellationToken cancellationToken = default)
    => this.SubmitSingleListenAsync(new SubmittedListen(timestamp, track, artist, release), cancellationToken);

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
  public Task SubmitSingleListenAsync(string track, string artist, string? release = null,
                                      CancellationToken cancellationToken = default)
    => this.SubmitSingleListenAsync(new SubmittedListen(track, artist, release), cancellationToken);

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
    var options = new Dictionary<string, string> {
      ["token"] = token
    };
    return this.GetAsync<ITokenValidationResult, TokenValidationResult>("validate-token", options, cancellationToken);
  }

  #endregion

}
