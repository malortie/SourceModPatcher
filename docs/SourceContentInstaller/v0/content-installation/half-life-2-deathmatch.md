# Half-Life 2: Deathmatch content installation

## Prerequisites

- [Half-Life 2: Deathmatch](../../../game-installation/game-installation/half-life-2-deathmatch.md)

## Installation

### Base content

1. Enable `hl2mp` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl2mp": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life 2: Deathmatch if you want since it's no longer needed

### Maps content

1. Enable `hl2mp_maps` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl2mp_maps": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
