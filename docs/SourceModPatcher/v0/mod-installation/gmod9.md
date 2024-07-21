# Garry's mod 9 installation

> **Important!** This is a multiplayer mod. Game plugins and or other programs may be available for it. It's **recommended** to disable [Valve Anti-Cheat](https://developer.valvesoftware.com/wiki/Valve_Anti-Cheat) to prevent unintended false positive ban. See [Disable VAC](../disable-vac.md).

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
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
