# Eclipse installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/eclipse/downloads/eclipse-102-updated>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\Eclipse
   ```

3. Enable `eclipse` in `sourcemods.install.settings.json`

   ```json
   "eclipse": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
