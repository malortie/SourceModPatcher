# Deja Vudu Episode 1 installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/SourceModPatcher-Mods-Registry/releases/download/smu-guildhall/deja_vudu_episode_1_setup.exe>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\Deja Vudu Episode 1
   ```

3. Enable `deja_vudu` in `sourcemods.install.settings.json`

   ```json
   "deja_vudu": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
