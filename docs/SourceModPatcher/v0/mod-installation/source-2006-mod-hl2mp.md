# Source 2006 (Mod HL2MP) installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Half-Life 2: Deathmatch (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-2-deathmatch.md#maps-content)
  - [Source SDK Base 2013 Multiplayer (HL2:DM)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-multiplayer.md#hl2dm-content)
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <url>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\source_2006_mod_hl2mp
   ```

3. Enable `source_2006_mod_hl2mp` in `sourcemods.install.settings.json`

   ```json
   "source_2006_mod_hl2mp": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam