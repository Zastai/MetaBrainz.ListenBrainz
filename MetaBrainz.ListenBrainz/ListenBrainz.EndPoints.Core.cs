using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MetaBrainz.Common;
using MetaBrainz.ListenBrainz.Interfaces;
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

  #region /1/user/xxx/playlists/

  /// <summary>Gets playlists that were created by a specific user.</summary>
  /// <param name="user">The MusicBrainz ID of the user.</param>
  /// <param name="count">
  /// The (maximum) number of results to return. If not specified (or <see langword="null"/>), up to
  /// <seealso cref="DefaultItemsPerGet"/> will be returned. Values over <see cref="MaxItemsPerGet"/> will be treated as if they
  /// were exactly <see cref="MaxItemsPerGet"/>.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the matching playlists to return. If this is equal to or higher than the total
  /// number of matches, no results will be returned.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The playlists that were found (if any). These will only contain the playlist information, not their tracks.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFoundPlaylists> GetPlaylistsCreatedByAsync(string user, int? count = null, int? offset = null,
                                                          CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForPageableResource(2, count, offset);
    return this.GetAsync<IFoundPlaylists, FoundPlaylists>($"user/{user}/playlists", options, cancellationToken);
  }

  #endregion

  #region /1/user/xxx/playlists/collaborator

  /// <summary>Gets playlists where a specific user is listed as a collaborator.</summary>
  /// <param name="user">The MusicBrainz ID of the user.</param>
  /// <param name="count">
  /// The (maximum) number of results to return. If not specified (or <see langword="null"/>), up to
  /// <seealso cref="DefaultItemsPerGet"/> will be returned. Values over <see cref="MaxItemsPerGet"/> will be treated as if they
  /// were exactly <see cref="MaxItemsPerGet"/>.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the matching playlists to return. If this is equal to or higher than the total
  /// number of matches, no results will be returned.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The playlists that were found (if any). These will only contain the playlist information, not their tracks.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFoundPlaylists> GetCollaboratorPlaylistsAsync(string user, int? count = null, int? offset = null,
                                                             CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForPageableResource(2, count, offset);
    return this.GetAsync<IFoundPlaylists, FoundPlaylists>($"user/{user}/playlists/recommendations", options, cancellationToken);
  }

  #endregion

  #region /1/user/xxx/playlists/createdfor

  /// <summary>Gets playlists that were created for a specific user.</summary>
  /// <param name="user">The MusicBrainz ID of the user.</param>
  /// <param name="count">
  /// The (maximum) number of results to return. If not specified (or <see langword="null"/>), up to
  /// <seealso cref="DefaultItemsPerGet"/> will be returned. Values over <see cref="MaxItemsPerGet"/> will be treated as if they
  /// were exactly <see cref="MaxItemsPerGet"/>.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the matching playlists to return. If this is equal to or higher than the total
  /// number of matches, no results will be returned.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The playlists that were found (if any). These will only contain the playlist information, not their tracks.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFoundPlaylists> GetPlaylistsCreatedForAsync(string user, int? count = null, int? offset = null,
                                                           CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForPageableResource(2, count, offset);
    return this.GetAsync<IFoundPlaylists, FoundPlaylists>($"user/{user}/playlists/createdfor", options, cancellationToken);
  }

  #endregion

  #region /1/user/xxx/playlists/recommendations

  /// <summary>Gets recommendation playlists for a specific user.</summary>
  /// <param name="user">The MusicBrainz ID of the user.</param>
  /// <param name="count">
  /// The (maximum) number of results to return. If not specified (or <see langword="null"/>), up to
  /// <seealso cref="DefaultItemsPerGet"/> will be returned. Values over <see cref="MaxItemsPerGet"/> will be treated as if they
  /// were exactly <see cref="MaxItemsPerGet"/>.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the matching playlists to return. If this is equal to or higher than the total
  /// number of matches, no results will be returned.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The playlists that were found (if any). These will only contain the playlist information, not their tracks.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFoundPlaylists> GetRecommendationPlaylistsAsync(string user, int? count = null, int? offset = null,
                                                               CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForPageableResource(2, count, offset);
    return this.GetAsync<IFoundPlaylists, FoundPlaylists>($"user/{user}/playlists/recommendations", options, cancellationToken);
  }

  #endregion

  #region /1/user/xxx/playlists/search

  /// <summary>Finds playlists for a specific user by name.</summary>
  /// <param name="user">The MusicBrainz ID of the user.</param>
  /// <param name="name">The playlist name.</param>
  /// <param name="count">
  /// The (maximum) number of results to return. If not specified (or <see langword="null"/>), up to
  /// <seealso cref="DefaultItemsPerGet"/> will be returned. Values over <see cref="MaxItemsPerGet"/> will be treated as if they
  /// were exactly <see cref="MaxItemsPerGet"/>.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the matching playlists to return. If this is equal to or higher than the total
  /// number of matches, no results will be returned.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The playlists that were found (if any). These will only contain the playlist information, not their tracks.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFoundPlaylists> FindPlaylistsByNameAsync(string user, string name, int? count = null, int? offset = null,
                                                        CancellationToken cancellationToken = default) {
    // LB-1837: The documentation says the parameter is `name`, but it actually responds to `query` instead. So pass both.
    var options = ListenBrainz.OptionsForPageableResource(4, count, offset);
    options["name"] = name;
    options["query"] = name;
    return this.GetAsync<IFoundPlaylists, FoundPlaylists>($"user/{user}/playlists/search", options, cancellationToken);
  }

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
