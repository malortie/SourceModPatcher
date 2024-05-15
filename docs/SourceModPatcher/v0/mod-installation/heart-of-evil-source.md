# Heart of Evil: Source installation

## Prerequisites

- [Half-Life: Source content](../../../SourceContentInstaller/v0/content-installation/half-life-source.md)
- [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)
- [Source SDK Base 2013 Singleplayer content](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/heart-of-evil-source-port/downloads/heart-of-evil-source-update-101008>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\hoe
   ```

3. Enable `hoe` in `sourcemods.install.settings.json`

   ```json
   "hoe": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
