# Counter-Strike: Source content installation

## Prerequisites

- [Counter-Strike: Source](../../../game-installation/game-installation/counter-strike-source.md)

## Installation

1. Enable `240` in `steamapps.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "240": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
3. (**Optional**) Uninstall Counter-Strike: Source if you want since it's no longer needed
