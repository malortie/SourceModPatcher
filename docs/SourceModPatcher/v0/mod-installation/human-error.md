# Human Error installation

## Prerequisites

- [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)
- [Source SDK Base 2013 Singleplayer content](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md)

## Installation

1. Download Human Error 1.0.2

   - <https://www.moddb.com/mods/half-life-2-short-stories/downloads/human-error-10>

2. Install 1.0.2 to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\Human_Error
   ```

3. Download Human Error Patch 1.0.3

   - <https://www.moddb.com/mods/half-life-2-short-stories/downloads/human-error-ep1-patch-103>

4. Install Patch 1.0.3  to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\Human_Error
   ```

5. Enable `human_error` in `sourcemods.install.settings.json`

   ```json
   "human_error": {
     "install": true
   }
   ```

6. Run SourceModPatcher
7. Restart Steam
