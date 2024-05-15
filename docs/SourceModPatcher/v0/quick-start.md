# Quick start

This will guide you through a basic use case of SourceModPatcher and SourceContentInstaller.

> **Note:** This guide assumes that you have a clean installation of SourceModPatcher and SourceContentInstaller.

## Table of Contents

- [Use case : Heart of Evil: Source](#use-case--heart-of-evil-source)
- [Conclusion](#conclusion)

## Use case : Heart of Evil: Source

### Table of Contents (Use case)

1. [Install Source SDK Base 2007](#1-install-source-sdk-base-2007)
2. [Download and install Heart of Evil: Source](#2-download-and-install-heart-of-evil-source)
3. [Configure SourceModPatcher to patch Heart of Evil: Source](#3-configure-sourcemodpatcher-to-patch-heart-of-evil-source)
4. [Launching SourceModPatcher](#4-launching-sourcemodpatcher)
   - [Diagnosing the error message](#diagnosing-the-error-message)
5. [Install Source games content](#5-install-source-games-content)
6. [Launching SourceModPatcher (again)](#6-launching-sourcemodpatcher-again)
7. [Restart Steam](#7-restart-steam)
8. [Launch Heart of Evil: Source](#8-launch-heart-of-evil-source)

### 1. Install Source SDK Base 2007

Install Source SDK Base 2007 in Steam :

1. Go to Library &gt; Tools
2. Install Source SDK Base 2007
3. Restart Steam
4. Launch Source SDK Base 2007

### 2. Download and install Heart of Evil: Source

Download the mod: <https://www.moddb.com/mods/heart-of-evil-source-port/downloads/heart-of-evil-source-update-101008>

Install the mod to your sourcemods folder :

```text
C:\Program Files (x86)\Steam\steamapps\sourcemods\hoe
```

### 3. Configure SourceModPatcher to patch Heart of Evil: Source

Enable Heart of Evil: Source in `sourcemods.install.settings.json`

```json
"hoe": {
  "install": true
}
```

See [Mod IDs](mod-ids.md) for more information.

### 4. Launching SourceModPatcher

Launch SourceModPatcher

```bash
.\SourceModPatcher.exe
```

You will be presented this menu. It is divided in 2 sections:

- Installed Source mods (All Source mods detected in `Steam/steamapps/sourcemods` directory)
- Content marked for installation (All Source mods marked for patching)

> **Note:** By default, SourceModPatcher automatically tries to locate `Steam/steamapps/sourcemods`. If you don't see any mods listed in the menu, set `sourcemods_path` to your sourcemods directory in `sourcemods.common.json` and relaunch SourceModPatcher with the option `--use-config-file`.

```text
Installed Source mods:
        ...
        [*] [hoe] Heart of Evil: Source
        ...
Content marked for installation:
        ...
        [*] [hoe] Heart of Evil: Source
        ...
The following content will be installed:
        [hoe] Heart of Evil: Source
Do you want to proceed [y/n] :
```

Press `y` to continue.

You should see the following message:

```text
One or more errors occured.
[WARNING] Step hoe_copy_files will not be executed: The following dependencies were not completed: <hoe_validate_dependencies>

[WARNING] Step hoe_replace_tokens will not be executed: The following dependencies were not completed: <hoe_copy_files>

[ERROR] Missing variable in variables.json : hl1_content_path [stage_0][hoe_validate_dependencies]

[ERROR] Missing variable in variables.json : sdkbase2013sp_hl2ep2_content_path [stage_0][hoe_validate_dependencies]

[ERROR] (1/3) Validate Heart of Evil: Source variables dependencies [FAILED]

[ERROR] (2/3) Copy files to hoe [CANCELLED]

[ERROR] (3/3) Replace tokens in hoe/gameinfo.txt [CANCELLED]

[ERROR] (1/1) Heart of Evil: Source [FAILED]
```

#### Diagnosing the error message

To begin with, Heart of Evil: Source depends on content from these games:

- Half-Life: Source
- Half-Life 2: Episode Two

For SourceModPatcher, these dependencies translate to:

- Half-Life: Source
- Source SDK Base 2013 Singleplayer (Half-Life 2: Episode Two)

The installation checks that you have the aforementionned content installed. In this case, it does so by checking if `hl1_content_path` and `sdkbase2013sp_hl2ep2_content_path` are present in `variables.json`. These [variables](../../SourceContentInstaller/v0/variables.md) are automatically added in the file when you install Source content using SourceContentInstaller. Since they don't exist, the installation fails and displays the above error message.

### 5. Install Source games content

Open `steamapps.install.settings.json` and enable Half-Life: Source and Source SDK Base 2013 Singleplayer by setting `280` and `243730` to `true`. See [Steam Application IDs](https://developer.valvesoftware.com/wiki/Steam_Application_IDs#Source_Engine_Games) for the list of Source games IDs.

`install_dir` is the location where SourceContentInstaller will install the content. By default it's `./content/hl1` but you can change it to a different value.

> **Note:** Setting `install` to `false` will **not** delete content that was previously installed.

```json
"280": {
  "install": true,
  "install_dir": "./content/hl1"
},
...
"243730": {
  "install": true,
  "install_dir": "./content/sdkbase2013sp"
}
```

Launch SourceContentIntaller

```bash
.\SourceContentIntaller.exe
```

You will be presented this menu. It is divided in 2 sections:

- Installed Source games (All installed Source games detected)
- Content marked for installation (All Source games to install content from)

> **Note:** By default, SourceContentInstaller automatically tries to locate Source games install location. If you don't see any games listed in the menu, set `install_dir` values in `steamapps.json` to each Source game install location and relaunch SourceContentInstaller with the option `--use-config-file`.

```text
Installed Source games:
        [*] [280] Half-Life: Source
        [*] [243730] Source SDK Base 2013 Singleplayer
Content marked for installation:
        [*] [280] Half-Life: Source
        [*] [243730] Source SDK Base 2013 Singleplayer
The following content will be installed:
        [280] Half-Life: Source
        [243730] Source SDK Base 2013 Singleplayer
Do you want to proceed [y/n] :
```

Press `y` to continue. The setup may take a few minutes.

If no errors occur you should see the following:

```text
=====================
Summary:
=====================
[Stages]
Completed: 2
Partially completed: 0
Failed: 0
Cancelled: 0

[Steps]
Completed: 10
Partially completed: 0
Failed: 0
Cancelled: 0
=====================
Installation finished.
All steps successfully completed.
```

In `variables.json`, you should have the following entries, which define the install location where Half-Life: Source and Source SDK Base 2013 Singleplayer content were installed.

```json
{
  "hl1_content_path": "<full path>/content/hl1",
  "hl1_hd_content_path": "<full path>/content/hl1/hl1_hd",
  "sdkbase2013sp_hl2_content_path": "<full path>/content/sdkbase2013sp/hl2",
  "sdkbase2013sp_hl2ep1_content_path": "<full path>/content/sdkbase2013sp/episodic",
  "sdkbase2013sp_hl2ep2_content_path": "<full path>/content/sdkbase2013sp/ep2"
}
```

### 6. Launching SourceModPatcher (again)

Now that we have `hl1_content_path` and `sdkbase2013sp_hl2ep2_content_path` defined, we can retry Heart of Evil: Source installation.

Launch SourceModPatcher

```bash
.\SourceModPatcher.exe
```

Press `y` to continue.

SourceModPatcher will apply fixes to your Heart of Evil: Source mod folder.

If no errors occur you should see the following:

```text
=====================
Summary:
=====================
[Stages]
Completed: 1
Partially completed: 0
Failed: 0
Cancelled: 0

[Steps]
Completed: 3
Partially completed: 0
Failed: 0
Cancelled: 0
=====================
Installation finished.
All steps successfully completed.
```

### 7. Restart Steam

### 8. Launch Heart of Evil: Source

1. Go to Library
2. Launch  Heart of Evil: Source

## Conclusion

Heart of Evil: Source should now be playable.

See [Mod installation](mod-installation.md) for a list of installation guides for each mod supported by SourceModPatcher.
