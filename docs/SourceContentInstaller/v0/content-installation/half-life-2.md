# Half-Life 2 content installation

## Prerequisites

- [Half-Life 2](../../../game-installation/game-installation/half-life-2.md)

## Installation

1. Enable `220` in `steamapps.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "220": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life 2 if you want since it's no longer needed
