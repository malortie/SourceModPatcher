# Source SDK Base content installation

## Prerequisites

- [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

## Installation

1. Enable `sdkbase2006` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "sdkbase2006": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Source SDK Base if you want since it's no longer needed
