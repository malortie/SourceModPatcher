# Dangerous World 2 Demo installation

## Prerequisites

- Games
  - [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)

- Contents
  - [Source SDK Base 2013 Singleplayer (HL2:EP2)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md#hl2ep2-content)
  - [Source 2007 (Episodic) (base)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#episodic-base-content)
  - [Source 2007 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/dangerous-world-2/downloads/dangerousworld-2-demo>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\DangerousWorld 2 Demo
   ```

3. Enable `dangerous_world_2` in `sourcemods.install.settings.json`

   ```json
   "dangerous_world_2": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
