<div align="center">
  <img src="https://pan.samyyc.dev/s/VYmMXE" />
  <h2><strong>MapChooser</strong></h2>
  <h3>A powerful and customizable map voting system for SwiftlyS2.</h3>
</div>

<p align="center">
  <img src="https://img.shields.io/badge/build-passing-brightgreen" alt="Build Status">
  <img src="https://img.shields.io/github/downloads/aga/MapChooser/total" alt="Downloads">
  <img src="https://img.shields.io/github/stars/aga/MapChooser?style=flat&logo=github" alt="Stars">
  <img src="https://img.shields.io/github/license/aga/MapChooser" alt="License">
</p>

## Overview

MapChooser is a full-featured map management plugin ported from the popular `cs2-rockthevote`. It includes Rock The Vote (RTV), map nomination, manual map voting, and automated end-of-map votes with support for both time-based and round-based triggers.

## Commands

| Command | Description |
| :--- | :--- |
| `!rtv` | Votes to trigger a map vote immediately (Rock The Vote). |
| `!nominate [map]` | Nominates a map to be included in the next map vote. |
| `!timeleft` | Shows the remaining time or rounds left on the current map. |
| `!nextmap` | Shows which map will be played next (if decided). |
| `!votemap [map]` | Directly votes for a specific map to change to. |
| `!revote` | Reopens the map vote menu if a vote is currently active. |
| `!setnextmap [map]` | Sets the next map directly or opens a selection menu (Admin only). |

## Configuration (`config.jsonc`)

### RTV Settings (`Rtv`)
| Setting | Default | Description |
| :--- | :--- | :--- |
| `Enabled` | `true` | Enable or disable the Rock The Vote system. |
| `EnabledInWarmup` | `true` | Allow players to RTV during warmup. |
| `NominationEnabled` | `true` | Allow players to nominate maps for the vote. |
| `MinPlayers` | `0` | Minimum number of players required to enable RTV. |
| `MinRounds` | `0` | Minimum rounds that must be played before RTV is allowed. |
| `ChangeMapImmediately` | `true` | Change the map immediately after a successful RTV vote (3s delay). |
| `MapsToShow` | `6` | Number of maps to display in the RTV vote menu. |
| `VoteDuration` | `30` | How long the vote menu remains open (seconds). |
| `VotePercentage` | `60` | Percentage of players required to trigger the vote. |

### Votemap Settings (`Votemap`)
| Setting | Default | Description |
| :--- | :--- | :--- |
| `Enabled` | `true` | Enable or disable the manual `!votemap` command. |
| `VotePercentage` | `60` | Percentage of players required to reach the vote threshold. |
| `ChangeMapImmediately` | `true` | Change map immediately once the threshold is reached. |
| `MinPlayers` | `0` | Minimum players required to use `!votemap`. |

### End Of Map Settings (`EndOfMap`)
| Setting | Default | Description |
| :--- | :--- | :--- |
| `Enabled` | `true` | Enable or disable the automated vote at the end of the map. |
| `MapsToShow` | `6` | Number of maps to show in the automated vote. |
| `VoteDuration` | `30` | Duration of the automated vote menu. |
| `TriggerSecondsBeforeEnd` | `120` | Seconds before timelimit to trigger the vote. |
| `TriggerRoundsBeforeEnd` | `2` | Rounds (or score difference from winning) before map end to trigger the vote. |

### Global Settings
| Setting | Default | Description |
| :--- | :--- | :--- |
| `MapsInCooldown` | `3` | Number of recently played maps to exclude from the next vote. |
| `Maps` | (List) | List of maps available. Use `ws:ID` for workshop maps. |

## Features

- **Live Vote Updates**: Menus refresh in real-time as players cast their votes.
- **Smart Triggers**: Automated votes trigger based on time remaining, rounds remaining, or when a team is close to winning.
- **Map Cooldown**: Prevents recently played maps from appearing in the vote too soon.
- **Workshop Support**: Seamlessly change to workshop maps using their IDs.
- **Localized**: Full translation support via JSONC files.

## Building

- Open the project in your preferred .NET IDE (e.g., VS Code, Visual Studio).
- Build the project. The output DLL and resources will be placed in the `build/` directory.