# README

## About The Project

Since the Steampipe update from 2013, game content mounting in older Source engine branches doesn't work. As a result, Pre-Steampipe Source mods that depend on other Source games content will either fail to launch or have errors due to missing resources. Though it's possible to make them playable, this requires many manual fixes and other resources e.g. vanilla game DLLs compatible with the designed engine branch for mods that don't provide their own.

Two tools can be used to help automate the process:

- SourceContentInstaller
- SourceModPatcher

SourceContentInstaller is a tool used to automatically detect and extract content from Source games VPKs.

SourceModPatcher is a tool used to automatically detect specific Pre-Steampipe Source mods and apply fixes to their files. The fixes adjust gameinfo.txt but also other files where applicable such as maps, resources, scripts, etc. The tool also includes Pre-Steampipe compatible DLLs for mods without custom DLLs.

## Supported Platforms

- Windows

## Quick start

- [Quick start (Latest version)](docs/SourceModPatcher/v0/quick-start.md)

## Documentation

- [SourceContentInstaller](docs/SourceContentInstaller/README.md)
- [SourceModPatcher](docs/SourceModPatcher/README.md)

## Contributing

See [contribution guidelines](CONTRIBUTING.md).

## Acknowledgments

Thanks to the following resources for templates, guides and or general structural inspiration.

- [Best README Template](https://github.com/othneildrew/Best-README-Template)
- [Keep a Changelog](https://keepachangelog.com/)
- [ValveSoftware/halflife's code of conduct](https://github.com/ValveSoftware/halflife?tab=readme-ov-file#conduct)
