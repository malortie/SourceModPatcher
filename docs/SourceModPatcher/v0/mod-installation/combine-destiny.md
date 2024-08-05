# Combine Destiny installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download Combine Destiny 1.0

   - <https://www.moddb.com/mods/combine-destiny/downloads/combine-destiny-zip>

2. Install Combine Destiny 1.0 to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\CombineDestiny
   ```

3. Download Combine Destiny - Patch to 1.1

   - <https://www.moddb.com/mods/combine-destiny/downloads/combine-destiny-patch-to-11>

4. Install Combine Destiny - Patch to 1.1 to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\CombineDestiny
   ```

5. Enable `combine_destiny` in `sourcemods.install.settings.json`

   ```json
   "combine_destiny": {
     "install": true
   }
   ```

6. Run SourceModPatcher
7. Restart Steam
