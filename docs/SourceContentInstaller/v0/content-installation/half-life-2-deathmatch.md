# Half-Life 2: Deathmatch content installation

## Prerequisites

- [Half-Life 2: Deathmatch](../../../game-installation/game-installation/half-life-2-deathmatch.md)

## Installation

1. Enable `320` in `steamapps.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "320": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life 2: Deathmatch if you want since it's no longer needed
