# Error messages

Common warning and error messages that may occur while using SourceContentInstaller.

## VPK Extraction

SourceContentInstaller displays one or more of the following messages:

```text
[ERROR] File tree checksum mismatch (0A-00-12-73-EB-F3-1D-3B-6B-D7-DD-66-43-38-18-BF != expected 1F-00-00-00-20-00-00-00-7E-00-00-00-01-00-00-00) [stage_0][sdkbase2006_extract_content]

[ERROR] File tree checksum mismatch (8E-39-BA-D4-6C-B9-25-46-74-86-91-EF-FA-2B-D7-C1 != expected 45-00-00-00-45-00-00-00-01-00-00-00-B8-C6-B1-04) [stage_0][sdkbase2006_extract_content]

[ERROR] File tree checksum mismatch (49-D1-CD-FC-89-1F-0E-74-E4-8C-47-ED-F1-89-E3-9B != expected 22-09-09-22-37-31-35-38-37-32-37-30-33-34-33-38) [stage_0][sdkbase2006_extract_content]
```

```text
[ERROR] CRC32 mismatch for read data. [stage_0][hl2_extract_content]
```

The VPK's file integrity validation failed and the step was aborted.

### Solution

Validate the game or tool's integrity:

1. Go to Library
2. Right click on Game &gt; *Properties*
3. Go to *Installed Files*
4. Click *Verify integrity of game files*

Relaunch SourceContentInstaller.

If it doesn't work, try to use a different VPK extraction tool and extract the game/tool's VPK content to a separate folder and add a new variable in `variables.json` that points to that folder. The variable must be named after one of the supported [variables](variables.md).
