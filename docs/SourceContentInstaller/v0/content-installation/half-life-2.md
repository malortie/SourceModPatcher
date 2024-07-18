# Half-Life 2 content installation

## Prerequisites

- [Half-Life 2](../../../game-installation/game-installation/half-life-2.md)

## Installation

### Base content

1. Enable `hl2` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl2": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life 2 if you want since it's no longer needed

### Maps content

1. Enable `hl2_maps` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl2_maps": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
