# Half-Life 2: Episode Two content installation

## Prerequisites

- [Half-Life 2: Episode Two](../../../game-installation/game-installation/half-life-2-episode-two.md)

## Installation

1. Enable `420` in `steamapps.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "420": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life 2: Episode Two if you want since it's no longer needed
