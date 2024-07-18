# Portal content installation

## Prerequisites

- [Portal](../../../game-installation/game-installation/portal.md)

## Installation

### Base content

1. Enable `portal` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "portal": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Portal if you want since it's no longer needed

### Maps content

1. Enable `portal_maps` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "portal_maps": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
