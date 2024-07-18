# MINERVA: Metastasis 3 installation

## Prerequisites

- Games
  - [Source SDK Base](../../../game-installation/game-installation/source-sdk-base.md)

- Contents
  - [Source 2006 (Episodic) (base)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#episodic-base-content)
  - [Source 2006 (HL2)](../../../SourceContentInstaller/v0/content-installation/source-2006.md#hl2-content)

## Installation

1. Download the mod

   - <https://www.moddb.com/mods/minerva/downloads/minerva-metastasis-full-patched>

2. Install the mod to your sourcemods folder i.e.

   ```text
   C:\Program Files (x86)\Steam\steamapps\sourcemods\metastasis
   ```

3. Enable `metastasis_3` in `sourcemods.install.settings.json`

   ```json
   "metastasis_3": {
     "install": true
   }
   ```

4. Run SourceModPatcher
5. Restart Steam
