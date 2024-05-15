# Half-Life: Source content installation

## Prerequisites

- [Half-Life: Source](../../../game-installation/game-installation/half-life-source.md)

## Installation

1. Enable `280` in `steamapps.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "280": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life: Source if you want since it's no longer needed
