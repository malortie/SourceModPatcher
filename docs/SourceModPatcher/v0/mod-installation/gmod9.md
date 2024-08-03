# Garry's mod 9 installation

> **Important!** This is a multiplayer mod. Game plugins and or other programs may be available for it. It's **recommended** to disable [Valve Anti-Cheat](https://developer.valvesoftware.com/wiki/Valve_Anti-Cheat) to prevent unintended false positive ban. See [Disable VAC](../disable-vac.md).

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
  - [Half-Life Deathmatch: Source (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-deathmatch-source.md#base-content)
  - [Half-Life Deathmatch: Source (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-deathmatch-source.md#maps-content)
  - [Source SDK Base 2013 Multiplayer (HL2:DM) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-multiplayer.md#hl2dm-content)
  - [Source 2006 (Episodic) (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#episodic-base-content)
  - [Source 2006 (Episodic) (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#episodic-maps-content)
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod (Choose any version)

   - [Gmod 9.0.4](https://www.moddb.com/mods/garrys-mod/downloads/gmod-904)
   - [Gmod 9.0.4b](https://github.com/malortie/SourceModPatcher-Mods-Registry/releases/download/downloads/gmod_9_0_4b.exe)

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\gmod9
   ```

3. Enable `gmod9` in `sourcemods.install.settings.json`

   ```json
   "gmod9": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam

## Campaign maps

The current HL2 maps are not compatible with the older engine (Source SDK Base 2006). To play the HL2 campaign in this mod, you'll need to find HL2 maps dated **2004**. Search online. Once you find a **2004** HL2 archive, extract all .ain, .bsp, .lmp files to the maps folder.

## Mounting additional content

Open `gameinfo.txt` and uncomment the section of each content you wish to mount.
