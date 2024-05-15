# Error messages

Common warning and error messages that may occur while using SourceModPatcher.

## Missing dependencies

SourceModPatcher displays the following message:

```text
[WARNING] Step research_and_development_copy_files will not be executed: The following dependencies were not completed: <research_and_development_validate_dependencies>

[WARNING] Step research_and_development_replace_tokens will not be executed: The following dependencies were not completed: <research_and_development_copy_files>

[ERROR] Missing variable in variables.json : sdkbase2013sp_hl2ep2_content_path [stage_0][research_and_development_validate_dependencies]

[ERROR] (1/3) Validate Research and Development variables dependencies [FAILED]

[ERROR] (2/3) Copy files to Research and Development [CANCELLED]

[ERROR] (3/3) Replace tokens in Research and Development/gameinfo.txt [CANCELLED]

[ERROR] (1/1) Research and Development [FAILED]
```

In this case, the user tried to install [Research and Development](mod-installation/research-and-development.md) but didn't have [Source SDK Base 2013 Singleplayer content](../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md) installed. The variable `sdkbase2013sp_hl2ep2_content_path` is used to locate Source SDK Base 2013 Singleplayer content install location but wasn't found in `variables.json`. See [variables](../../SourceContentInstaller/v0/variables.md) for more information.

### Solution

1. Install the missing Source content. In this case, it's [Source SDK Base 2013 Singleplayer content](../../SourceContentInstaller/v0/content-installation/source-sdk-base-2013-singleplayer.md)
2. Relaunch SourceModPatcher
