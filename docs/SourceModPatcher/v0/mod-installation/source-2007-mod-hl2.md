# Source 2007 (Mod HL2) installation

## Prerequisites

- Games
  - [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)

- Contents
  - [Half-Life 2 (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-2.md#maps-content)
  - [Source 2007 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/source-sdk-2007/releases>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\source_2007_mod_hl2
   ```

3. Enable `source_2007_mod_hl2` in `sourcemods.install.settings.json`

   ```json
   "source_2007_mod_hl2": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
