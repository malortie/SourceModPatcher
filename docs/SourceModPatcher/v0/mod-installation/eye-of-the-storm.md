# Eye of the storm installation

## Prerequisites

- [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)
- [Source SDK Base 2013 Singleplayer content](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/eye-of-the-storm/downloads/eye-of-the-storm-episode-i-full-10>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\eots
   ```

3. Enable `eots` in `sourcemods.install.settings.json`

   ```json
   "eots": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
