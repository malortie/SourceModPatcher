# Source 2007 (Mod EP2) installation

## Prerequisites

- Games
  - [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)

- Contents
  - [Source SDK Base 2013 Singleplayer (HL2:EP2)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md#hl2ep2-content)
  - [Source SDK Base 2013 Singleplayer (HL2:EP2) (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md#hl2ep2-maps-content)
  - [Source 2007 (Episodic) (base)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#episodic-base-content)
  - [Source 2007 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/source-sdk-2007/releases>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\source_2007_mod_ep2
   ```

3. Enable `source_2007_mod_ep2` in `sourcemods.install.settings.json`

   ```json
   "source_2007_mod_ep2": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
