# Final Project Diary installation

> **Important!** If you encounter issues while playing, see [Bugs](#bugs).

## Prerequisites

- Games
  - [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)

- Contents
  - [Source SDK Base 2013 Singleplayer (HL2:EP2)](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md#hl2ep2-content)
  - [Source 2007 (Episodic) (base)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#episodic-base-content)
  - [Source 2007 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/final-project/downloads/final-project>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\final_project_diary
   ```

3. Enable `final_project_diary` in `sourcemods.install.settings.json`

   ```json
   "final_project_diary": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam

## Bugs

### Invisible HUD

During cinematics, the game will sometimes hide/unhide the HUD. **This is normal**. Other than that, the HUD is supposed to always be visible.

If you load or reload from a saved game, the HUD may sometimes remain invisible. In this case, **exit and restart the game**.
