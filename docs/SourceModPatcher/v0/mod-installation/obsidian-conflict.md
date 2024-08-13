# Obsidian Conflict 1.35 installation

> **Important!** This is a multiplayer mod. Game plugins and or other programs may be available for it. It's **recommended** to disable [Valve Anti-Cheat](https://developer.valvesoftware.com/wiki/Valve_Anti-Cheat) to prevent unintended false positive ban. See [Disable VAC](../disable-vac.md).

> **Important!** For mounting additional content, see [Mounting additional content](#mounting-additional-content).

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)
  - [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)

- Contents
  - [Counter-Strike: Source (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/counter-strike-source.md#base-content)
  - [Counter-Strike: Source (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/counter-strike-source.md#maps-content)
  - [Day of Defeat: Source (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/day-of-defeat-source.md#base-content)
  - [Day of Defeat: Source (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/day-of-defeat-source.md#maps-content)
  - [Half-Life: Source (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-source.md#base-content)
  - [Half-Life: Source (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-source.md#maps-content)
  - [Half-Life 2 (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-2.md#maps-content)
  - [Half-Life 2: Deathmatch (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-2-deathmatch.md#maps-content)
  - [Half-Life 2: Loast Coast (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-2-lost-coast.md#base-content)
  - [Half-Life 2: Loast Coast (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-2-lost-coast.md#maps-content)
  - [Half-Life Deathmatch: Source (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-deathmatch-source.md#base-content)
  - [Half-Life Deathmatch: Source (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/half-life-deathmatch-source.md#maps-content)
  - [Portal (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/portal.md#base-content)
  - [Portal (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/portal.md#maps-content)
  - [Source SDK Base 2013 Singleplayer (HL2:EP2) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md#hl2ep2-content)
  - [Source SDK Base 2013 Singleplayer (HL2:EP2) (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md#hl2ep2-maps-content)
  - [Source 2006 (Episodic) (maps) (bsps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#episodic-maps-bsps-content)
  - [Source 2007 (Episodic) (base) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#episodic-base-content)
  - [Source 2007 (Episodic) (maps) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#episodic-maps-content)
  - [Source 2007 (HL2) (**Optional**)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/SourceModPatcher-Mods-Registry/releases/download/downloads/obsidian_conflict_135_+_client_hotfix_without_mountfix.zip>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\obsidian
   ```

3. Enable `obsidian` in `sourcemods.install.settings.json`

   ```json
   "obsidian": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam

## Mounting additional content

1. Open `obsidian/sourcemodpatcher_scripts/setup.bat`
2. Uncomment each line of the content you want to mount by removing `rem` at the beginning of the line

    ```text
    rem call setup_cstrike.bat
    rem call setup_dod.bat
    rem call setup_ep2.bat
    rem call setup_episodic.bat
    rem call setup_hl1.bat
    rem call setup_hl1mp.bat
    rem call setup_hl2.bat
    rem call setup_hl2mp.bat
    rem call setup_lostcoast.bat
    rem call setup_portal.bat
    ```

3. Open a command prompt
4. Execute `setup.bat`
