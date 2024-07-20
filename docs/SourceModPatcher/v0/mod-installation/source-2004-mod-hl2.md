# Source 2004 (Mod HL2) installation

> **Important!** For campaign maps, see [Campaign maps](#campaign-maps).

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/source-sdk-2004/releases>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\source_2004_mod_hl2
   ```

3. Enable `source_2004_mod_hl2` in `sourcemods.install.settings.json`

   ```json
   "source_2004_mod_hl2": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam

## Campaign maps

The current HL2 maps are not compatible with the older engine (Source SDK Base 2006). To play the HL2 campaign in this mod, you'll need to find HL2 maps dated **2004**. Search online. Once you find a **2004** HL2 archive, extract all .ain, .bsp, .lmp files to the maps folder.
