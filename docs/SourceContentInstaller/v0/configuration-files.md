# Configuration files

## steamapps.install.settings.json

Defines Source game content to install during the installation.

```json
{
  "<id>": {
    "install": <true/false>,
    "install_dir": "..."
  },
  ...
}
```

| Name | Type | Description |
| ---- | -----| ----------- |
| id | string | The Steam app ID |
| install | boolean | Whether to install this Steam app's content |
| install_dir | string | Content install location |

## steamapps.install.steps.json

Defines which install steps file to use for a specific Steam app content.

## steamapps.json

Defines properties for each supported Steam app. They have the form:

```json
{
  "<id>": {
    "name": "...",
    "appmanifest_file": "...",
    "install_dir": "..."
  },
  ...
}
```

| Name | Type | Description |
| ---- | -----| ----------- |
| id | string | The Steam app ID |
| name | string | The Steam app name |
| appmanifest_file | string | The app manifest file |
| install_dir | string | The full path to the Steam app's install location |

## variables.json

> **Note:** This file is autogenerated.

Contains the paths to installed Source games content locations.
