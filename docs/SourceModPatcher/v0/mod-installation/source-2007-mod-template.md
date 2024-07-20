# Source 2007 (Mod SDK Template) installation

## Prerequisites

- Games
  - [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)

- Contents
  - [Source 2007 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/source-sdk-2007/releases>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\source_2007_mod_template
   ```

3. Enable `source_2007_mod_template` in `sourcemods.install.settings.json`

   ```json
   "source_2007_mod_template": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
