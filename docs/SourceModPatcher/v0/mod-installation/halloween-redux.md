# Halloween HL2: Pumpkin Night (Redux) installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/halloween-2005/downloads/pumpkin-night-redux-1>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\Halloween HL2 PN
   ```

3. Enable `halloween_redux` in `sourcemods.install.settings.json`

   ```json
   "halloween_redux": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
