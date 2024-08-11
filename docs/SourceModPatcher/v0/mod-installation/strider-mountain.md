# Strider Mountain installation

> **Important!** If you encounter issues while playing, see [Bugs](#bugs).

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/strider-mountain/downloads/new-strider-mountain-version-3>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\Strider_Mountain
   ```

3. Enable `strider_mountain` in `sourcemods.install.settings.json`

   ```json
   "strider_mountain": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam

## Bugs

### Invisible HUD

If you load or reload from a saved game, the HUD may sometimes remain invisible. In this case, **exit and restart the game**.
