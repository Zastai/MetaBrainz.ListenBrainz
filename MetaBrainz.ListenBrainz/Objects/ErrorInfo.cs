using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class ErrorInfo : JsonBasedObject {

    public ErrorInfo(int code, string error) {
      this.Code = code;
      this.Error = error;
    }

    public int Code { get; set; }

    public string Error { get; set; }

    public static ErrorInfo? ExtractFrom(HttpWebResponse? response) {
      if (response == null || response.ContentLength == 0)
        return null;
      try {
        using var stream = response.GetResponseStream();
        if (stream == null || !stream.CanRead) {
          Debug.Print($"[{DateTime.UtcNow}] => EMPTY/UNREADABLE ERROR RESPONSE ({response.ContentType}): {response.ContentLength} byte(s)");
          return null;
        }
        if (!response.ContentType.StartsWith("application/json")) {
          Debug.Print($"[{DateTime.UtcNow}] => UNHANDLED ERROR RESPONSE ({response.ContentType}): {response.ContentLength} byte(s)");
          return null;
        }
        var characterSet = response.CharacterSet;
        if (string.IsNullOrWhiteSpace(characterSet))
          characterSet = "utf-8";
        var enc = Encoding.GetEncoding(characterSet);
        using var sr = new StreamReader(stream, enc, false, 1024, true);
        var json = sr.ReadToEnd();
        Debug.Print($"[{DateTime.UtcNow}] => ERROR RESPONSE ({response.ContentType}): \"{json}\"");
        var ei = JsonSerializer.Deserialize<ErrorInfo>(json);
        Debug.Print($"[{DateTime.UtcNow}] => ERROR {ei?.Code}: \"{ei?.Error}\"");
        return ei;
      }
      catch (Exception e) {
        Debug.Print($"[{DateTime.UtcNow}] => FAILED TO PROCESS ERROR RESPONSE: [{e.GetType()}] {e.Message}");
        return null;
      }
    }

    public static async Task<ErrorInfo?> ExtractFromAsync(HttpWebResponse? response) {
      if (response == null || response.ContentLength == 0)
        return null;
      try {
#if NETSTANDARD2_1 || NETCOREAPP3_1 // || NET5_0
        var stream = response.GetResponseStream();
        await using var _ = stream.ConfigureAwait(false);
#else
        using var stream = response.GetResponseStream();
#endif
        if (stream == null || !stream.CanRead)
          throw new WebException("No data received.", WebExceptionStatus.ReceiveFailure);
        if (!response.ContentType.StartsWith("application/json")) {
          Debug.Print($"[{DateTime.UtcNow}] => UNHANDLED ERROR RESPONSE ({response.ContentType}): {response.ContentLength} byte(s)");
          return null;
        }
        var characterSet = response.CharacterSet;
        if (string.IsNullOrWhiteSpace(characterSet))
          characterSet = "utf-8";
#if !DEBUG
        if (characterSet == "utf-8") // Directly use the stream
          return await JsonSerializer.DeserializeAsync<ErrorInfo>(stream);
#endif
        var enc = Encoding.GetEncoding(characterSet);
        using var sr = new StreamReader(stream, enc, false, 1024, true);
        var json = await sr.ReadToEndAsync().ConfigureAwait(false);
        Debug.Print($"[{DateTime.UtcNow}] => ERROR RESPONSE ({response.ContentType}): \"{json}\"");
        var ei = JsonSerializer.Deserialize<ErrorInfo>(json);
        Debug.Print($"[{DateTime.UtcNow}] => ERROR: ({ei?.Code}): \"{ei?.Error}\"");
        return ei;
      }
      catch (Exception e) {
        Debug.Print($"[{DateTime.UtcNow}] => FAILED TO PROCESS ERROR RESPONSE: [{e.GetType()}] {e.Message}");
        return null;
      }
    }

  }

}
