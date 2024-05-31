# Updates

## Installing a new version over a previous installation

> **Important!** If you don't have SourceModPatcher installed, you don't need to follow this guide.

Every release contains default configuration files.

For now, you can install a new release on top of a previous installation but there's a couple of files you should backup to avoid losing previous configuration e.g. Content paths in `variables.json`.

Procedure:

1. Go to your current SourceModPatcher folder

2. Create a copy of the following files:
   - `variables.json` ->  `variables - Copy.json`

3. Copy the new release's files over your SourceModPatcher folder

4. Replace the new files with your saved files:
   - `variables - Copy.json` -> `variables.json`
