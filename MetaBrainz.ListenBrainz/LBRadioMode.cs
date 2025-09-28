namespace MetaBrainz.ListenBrainz;

/// <summary>Modes that can be used for generating LB Radio playlists.</summary>
/// <seealso href="https://troi.readthedocs.io/en/latest/lb_radio.html#modes">Official Documentation</seealso>
public enum LBRadioMode {

  /// <summary>Easy mode picks recordings based on their own information.</summary>
  Easy,

  /// <summary>
  /// Medium mode tries a bit harder to find matches; this will result in more diverse (and potentially less relevant) results.
  /// </summary>
  Medium,

  /// <summary>Hard mode casts the widest net, finding even more diverse (and potentially less relevant) results.</summary>
  Hard,

}
