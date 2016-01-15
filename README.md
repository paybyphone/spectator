Spectator
=========

Overview																			
--------

Spectator is a Windows Service for taking measurements from Performance Counters or WMI and publishing them to a Statsd-compatible server.


Quick Start
-----------

Edit the `spectator.exe.config` file to set the Consul connection settings (or leave empty to use the local `spectator-config.json` file):

```
<appSettings>
   <add key="Spectator.ConsulHost" value="consul.server.com:8500" />
   <add key="Spectator.ConsulKey" value="metrics/spectator/config"/>
</appSettings>
```

Install the service via Topshelf (http://docs.topshelf-project.com/en/latest/overview/commandline.html):

```
spectator.exe install
```

Start the service via Topshelf:

```
spectator.exe start
```


Configuration Example
---------------------

```
{
  "statsdHost": "statsd.server.com",
  "statsdPort": 8125,
  "metricPrefix": "metrics.prefix.{machine}",
  "interval":  "00:00:15",
  "metrics": [
    {
	  /* Performance counter */
      "source": "performanceCounter",
      "path": "\\System\\Context Switches/sec",
      "type": "gauge",
      "template": "system.processor.context_switches_per_sec"
    },
    {
	  /* Performance counter reading a particular instance */
      "source": "performanceCounter",
      "path": "\\Paging File(_Total)\\% Usage",
      "type": "gauge",
      "template": "system.swap.percent_used"
    },
    {
	  /* Performance counter reading all instances */
      "source": "performanceCounter",
      "path": "\\Network Interface(*)\\Bytes Sent/sec",
      "type": "gauge",
      "template": "system.network.interface.{instance}.bytes_sent_per_sec",
      "exclude": "^(isatap|Teredo).*$"
    },
    {
	  /* Windows Management Instrumentation (WMI) */
      "source": "windowsManagementInstrumentation",
      "path": "\\Win32_ComputerSystem\\TotalPhysicalMemory",
      "type": "gauge",
      "template": "system.memory.total_physical"
    },
  ]
}
```

License
-------

spectator is made available as-is under the _Apache License, Version 2.0_. See the LICENSE file for full license details.