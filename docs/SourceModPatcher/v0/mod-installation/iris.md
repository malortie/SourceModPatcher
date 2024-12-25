# IRIS installation

> **Important!** If you encounter issues while playing, see [Bugs](#bugs).

> **Important!** It's recommended to enable game console **before playing**. See [Cursor trapped at the center of the screen](#cursor-trapped-at-the-center-of-the-screen).

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/iris/downloads/iris-installer>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\iris
   ```

3. Enable `iris` in `sourcemods.install.settings.json`

   ```json
   "iris": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam

## Bugs

### Cursor trapped at the center of the screen

After taking control of the character, the mouse cursor may end up being trapped at the center of the screen, making it impossible to interact with entities or the main menu.

If you have the console enabled, type `quit` to exit the game. Otherwise, `Ctrl+Alt+Delete` and terminate `hl2.exe` process.
