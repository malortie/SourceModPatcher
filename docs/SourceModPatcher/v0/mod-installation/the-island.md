# The Island installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/the-island/downloads/the-island>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\Half-Life 2 - The Island
   ```

3. Enable `the_island` in `sourcemods.install.settings.json`

   ```json
   "the_island": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
