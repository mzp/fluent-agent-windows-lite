# fluent-agent-windows-lite

* http://github.com/mzp/fluent-agent-windows-lite

## DESCRIPTION

'fluent-agent-windows-lite' is a log transfer agent, for Fluentd's 'forward' input.

This agent reads specified files, and sends each lines to fluentd servers. One log line will be packed one fluentd message, that has one attribute ('message' or specified in configuration) with entire line (not terminated by newline).

*NOTE: This aim to be Windows version of [fluent-agent-lite](https://github.com/tagomoris/fluent-agent-lite). But there is NO compatibility between them.*

### VERSION

0.1

## BUILD

Setup F# environment. See also: [MSDN Visual F#](http://msdn.microsoft.com/en-us/vstudio/hh388569.aspx).

    > git clone https://github.com/mzp/fluent-agent-windows-lite.git
    > cd fluent-agent-windows-lite
    > msbuild fluent-agent-windows-lite.sln /t:Build /p:Configuration=Release

## INSTALL

Copy `fluent-agent-windows-lite\bin\Release` to some directory(e.g. `C:/fluent-agent`).

### Service

If you register as a Windows service, you could start/stop via Service management console.

    > cd c:/path/to/fluent-agent
    > installutil fluent_agent_lite_for_windows.exe

And see "Services" from the Windows Control Panel → Administrative Tools or typing "Services.msc" in the Run command on Start menu.

### Standalone

You could run directory. This is useful for trouble shooting.

    > cd c:/path/to/fluent-agent
    > fluent_agent_lite_for_windows.exe /run

## Configuration

All of configurations are written in configuration json file (`fleunt-agent-windows.json`). It is read from same directory of applicaiton.

Following is example:

    {
      "host" : "192.168.11.14",
      "port" : 24224,
      "files" : [
         { "tag" : "debug.1", "path" : "c:/tmp/log", "pos" : "c:/tmp/pos" },
         { "tag" : "debug.2", "path" : "c:/tmp/log2" }
       ]
     }

### host

Host name of forwarding server.

### port

Port number of forwarding server. If this field is omited, 24224 is used.

### files

Files are watched.

#### tag

Tag name for this file.

#### path

Path for this file.

#### pos

Path for position stored file. If this field is omited, a file position is not stored.

*****

## License

Copyright 2012- MIZUNO Hiroki(mzp)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

