using JetBrains.Annotations;

namespace MetaBrainz.ListenBrainz {

  /// <summary>Enumeration of the supported request methods.</summary>
  [PublicAPI]
  internal enum Method {

    /// <summary>HTTP DELETE: Delete a resource.</summary>
    Delete,

    /// <summary>HTTP GET: Request data from a resource.</summary>
    Get,

    /// <summary>HTTP POST: Submit data to a resource.</summary>
    Post,

    /// <summary>HTTP PUT: Upload data representing a resource.</summary>
    Put,

  }

}
