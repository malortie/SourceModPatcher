# Day of Defeat: Source content installation

## Prerequisites

- [Day of Defeat: Source](../../../game-installation/game-installation/day-of-defeat-source.md)

## Installation

1. Enable `300` in `steamapps.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "300": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Day of Defeat: Source if you want since it's no longer needed
