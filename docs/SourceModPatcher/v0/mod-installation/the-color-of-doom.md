# The Color of Doom installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/SourceModPatcher-Mods-Registry/releases/download/smu-guildhall/ColorOfDoomSetup.exe>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\ColorOfDeath
   ```

3. Enable `the_color_of_doom` in `sourcemods.install.settings.json`

   ```json
   "the_color_of_doom": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
