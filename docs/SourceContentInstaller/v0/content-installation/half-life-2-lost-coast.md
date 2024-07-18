# Half-Life 2: Lost Coast content installation

## Prerequisites

- [Half-Life 2: Lost Coast](../../../game-installation/game-installation/half-life-2-lost-coast.md)

## Installation

### Base content

1. Enable `lostcoast` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "lostcoast": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life 2: Lost Coast if you want since it's no longer needed

### Maps content

1. Enable `lostcoast_maps` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "lostcoast_maps": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
