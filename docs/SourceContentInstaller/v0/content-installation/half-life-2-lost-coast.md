# Half-Life 2: Lost Coast content installation

## Prerequisites

- [Half-Life 2: Lost Coast](../../../game-installation/game-installation/half-life-2-lost-coast.md)

## Installation

1. Enable `340` in `steamapps.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "340": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life 2: Lost Coast if you want since it's no longer needed
