# SMOD 4.0 installation

> **Important!** For campaign maps, see [Campaign maps](#campaign-maps).

> **Important!** For mounting additional content, see [Mounting additional content](#mounting-additional-content).

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Counter-Strike: Source (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/counter-strike-source.md#base-content)
  - [Counter-Strike: Source (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/counter-strike-source.md#maps-content)
  - [Day of Defeat: Source (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/day-of-defeat-source.md#base-content)
  - [Day of Defeat: Source (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/day-of-defeat-source.md#maps-content)
  - [Half-Life: Source (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-source.md#base-content)
  - [Half-Life: Source (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-source.md#maps-content)
  - [Half-Life 2: Deathmatch (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-2-deathmatch.md#maps-content)
  - [Half-Life 2: Loast Coast (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-2-lost-coast.md#maps-content)
  - [Source SDK Base 2013 Multiplayer (HL2:DM) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-multiplayer.md#hl2dm-content)
  - [Source 2006 (Episodic) (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#episodic-base-content)
  - [Source 2006 (Episodic) (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#episodic-maps-content)
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/SourceModPatcher-Mods-Registry/releases/download/downloads/smod40_full.7z>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\smod
   ```

3. Enable `smod` in `sourcemods.install.settings.json`

   ```json
   "smod": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam

## Campaign maps

The current HL2 maps are not compatible with the older engine (Source SDK Base 2006). To play the HL2 campaign in this mod, you'll need to find HL2 maps dated **2004**. Search online. Once you find a **2004** HL2 archive, extract all .ain, .bsp, .lmp files to the maps folder.

## Mounting additional content

Open `gameinfo.txt` and uncomment the section of each content you wish to mount.
