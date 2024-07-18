# Source 2007 (Mod Episodic) installation

## Prerequisites

- Games
  - [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)

- Contents
  - [Source 2007 (Episodic) (base)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#episodic-base-content)
  - [Source 2007 (Episodic) (maps)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#episodic-maps-content)
  - [Source 2007 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2007.md#hl2-content)

## Installation

1. Download the mod

   - <url>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\source_2007_mod_episodic
   ```

3. Enable `source_2007_mod_episodic` in `sourcemods.install.settings.json`

   ```json
   "source_2007_mod_episodic": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
