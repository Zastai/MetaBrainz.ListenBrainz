using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using MetaBrainz.Common;
using MetaBrainz.ListenBrainz.Interfaces;
using MetaBrainz.ListenBrainz.Objects;

namespace MetaBrainz.ListenBrainz;

public sealed partial class ListenBrainz {

  private static Dictionary<string, string> OptionsForFeedback(int? score, int? count, int? offset) {
    var options = new Dictionary<string, string>(3);
    if (score is not null) {
      if (score is not (1 or -1)) {
        const string msg = "The score must be either 1 (to return only loved recordings) or -1 (to return only hated recordings).";
        throw new ArgumentOutOfRangeException(nameof(score), score, msg);
      }
      options.Add("score", score.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (count is not null) {
      options.Add("count", count.Value.ToString(CultureInfo.InvariantCulture));
    }
    if (offset is not null) {
      options.Add("offset", offset.Value.ToString(CultureInfo.InvariantCulture));
    }
    return options;
  }

  #region /1/feedback/recording/xxx/get-feedback

  /// <summary>Gets feedback logged for a recording.</summary>
  /// <param name="msid">The MessyBrainz ID identifying the recording.</param>
  /// <param name="score">
  /// Specify this as 1 to only return information about users who loved the recording; specify it as -1 for those who hated it.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of results to return. If not specified (or <see langword="null"/>), up to
  /// <seealso cref="DefaultItemsPerGet"/> will be returned. Values over <see cref="MaxItemsPerGet"/> will be treated as if they
  /// were exactly <see cref="MaxItemsPerGet"/>.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the matching feedback items to return. If this is equal to or higher than the
  /// total number of matches, no results will be returned.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The feedback that was found (if any).</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  /// When <paramref name="score"/> is specified with a value other than 1 or -1.
  /// </exception>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFoundFeedback> GetFeedbackForMessyRecordingAsync(Guid msid, int? score = null, int? count = null, int? offset = null,
                                                                CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForFeedback(score, count, offset);
    return this.GetAsync<IFoundFeedback, FoundFeedback>($"feedback/recording/{msid}/get-feedback", options, cancellationToken);
  }

  #endregion

  #region /1/feedback/recording/xxx/get-feedback-mbid

  /// <summary>Gets feedback logged for a recording.</summary>
  /// <param name="mbid">The MusicBrainz ID identifying the recording.</param>
  /// <param name="score">
  /// Specify this as 1 to only return information about users who loved the recording; specify it as -1 for those who hated it.
  /// </param>
  /// <param name="count">
  /// The (maximum) number of results to return. If not specified (or <see langword="null"/>), up to
  /// <seealso cref="DefaultItemsPerGet"/> will be returned. Values over <see cref="MaxItemsPerGet"/> will be treated as if they
  /// were exactly <see cref="MaxItemsPerGet"/>.
  /// </param>
  /// <param name="offset">
  /// The offset (from the start of the results) of the matching feedback items to return. If this is equal to or higher than the
  /// total number of matches, no results will be returned.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <returns>The feedback that was found (if any).</returns>
  /// <exception cref="ArgumentOutOfRangeException">
  /// When <paramref name="score"/> is specified with a value other than 1 or -1.
  /// </exception>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task<IFoundFeedback> GetFeedbackForRecordingAsync(Guid mbid, int? score = null, int? count = null, int? offset = null,
                                                           CancellationToken cancellationToken = default) {
    var options = ListenBrainz.OptionsForFeedback(score, count, offset);
    return this.GetAsync<IFoundFeedback, FoundFeedback>($"feedback/recording/{mbid}/get-feedback-mbid", options, cancellationToken);
  }

  #endregion

  #region /1/feedback/recording-feedback

  /// <summary>Submits recording feedback for the user whose token is set in <see cref="UserToken"/>.</summary>
  /// <param name="feedback">
  /// The feedback to submit.<br/>
  /// At least one of its <see cref="ISubmittedRecordingFeedback.Id"/> and <see cref="ISubmittedRecordingFeedback.MessyId"/>
  /// properties must be set.
  /// </param>
  /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
  /// <return>A task that will perform the operation.</return>
  /// <remarks>
  /// This will access the <c>POST /1/feedback/recording-feedback</c> endpoint and requires <see cref="UserToken"/> to be set.<br/>
  /// Users can find their token on <a href="https://listenbrainz.org/profile/">their profile page</a>.<br/>
  /// </remarks>
  /// <exception cref="ArgumentException">
  /// When neither the <see cref="ISubmittedRecordingFeedback.Id"/> nor the <see cref="ISubmittedRecordingFeedback.MessyId"/>
  /// property is set on <paramref name="feedback"/>.
  /// </exception>
  /// <exception cref="HttpRequestException">When there was a problem sending the web service request.</exception>
  /// <exception cref="HttpError">When the web service sends a response indicating an error.</exception>
  public Task SubmitRecordingFeedbackAsync(ISubmittedRecordingFeedback feedback, CancellationToken cancellationToken = default) {
    if (feedback.Id is null && feedback.MessyId is null) {
      throw new ArgumentException("At least one recording ID must be specified (MBID, MSID, or both).", nameof(feedback));
    }
    return this.PostAsync("feedback/recording-feedback", feedback, null, cancellationToken);
  }

  #endregion

}
