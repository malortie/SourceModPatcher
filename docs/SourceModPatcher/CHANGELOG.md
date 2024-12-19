# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Mods
  - Deja Vudu Episode 1
  - Dissonance
  - Haunted
  - Son of Ithaca

### Fixed

- Mods
  - Haunted
    - Missing widescreen background materials
  - Son of Ithaca
    - Enable culling on player textures

## [0.3.0] - 2024-08-13

### Added

- Mods
  - A Frayed Stitch in Time
  - Combine Destiny
  - Dangerous World
  - Dangerous World 2 Demo
  - Day Hard
  - Final Project Diary
  - Half-Life 2: Riot Act
  - Half-Mind
  - Mistake Of Pythagoras
  - Obsidian Conflict 1.35
  - Rebellion
  - Rock 24
  - Slums
  - Slums 2
  - SMOD 4.0
  - SMOD Outbreak
  - SMOD Redux Version 8
  - SMOD: Tactical Delta 5.56
  - SMOD Troopers 0.9.9
  - Strider Mountain

### Changed

- Mods
  - Day Hard
    - Make *Fast weapon switch* visible in advanced Keyboard options
    - Make *Enable Developer Console* visible in advanced Keyboard options

  - Garry's mod 9
    - Add Counter-Strike: Source content support in gameinfo.txt
    - Add Day of Defeat: Source content support in gameinfo.txt
    - Add Episodic content support in gameinfo.txt
    - Add HL1 content support in gameinfo.txt
    - Add HL1MP content support in gameinfo.txt
    - Add HL2MP content support in gameinfo.txt
    - Add Lost Coast content support in gameinfo.txt

  - Obsidian Conflict 1.35
    - Add HL1MP content support
    - Add HL2MP content support
    - Add Portal content support

  - SMOD 4.0
    - Add Episodic content support in gameinfo.txt
    - Add HL1MP content support in gameinfo.txt

  - SMOD Redux Version 8
    - Add HL1MP content support in gameinfo.txt

  - SMOD: Tactical Delta 5.56
    - Add Episodic content support in gameinfo.txt
    - Add HL1MP content support in gameinfo.txt

  - SMOD Troopers 0.9.9
    - Add Episodic content support in gameinfo.txt
    - Add HL1MP content support in gameinfo.txt

### Fixed

- Mods
  - Combine Destiny
    - HUD message ammo 'full' not showing up

  - Day Hard
    - Remove Voice and Multiplayer tab in Options

  - Final Project Diary
    - Missing flashlight icon
    - Missing ammo icon

  - Half-Life 2: Riot Act
    - Closed captions not showing up
    - Missing topoffset in HudCloseCaption

  - Half-Mind
    - Remove Multiplayer tab in Options

  - Mistake Of Pythagoras
    - Remove HL2 backgrounds from ChapterBackgrounds.txt

  - Rebellion
    - Missing widescreen background materials

  - SMOD Outbreak
    - Localization strings
    - Missing Alyx gun model
    - Missing Hopwire model
    - Missing Hopwire materials

  - SMOD: Tactical Delta 5.56
    - Missing entry for HL2_Flamethrower

  - Strider Mountain
    - Missing widescreen background materials
    - Missing topoffset in HudCloseCaption

## [0.2.0] - 2024-07-21

### Added

- Steps
  - CopyDirectory

- Mods
  - Awakeking
  - Cite 14
  - City 7: Toronto Conflict
  - Das Roboss
  - Garry's mod 9
  - MACe the Adventure
  - Minverva: Metastasis 2
  - Minverva: Metastasis 3
  - Ravenholm - The Lost Chapter
  - School Adventures
  - Sebastian
  - Source 2004 (Mod HL2)
  - Source 2004 (Mod SDK)
  - Source 2006 (Mod Episodic)
  - Source 2006 (Mod HL2)
  - Source 2006 (Mod HL2MP)
  - Source 2006 (Mod SDK Sample)
  - Source 2007 (Mod EP2)
  - Source 2007 (Mod Episodic)
  - Source 2007 (Mod HL2)
  - Source 2007 (Mod HL2MP)
  - Source 2007 (Mod SDK Template)
  - The Lost City
  - The Island
  - Wivenhoe: The Fall of Ravenholm
  - Zombie Stress

### Changed

- Updated .NET project dependencies
  - SourceModPatcher.Tests
    - Bump Microsoft.NET.Test.Sdk from 17.6.0 to 17.10.0
    - Bump MSTest.TestAdapter from 3.0.4 to 3.4.3
    - Bump MSTest.TestFramework from 3.0.4 to 3.4.3
    - Bump coverlet.collector from 6.0.0 to 6.0.2
    - Bump System.IO.Abstractions.TestingHelpers from 19.2.87 to 21.0.22
- Removed binaries and SDK patch files (Now part of SourceContentInstaller)
- Gameinfo.txt now uses the new structure
- Cleaned mod data files that are now handled in SourceContentInstaller
- Reworked content dependency validation
  - Supports optional dependencies
  - Improved validation
  - Display names of missing dependencies in addition to missing variables

### Fixed

- Mods
  - City 7: Toronto Conflict
    - Map dundas_square_f
      - Fix APC crashing the game
    - Map yonge_st_f
      - Fix APC crashing the game

  - Das Roboss
    - Localization strings
    - Main menu title

  - The Island
    - Localization strings
    - Missing widescreen background materials

  - The Lost City
    - Localization strings
    - Missing widescreen background materials

  - MACe the Adventure
    - Missing widescreen background materials

  - Minverva: Metastasis 2
    - Missing widescreen background materials

  - School Adventures
    - Missing widescreen background materials

  - Wivenhoe: The Fall of Ravenholm
    - Localization strings
    - Missing widescreen background materials

  - Zombie Stress
    - Missing widescreen background materials

## [0.1.0] - 2024-05-31

### Added

- Mods
  - Eclipse
  - Get a Life
  - Halloween HL2: Pumpkin Night
  - Halloween HL2: Pumpkin Night (Redux)
  - Shantytown

### Fixed

- Black Snow
  - Missing widescreen background materials

- Eclipse
  - Missing widescreen background materials
  - Enable culling on player textures

- Get a Life
  - Missing widescreen background materials
  - Remove duplicate entry in credits
  - Localization strings in credits
  - Missing topoffset in HudCloseCaption
  - Missing closed captions in closecaption_russian.txt
  - Missing closed captions in closecaption_schinese.txt
  - Missing closed captions in closecaption_tchinese.txt
  - Remove unnecessary new lines in closed captions

- Halloween HL2: Pumpkin Night
  - Missing entry for Halloween_Chapter1_Title
  - Missing widescreen background materials
  - Localization strings on weapons names
  - Remove Voice and Multiplayer tab in Options
  - Map hood01_final
    - Remove buggy skateboard
    - Make non-interactible houses doors start locked

- Halloween HL2: Pumpkin Night (Redux)
  - Missing entry for Halloween HL2 PN_Chapter1_Title
  - Missing widescreen background materials
  - Localization strings on weapons names
  - Remove Voice and Multiplayer tab in Options
  - Map hoodrd_01a
    - Remove underscore in text `hint: Follow_toby`
  - Map hoodrd_01b
    - Remove underscore in text `hint: Follow_toby`
  - Map hoodrd_02a
    - Fix � symbols in texts `game_text_end01`, `game_text_end08`, `game_text_end09`
    - Make non-interactible houses doors start locked

## [0.0.0] - 2024-05-18

### Added

- Initial release
- Mods
  - 1187: Episode One
  - 1187: Rogue Train
  - Black Snow
  - Coastline to Atmosphere
  - Eye of the Storm
  - Heart of Evil: Source
  - Human Error EP1
  - Mission Improbable
  - Ravenholm
  - Research and Development

### Changed

- Coastline to Atmosphere
  - Make HUD visible in several Coastline to Atmosphere

### Fixed

- 1187: Episode One
  - Missing entry for HL2_Enable_Commentary
- 1187: Rogue Train
  - Missing entry for 1187_Chapter11_Title
  - Missing entry for HL2_Enable_Commentary
- Black Snow
  - HUDHistoryResource glitch
- Eye of the storm
  - HUDHistoryResource glitch
  - Shiny hands
- Heart of Evil: Source
  - Alien slave chrome
  - Missing entry for HL2_Enable_Commentary
  - Missing flashlight icon
- Ravenholm
  - Errors in Ravenholm22.bsp
  - Bonus decal not visible
  - Audio and Video schemes
- Mission improbable
  - Missing flashlight icon
  - HUDHistoryResource glitch
- Research and development
  - HUDHistoryResource glitch
- Source 2007 mods
  - Male and female gestures