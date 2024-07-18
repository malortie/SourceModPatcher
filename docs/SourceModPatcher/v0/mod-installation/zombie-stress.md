# Zombie Stress installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/zombie-stress/downloads/zombie-stress-version-11>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\Zombie Stress
   ```

3. Enable `zombie_stress` in `sourcemods.install.settings.json`

   ```json
   "zombie_stress": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
