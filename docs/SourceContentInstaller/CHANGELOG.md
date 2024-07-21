# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Content
  - Counter-Strike: Source (maps)
  - Day of Defeat: Source (maps)
  - Half-Life: Source (maps)
  - Half-Life: Source (HD content)
  - Half-Life Deathmatch: Source (maps)
  - Half-Life 2 (maps)
  - Half-Life 2: Deathmatch (maps)
  - Half-Life 2: Lost Coast (maps)
  - Portal (maps)
  - Source SDK Base 2013 Singleplayer (HL2:EP2) (maps)
  - Source 2006 Episodic
  - Source 2006 Episodic (maps)
  - Source 2006 HL2
  - Source 2007 Episodic
  - Source 2007 Episodic (maps)
  - Source 2007 HL2

### Changed

- Updated .NET project dependencies
  - SourceContentInstaller
    - Bump AutoMapper from 12.0.1 to 13.0.1
    - Bump NLog from 5.2.7 to 5.3.2
    - Bump Pastel from 4.2.0 to 5.1.0
    - Bump System.IO.Abstractions from 19.2.87 to 21.0.22
    - Bump ValvePak from 1.6.2.76 to 2.0.1.107
  - SourceContentInstaller.Tests
    - Bump Microsoft.NET.Test.Sdk from 17.6.0 to 17.10.0
    - Bump MSTest.TestAdapter from 3.0.4 to 3.4.3
    - Bump MSTest.TestFramework from 3.0.4 to 3.4.3
    - Bump coverlet.collector from 6.0.0 to 6.0.2
    - Bump System.IO.Abstractions.TestingHelpers from 19.2.87 to 21.0.22
- Reworked content management
  - More modular instead of extracting all content from a specific SteamApp
  - Supports checking for multiple Steam apps before installing content
  - Now uses binaries from SDK repos:
    - source-sdk-2004 (v1.0.0)
    - source-sdk-2006 (v1.0.0)
    - source-sdk-2007 (v1.0.0)
  - Renamed `steamapps.install.settings.json` to `contents.install.settings.json`
  - Renamed `steamapps.install.steps.json` to `contents.install.steps.json`
  - Various changes in extract step
  - Display installed content

- Disable CRC verification during extraction
- Content
  - Counter-Strike: Source now extracts base content only
  - Day of Defeat: Source now extracts base content only
  - Half-Life: Source now extracts base content only
  - Half-Life Deathmath: Source now extracts base content only
  - Half-Life 2 now extracts base content only
  - Half-Life 2: Deathmatch now extracts base content only
  - Half-Life 2: Lost Coast now extracts base content only
  - Portal now extracts base content only
  - Source SDK Base 2013 Multiplayer is now separated (HL2/HL2MP)
  - Source SDK Base 2013 Singleplayer is now separated (HL2/EP1/EP2)
- Creates variables.json if it does not exist
- On app startup, inexistant paths are removed from variables.json

### Fixed

- VPKExtractor
  - Now compares the expected number of files to be extracted and the number of files extracted to determine if an error occured. Previously, it would assume that no files extracted means that an error occured but the case when regex can't find find files was not handled.

## [0.0.0] - 2024-05-18

### Added

- Initial release
- Content
  - Counter-Strike: Source
  - Day of Defeat: Source
  - Half-Life 2
  - Half-Life 2: Deathmatch
  - Half-Life 2: Episode One
  - Half-Life 2: Episode Two
  - Half-Life 2: Lost Coast
  - Half-Life Deathmatch: Source
  - Half-Life: Source
  - Portal
  - Source SDK Base
  - Source SDK Base 2007
  - Source SDK Base 2013 Multiplayer
  - Source SDK Base 2013 Singleplayer
