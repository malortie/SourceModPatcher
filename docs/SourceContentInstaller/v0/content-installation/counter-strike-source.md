# Counter-Strike: Source content installation

## Prerequisites

- [Counter-Strike: Source](../../../game-installation/game-installation/counter-strike-source.md)

## Installation

### Base content

1. Enable `cstrike` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "cstrike": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Counter-Strike: Source if you want since it's no longer needed

### Maps content

1. Enable `cstrike_maps` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "cstrike_maps": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
