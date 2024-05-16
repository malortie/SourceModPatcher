# Research and Development installation

## Prerequisites

- [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)
- [Source SDK Base 2013 Singleplayer content](../../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/research-and-development/downloads/research-and-development1>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\Research and Development
   ```

3. Enable `research_and_development` in `sourcemods.install.settings.json`

   ```json
   "research_and_development": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam