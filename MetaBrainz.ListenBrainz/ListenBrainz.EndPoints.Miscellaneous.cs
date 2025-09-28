using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using MetaBrainz.Common;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz;

public sealed partial class ListenBrainz {

  #region /1/explore/lb-radio

  /// <summary>Generates an LB Radio playlist based on a prompt.</summary>
  /// <param name="prompt">
  /// The LB radio prompt to use. See <a href="https://troi.readthedocs.io/en/latest/lb_radio.html">the official documentation</a>
  /// for details.
  /// </param>
  /// <param name="mode">The generation mode to use.</param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>
  /// The generated playlist. Note that this is not deterministic; multiple invocations with the same prompt may produce different
  /// playlists.
  /// </returns>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<ILBRadioPlaylist> CreateLBRadioPlaylistAsync(string prompt, LBRadioMode mode,
                                                           CancellationToken cancellationToken = default) {
    var options = new Dictionary<string, string> {
      ["prompt"] = prompt,
      ["mode"] = mode.ToString().ToLowerInvariant(),
    };
    return this.GetAsync<ILBRadioPlaylist, LBRadioPlaylist>("explore/lb-radio", options, cancellationToken);
  }

  #endregion

}
