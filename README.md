# MetaBrainz.ListenBrainz [![Build Status][CI-S]][CI-L] [![NuGet Version][NuGet-S]][NuGet-L]

This is a library providing access to the
[ListenBrainz API][api-reference].

[ListenBrainz][home] keeps track of users' listens of music tracks
(similar to sites like [last.fm][last-fm] and [libre.fm][libre-fm]).

[CI-S]: https://github.com/Zastai/MetaBrainz.ListenBrainz/actions/workflows/build.yml/badge.svg
[CI-L]: https://github.com/Zastai/MetaBrainz.ListenBrainz/actions/workflows/build.yml

[NuGet-S]: https://img.shields.io/nuget/v/MetaBrainz.ListenBrainz
[NuGet-L]: https://nuget.org/packages/MetaBrainz.ListenBrainz

[api-reference]: https://listenbrainz.readthedocs.io/en/latest/users/api
[home]: https://listenbrainz.org/

[last-fm]: https://www.last.fm
[libre-fm]: https://libre.fm

## Debugging

The `ListenBrainz` class provides a `TraceSource` that can be used to
configure debug output; its name is `MetaBrainz.ListenBrainz`.

### Configuration

#### In Code

In code, you can enable tracing like follows:

```cs
// Use the default switch, turning it on.
ListenBrainz.TraceSource.Switch.Level = SourceLevels.All;

// Alternatively, use your own switch so multiple things can be
// enabled/disabled at the same time.
var mySwitch = new TraceSwitch("MyAppDebugSwitch", "All");
ListenBrainz.TraceSource.Switch = mySwitch;

// By default, there is a single listener that writes trace events to
// the debug output (typically only seen in an IDE's debugger). You can
// add (and remove) listeners as desired.
var listener = new ConsoleTraceListener {
  Name = "MyAppConsole",
  TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ProcessId,
};
ListenBrainz.TraceSource.Listeners.Clear();
ListenBrainz.TraceSource.Listeners.Add(listener);
```

#### In Configuration

Your application can also be set up to read tracing configuration from
the application configuration file. To do so, the following needs to be
added to its startup code:

```cs
System.Diagnostics.TraceConfiguration.Register();
```

(Provided by the `System.Configuration.ConfigurationManager` package.)

The application config file can then have a `system.diagnostics` section
where sources, switches and listeners can be configured.

```xml
<configuration>
  <system.diagnostics>
    <sharedListeners>
      <add name="console" type="System.Diagnostics.ConsoleTraceListener" traceOutputOptions="DateTime,ProcessId" />
    </sharedListeners>
    <sources>
      <source name="MetaBrainz.ListenBrainz" switchName="MetaBrainz.ListenBrainz">
        <listeners>
          <add name="console" />
          <add name="lb-log" type="System.Diagnostics.TextWriterTraceListener" initializeData="lb.log" />
        </listeners>
      </source>
    </sources>
    <switches>
      <add name="MetaBrainz.ListenBrainz" value="All" />
    </switches>
  </system.diagnostics>
</configuration>
```

## Release Notes

These are available [on GitHub][release-notes].

[release-notes]: https://github.com/Zastai/MetaBrainz.ListenBrainz/releases
