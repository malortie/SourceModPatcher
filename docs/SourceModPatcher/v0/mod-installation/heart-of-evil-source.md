# Heart of Evil: Source installation

## Prerequisites

- Games
  - [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)

- Contents
  - [Half-Life: Source (base)](../../../SourceContentInstaller/v0/content-installation/half-life-source.md#base-content)
  - [Source SDK Base 2013 Singleplayer (HL2:EP2)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md#hl2ep2-content)
  - [Source 2007 (Episodic) (base)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#episodic-base-content)
  - [Source 2007 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/heart-of-evil-source-port/downloads/heart-of-evil-source-update-101008>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\hoe
   ```

3. Enable `hoe` in `sourcemods.install.settings.json`

   ```json
   "hoe": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
