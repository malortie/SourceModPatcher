# Half-Life 2: Episode One content installation

## Prerequisites

- [Half-Life 2: Episode One](../../../game-installation/game-installation/half-life-2-episode-one.md)

## Installation

1. Enable `hl2ep1` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "hl2ep1": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Half-Life 2: Episode One if you want since it's no longer needed
