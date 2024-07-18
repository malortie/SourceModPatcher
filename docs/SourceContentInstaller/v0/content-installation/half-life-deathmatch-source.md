# Half-Life Deathmatch: Source content installation

## Prerequisites

- [Half-Life Deathmatch: Source](../../../game-installation/game-installation/half-life-deathmatch-source.md)

## Installation

### Base content

1. Enable `hl1mp` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl1mp": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life Deathmatch: Source if you want since it's no longer needed

### Maps content

1. Enable `hl1mp_maps` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl1mp_maps": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
