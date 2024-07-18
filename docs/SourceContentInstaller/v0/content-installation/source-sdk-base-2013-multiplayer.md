# Source SDK Base 2013 Multiplayer content installation

## Prerequisites

- [Source SDK Base 2013 Multiplayer](../../../game-installation/game-installation/source-sdk-base-2013-multiplayer.md)

## Installation

### HL2 content

1. Enable `sdkbase2013mp_hl2` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "sdkbase2013mp_hl2": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Source SDK Base 2013 Multiplayer if you want since it's no longer needed

### HL2:DM content

1. Enable `sdkbase2013mp_hl2mp` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "sdkbase2013mp_hl2mp": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Source SDK Base 2013 Multiplayer if you want since it's no longer needed
