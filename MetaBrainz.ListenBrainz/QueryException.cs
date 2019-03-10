using System;
using System.Runtime.Serialization;

namespace MetaBrainz.ListenBrainz {

  /// <summary>An error reported by the ListenBrainz web service.</summary>
  [Serializable]
  public sealed class QueryException : Exception {

    /// <summary>The HTTP message code for the error.</summary>
    public readonly int Code;

    /// <summary>Creates a new <see cref="QueryException"/> instance.</summary>
    /// <param name="code">The HTTP message code for the error.</param>
    /// <param name="message">The message for the error.</param>
    /// <param name="cause">The exception that caused the error (if any).</param>
    public QueryException(int code, string message, Exception cause = null) : base(message, cause) {
      this.Code = code;
    }

    #region ISerializable

    /// <inheritdoc />
    public QueryException(SerializationInfo info, StreamingContext context) : base(info, context) {
      this.Code = info.GetInt32("query:code");
    }

    /// <inheritdoc />
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      base.GetObjectData(info, context);
      info.AddValue("query:code", this.Code);
    }

    #endregion

  }

}
