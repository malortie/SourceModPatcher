# The Extraction Point installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/SourceModPatcher-Mods-Registry/releases/download/the-extraction-point/EXP_cleaned.zip>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\EXP
   ```

3. Enable `exp` in `sourcemods.install.settings.json`

   ```json
   "exp": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
