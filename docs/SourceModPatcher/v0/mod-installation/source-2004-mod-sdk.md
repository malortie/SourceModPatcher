# Source 2004 (Mod SDK) installation

> **Important!** This is a multiplayer mod. Game plugins and or other programs may be available for it. It's **recommended** to disable [Valve Anti-Cheat](https://developer.valvesoftware.com/wiki/Valve_Anti-Cheat) to prevent unintended false positive ban. See [Disable VAC](../disable-vac#disable-vac).

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://github.com/malortie/source-sdk-2004/releases>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\source_2004_mod_sdk
   ```

3. Enable `source_2004_mod_sdk` in `sourcemods.install.settings.json`

   ```json
   "source_2004_mod_sdk": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
