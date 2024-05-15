# Source SDK Base 2013 Multiplayer content installation

## Prerequisites

- [Source SDK Base 2013 Multiplayer](../../../game-installation/game-installation/source-sdk-base-2013-multiplayer.md)

## Installation

1. Enable `243750` in `steamapps.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "243750": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Source SDK Base 2013 Multiplayer if you want since it's no longer needed
