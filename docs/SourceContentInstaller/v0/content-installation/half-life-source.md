# Half-Life: Source content installation

## Prerequisites

- [Half-Life: Source](../../../game-installation/game-installation/half-life-source.md)

## Installation

### Base content

1. Enable `hl1` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl1": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life: Source if you want since it's no longer needed

### HD content

1. Enable `hl1_hd` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl1_hd": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life: Source if you want since it's no longer needed

### Maps content

1. Enable `hl1_maps` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl1_maps": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
