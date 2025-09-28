using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using MetaBrainz.Common;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Interfaces.JSPF;
using MetaBrainz.ListenBrainz.Objects;
using MetaBrainz.ListenBrainz.Objects.JSPF;

namespace MetaBrainz.ListenBrainz;

public sealed partial class ListenBrainz {

  #region /1/playlist

  /// <summary>Gets a ListenBrainz playlist by its ID.</summary>
  /// <param name="id">The ID of the ListenBrainz playlist to retrieve.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The requested playlist.</returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IPlaylist> GetPlaylistAsync(Guid id, CancellationToken cancellationToken = default)
    => this.GetAsync<IPlaylist, Playlist>($"playlist/{id}", null, cancellationToken);

  #endregion

  #region /1/playlist/search

  /// <summary>Searches for playlists by name or description.</summary>
  /// <param name="query">The query string. Must be at least 3 characters in length.</param>
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
  public Task<IFoundPlaylists> FindPlaylistsAsync(string query, int? count = null, int? offset = null,
                                                  CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForPageableResource(3, count, offset);
    options.Add("query", query);
    return this.GetAsync<IFoundPlaylists, FoundPlaylists>("playlist/search", options, cancellationToken);
  }

  #endregion

}
