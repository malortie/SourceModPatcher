# Age of Chivalry v1.1 installation

> **Important!** This is a multiplayer mod. Game plugins and or other programs may be available for it. It's **recommended** to disable [Valve Anti-Cheat](https://developer.valvesoftware.com/wiki/Valve_Anti-Cheat) to prevent unintended false positive ban. See [Disable VAC](../disable-vac.md).

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/SourceModPatcher-Mods-Registry/releases/download/age-of-chivalery/aoc_1_1_Client.exe>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\ageofchivalry
   ```

3. Enable `age_of_chivalry` in `sourcemods.install.settings.json`

   ```json
   "age_of_chivalry": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
