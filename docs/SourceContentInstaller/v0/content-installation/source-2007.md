# Source 2007 content installation

## Prerequisites

- [Source SDK Base 2007](../../../game-installation/game-installation/source-sdk-base-2007.md)

## Installation

### Episodic Base content

1. Enable `source2007_episodic` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "source2007_episodic": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller

### Episodic Maps content

1. Enable `source2007_episodic_maps` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "source2007_episodic_maps": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller

### HL2 content

1. Enable `source2007_hl2` in `contents.install.settings.json`

   > **Note:** You can change `install_dir` to a different directory if you want.

   ```json
   "source2007_hl2": {
     "install": true,
     "install_dir": "..."
   }
   ```

2. Run SourceContentInstaller
