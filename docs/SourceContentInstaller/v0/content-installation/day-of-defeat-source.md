# Day of Defeat: Source content installation

## Prerequisites

- [Day of Defeat: Source](../../../game-installation/game-installation/day-of-defeat-source.md)

## Installation

### Base content

1. Enable `dod` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "dod": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Day of Defeat: Source if you want since it's no longer needed

### Maps content

1. Enable `dod_maps` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "dod_maps": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
