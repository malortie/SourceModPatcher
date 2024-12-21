# SourceForts v1.9.4 installation

> **Important!** This is a multiplayer mod. Game plugins and or other programs may be available for it. It's **recommended** to disable [Valve Anti-Cheat](https://developer.valvesoftware.com/wiki/Valve_Anti-Cheat) to prevent unintended false positive ban. See [Disable VAC](../disable-vac.md).

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source SDK Base 2013 Multiplayer (HL2:DM)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-multiplayer.md#hl2dm-content)
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/sourceforts/downloads/sourceforts-1-9-4-full>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\sourceforts
   ```

3. Enable `sourceforts` in `sourcemods.install.settings.json`

   ```json
   "sourceforts": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
