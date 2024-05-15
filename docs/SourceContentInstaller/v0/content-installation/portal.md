# Portal content installation

## Prerequisites

- [Portal](../../../game-installation/game-installation/portal.md)

## Installation

1. Enable `400` in `steamapps.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "400": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Portal if you want since it's no longer needed
