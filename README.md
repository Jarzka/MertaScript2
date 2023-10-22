# MertaScript - Counter-Strike Game Commentator

## Introduction

This application reads and analyses Counter-Strike log files in real-time and plays audio comments when something
interesting happens.

The application is based on the original MertaScript from 2014, which was written in Python but has now been
rewritten in C#.

### Architecture

The application consists of two parts:

- A server software, which reads the log file and sends the playable comment file path to the connected clients
- A client software, which plays the comment files locally

### Commentators

There are two types of commentators:

- A global game commentator, which comments global game events. This commentator always favors the team set in the
  config file.
- Every (BOT) player listed in the `players.json` file also have an individual commentator. This commentator comments
  only the
  events
  of the player.

### Supported games

- Counter-Strike 2
- Counter-Strike: Global Offensive

Counter-Strike: Source also partially works due to the similarity of the log files, but it is not officially
supported.

## Setup

- Rename `config_template.txt` to `config.txt` and edit it
- Rename `players_template.json` to `players.json` and edit it
- Build the application by running `dotnet build` (the default target location is `bin\Debug\net7.0`)
- Setup audio files:
    - In the application root directory, create a folder named `sound/mertaranta`
    - Move the `create_commentator_sound_folder_structure.ps1` script there and run it to create the folder
      structure
    - In the application root directory, create a folder named `sound/players/<player_name>`
    - Move the `create_player_sound_folder_structure.ps1` script there and run it to create the folder
      structure
    - Fill the folders with funny audio comments :D
- Run the application:
    - In server mode on the same machine where the game is running
    - In client mode on the machine where you want to hear the comments
        - In practice this means that every player needs to run the application in client mode on their own machine.
          However, it may be more practical to run the application in client mode on a single (virtual) machine and then
          use a
          virtual audio cable to route the audio to a voice chat application (e.g. Mumble / Discord).

## Configuration

### config.txt

| Name                            | Description                                                                   |
|---------------------------------|-------------------------------------------------------------------------------|
| start                           | host / join                                                                   |
| host_client_team_player_names   | List of player names who play in the team that the global commentator favors. |
| host_game_event_sounds_folder   | Folder name                                                                   |
| host_player_event_sounds_folder | Folder name                                                                   |
| host_round_time                 | Same as `mp_roundtime` setting used in the game                               |
| host_max_rounds                 | Same as `mp_maxrounds` setting used in the game                               |
| host_c4_time                    | Same as `mp_c4timer` setting used in the game                                 |
| host_hostage_taken_time_bonus   | A time bonus given when a hostage has been taken                              |
| host_logs_path                  | Path to the game's log files                                                  |
| host_port                       | Port to be used when hosting the application server                           |
| join_ip                         | IP to be used when joining the application server                             |
| join_port                       | Port to be used when joining the application server                           |

### players.json

This file defines the (BOT) player names who play in the game and whose actions should be commented individually. Alias
names are
also supported, so that a specific player can be recognized with a different name.

Good to know:

In **Counter-Strike 2**, BOT player names are hardcoded in the game, but their display name can be changed by
modifying the translation files included in the game. These files are client-based, i.e. every player needs to change
them locally (and probably re-change after every game update).

In practice, this means that when a BOT player is added in the game, their hardcoded name appears in the game log files,
but their display name (the name that a player sees in the game) can be something else.

In **Counter-Strike: Global Offensive**, the BOT player names seen in the log files and in the game should be same, so
alias names are not needed.