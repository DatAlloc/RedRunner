# DA Red Runner

This is a modified version of the original [:star: RedRunner](https://github.com/BayatGames/RedRunner) game that supports sending events to Node.

## Download

Clone this repository locally:

```bash
git clone git@github.com:DatAlloc/RedRunner.git
```

## Install Unity

This game is quite old and requires Unity 2018.3.5f1 which can be installed from here https://unity.com/releases/editor/archive.

## Build Package

Launch the Unity studio and follow these steps:

- From `File -> Open Scene`, select `RedRunner\Assets\Scenes\Play.unity`. 
- In the `File -> Build Settings`, select the target Platform.
- Build the package using `File -> Build`.

## Config

By default, the game tries to connect to a Noded instance running at 127.0.0.1:9123. This can be changed by specifying different values in the configuration file.

In Windows, this file is located here:
```bash
C:\Users\%USERNAME%\AppData\LocalLow\Bayat Games\Red Runner\noded.config
```

for other platforms see [Application Persistent Data Path](https://docs.unity3d.com/6000.1/Documentation/ScriptReference/Application-persistentDataPath.html)

The `noded.config` file format is INI. It supports only one `default` section and these two parameters:
```bash
[default]
ipaddr = 192.168.1.0
port = 12345
```

## License

MIT @ [Bayat Games](https://github.com/BayatGames)

Made with :heart: by [Bayat Games](https://github.com/BayatGames)
