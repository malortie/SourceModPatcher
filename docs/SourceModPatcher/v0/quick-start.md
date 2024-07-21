# Quick start

This will guide you through a basic use case of SourceModPatcher and SourceContentInstaller.

> **Note:** This guide assumes that you have a clean installation of SourceModPatcher and SourceContentInstaller.

## Table of Contents

- [Use case : Heart of Evil: Source](#use-case--heart-of-evil-source)
- [Conclusion](#conclusion)

## Use case : Heart of Evil: Source

### Table of Contents (Use case)

1. [Install Source SDK Base 2007](#1-install-source-sdk-base-2007)
2. [Install Source SDK Base 2013 Singleplayer](#2-install-source-sdk-base-2013-singleplayer)
3. [Install Half-Life: Source](#3-install-half-life-source)
4. [Download and install Heart of Evil: Source](#4-download-and-install-heart-of-evil-source)
5. [Configure SourceModPatcher to patch Heart of Evil: Source](#5-configure-sourcemodpatcher-to-patch-heart-of-evil-source)
6. [Launching SourceModPatcher](#6-launching-sourcemodpatcher)
   - [Diagnosing the error message](#diagnosing-the-error-message)
7. [Install Source games content](#7-install-source-games-content)
8. [Launching SourceModPatcher (again)](#8-launching-sourcemodpatcher-again)
9. [Restart Steam](#9-restart-steam)
10. [Launch Heart of Evil: Source](#10-launch-heart-of-evil-source)

### 1. Install Source SDK Base 2007

Install Source SDK Base 2007 in Steam :

1. Go to Library &gt; Tools
2. Install Source SDK Base 2007
3. Restart Steam
4. Launch Source SDK Base 2007

### 2. Install Source SDK Base 2013 Singleplayer

Install Source SDK Base 2013 Singleplayer in Steam :

1. Go to Library &gt; Tools
2. Install Source SDK Base 2013 Singleplayer
3. Right-click on Source SDK Base 2013 Singleplayer &gt; Properties
4. Go to Betas
5. Select *upcoming - upcoming*
6. Restart Steam
7. Launch Source SDK Base 2013 Singleplayer

### 3. Install Half-Life: Source

1. Go to Library
2. Install Half-Life: Source
3. Restart Steam
4. Launch Half-Life: Source

### 4. Download and install Heart of Evil: Source

Download the mod: <https://www.moddb.com/mods/heart-of-evil-source-port/downloads/heart-of-evil-source-update-101008>

Install the mod to your sourcemods folder :

```text
C:\Program Files (x86)\Steam\steamapps\sourcemods\hoe
```

### 5. Configure SourceModPatcher to patch Heart of Evil: Source

Enable Heart of Evil: Source in `sourcemods.install.settings.json`

```json
"hoe": {
  "install": true
}
```

See [Mod IDs](mod-ids.md) for more information.

### 6. Launching SourceModPatcher

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
[WARNING] Step hoe_copy_directory will not be executed: The following dependencies were not completed: <hoe_validate_dependencies>

[WARNING] Step hoe_replace_tokens will not be executed: The following dependencies were not completed: <hoe_copy_directory>

[ERROR] Missing variable(s) in variables.json : [hl1_content_path] [stage_0][hoe_validate_dependencies]

[ERROR] Install content: Half-Life: Source (base) [stage_0][hoe_validate_dependencies]

[ERROR] Missing variable(s) in variables.json : [sdkbase2013sp_hl2ep2_content_path,source2007_ep2_content_path] [stage_0][hoe_validate_dependencies]

[ERROR] Install content: Source SDK Base 2013 Singleplayer (HL2:EP2) [stage_0][hoe_validate_dependencies]

[ERROR] Missing variable(s) in variables.json : [source2007_episodic_content_path] [stage_0][hoe_validate_dependencies]

[ERROR] Install content: Source 2007 (Episodic) (base) [stage_0][hoe_validate_dependencies]

[ERROR] Missing variable(s) in variables.json : [source2007_hl2_content_path] [stage_0][hoe_validate_dependencies]

[ERROR] Install content: Source 2007 (HL2) [stage_0][hoe_validate_dependencies]

[ERROR] (1/3) Validate Heart of Evil: Source variables dependencies [FAILED]

[ERROR] (2/3) Copy directory to hoe [CANCELLED]

[ERROR] (3/3) Replace tokens in hoe/gameinfo.txt [CANCELLED]

[ERROR] (1/1) Heart of Evil: Source [FAILED]
```

#### Diagnosing the error message

> **Note:** Infos, warnings, errors are logged to `output.log`, `warning.log`, `error.log`.

To begin with, Heart of Evil: Source depends on content from these games:

- Half-Life: Source
- Half-Life 2: Episode Two

For SourceModPatcher, these dependencies translate to:

- Half-Life: Source (base)
- Source SDK Base 2013 Singleplayer (HL2:EP2)
- Source 2007 (Episodic) (base)
- Source 2007 (HL2)

The installation checks that you have the aforementionned content installed. In this case, it does so by checking if all required contents variables (e.g. `hl1_content_path`) are present in `variables.json`. These [variables](../../SourceContentInstaller/v0/variables.md) are automatically added in the file when you install Source content using SourceContentInstaller. Since they don't exist, the installation fails and displays the above error message.

### 7. Install Source games content

Open `contents.install.settings.json` and enable the following content by setting `"install": true` :

- hl1
- sdkbase2013sp_hl2ep2
- source2007_episodic
- source2007_hl2

`install_dir` is the location where SourceContentInstaller will install the content. By default it's `./content/*` but you can change it to a different value.

> **Note:** Setting `install` to `false` will **not** delete content that was previously installed.

```json
"hl1": {
  "install": true,
  "install_dir": "./content/hl1"
},
...
"sdkbase2013sp_hl2ep2": {
  "install": true,
  "install_dir": "./content/sdkbase2013sp/ep2"
}
...
"source2007_episodic": {
  "install": true,
  "install_dir": "./content/source2007"
}
...
"source2007_hl2": {
  "install": true,
  "install_dir": "./content/source2007"
}
```

Launch SourceContentIntaller

```bash
.\SourceContentIntaller.exe
```

You will be presented this menu. It is divided in 3 sections:

- Installed Source games (All installed Source games detected)
- Content currently installed (What you previously installed using SourceContentInstaller)
- Content marked for installation (What you want to install)

> **Note:** By default, SourceContentInstaller automatically tries to locate Source games install location. If you don't see any games listed in the menu, set `install_dir` values in `steamapps.json` to each Source game install location and relaunch SourceContentInstaller with the option `--use-config-file`.

```text
Installed Source games:
        ...
        [*] [218] Source SDK Base 2007
        [*] [280] Half-Life: Source
        [*] [243730] Source SDK Base 2013 Singleplayer
        ...
Content installed:
        ...
        [ ] [hl1] Half-Life: Source (base)
        [ ] [sdkbase2013sp_hl2ep2] Source SDK Base 2013 Singleplayer (HL2:EP2)
        [ ] [source2007_episodic] Source 2007 (Episodic) (base)
        [ ] [source2007_hl2] Source 2007 (HL2)
        ...
Content marked for installation:
        ...
        [*] [hl1] Half-Life: Source (base)
        [*] [sdkbase2013sp_hl2ep2] Source SDK Base 2013 Singleplayer (HL2:EP2)
        [*] [source2007_episodic] Source 2007 (Episodic) (base)
        [*] [source2007_hl2] Source 2007 (HL2)
        ...
The following content will be installed:
        [hl1] Half-Life: Source (base)
        [sdkbase2013sp_hl2ep2] Source SDK Base 2013 Singleplayer (HL2:EP2)
        [source2007_episodic] Source 2007 (Episodic) (base)
        [source2007_hl2] Source 2007 (HL2)
Do you want to proceed [y/n] :
```

Press `y` to continue. The setup may take a few minutes.

If no errors occur you should see the following:

```text
=====================
Summary:
=====================
[Stages]
Completed: 4
Partially completed: 0
Failed: 0
Cancelled: 0

[Steps]
Completed: 9
Partially completed: 0
Failed: 0
Cancelled: 0
=====================
Installation finished.
All steps successfully completed.
```

In `variables.json`, you should find the following entries, which define the install location where the contents were installed.

```json
{
  "hl1_content_path": "<full path>/content/hl1",
  "sdkbase2013sp_hl2ep2_content_path": "<full path>/content/sdkbase2013sp/ep2",
  "source2007_ep2_content_path": "<full path>/content/sdkbase2013sp/ep2",
  "source2007_episodic_content_path": "<full path>/content/source2007/episodic",
  "source2007_hl2_content_path": "<full path>/content/source2007/hl2"
}
```

### 8. Launching SourceModPatcher (again)

Now that we have the variables defined, we can retry Heart of Evil: Source installation.

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

### 9. Restart Steam

### 10. Launch Heart of Evil: Source

1. Go to Library
2. Launch  Heart of Evil: Source

## Conclusion

Heart of Evil: Source should now be playable.

See [Mod installation](mod-installation.md) for a list of installation guides for each mod supported by SourceModPatcher.
