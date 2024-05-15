# Configuration files

## sourcemods.common.json

Defines common variables.

## sourcemods.install.settings.json

Defines Source mods content to install during the installation.

```json
{
  "<id>": {
    "install": <true/false>
  },
  ...
}
```

| Name | Type | Description |
| ---- | -----| ----------- |
| id | string | The mod ID |
| install | boolean | Whether to install the mod |

## sourcemods.install.steps.json

Defines which install steps file to use for a specific Source mod.

## sourcemods.install.variables.json

Defines variables related to installation.

## sourcemods.json

Defines properties for each supported Source mods. They have the form:

```json
{
  "<id>": {
    "name": "...",
    "sourcemod_folder": "...",
    "data_dir": "..."
  },
  ...
}
```

| Name | Type | Description |
| ---- | -----| ----------- |
| id | string | The mod ID |
| name | string | The mod name |
| sourcemod_folder | string | The expected mod folder's name in `Steam/steamapps/sourcemods` directory |
| data_dir | string | The full or relative path to the directory that contains patch files for the mod |
