# Placid Plastic Duck Simulator - Mods 
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/KitsueFox/PPDS-Mods/dotnet.yml?style=for-the-badge)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/KitsueFox/PPDS-Mods?style=for-the-badge)
![MelonLoader Version](https://img.shields.io/badge/Melonloader-0.5.7-red?style=for-the-badge&)
![GitHub](https://img.shields.io/github/license/KitsueFox/PPDS-Mods?style=for-the-badge)

This repository contains my mods
for [Placid Plastic Duck Simulator](https://store.steampowered.com/app/1999360/Placid_Plastic_Duck_Simulator/). I'm not 
responsible for you save data being corrupted, Please backup before modding the game!

# **You need at least MelonLoader v0.5.7!**

## Current Mods
- - -
## Duck Trainer
**Achievements will be disabled by default to prevent cheating. If you wish to cheat anyways edit the `MelonPreferences.cfg`**

### Open GUI "F9"

* Spawn Ducks (K)
* Duck Movement (**Space** *to fly*)
* Duck All Quack
* Respawn All Ducks (H)
* Clear Weather
* Ducks Auto Respawn (*Never Loose a duck again*)
* Snowplow Controls

*Make sure to have one Movement Controls on at a time*

## Event Reenabler
Adds back the Ducks from the Christmas Event. Incase you missed it or accidentally lost your save file.

## Installation
To install these mods, you will need to install [MelonLoader](https://discord.gg/2Wn3N2P) (discord link, see \#how-to-install).
Then, you will have to put mod .dll files from [releases](https://github.com/KitsueFox/PPDS-Mods/releases) into the `Mods` folder of your game directory

## Building
To build these, drop required libraries (found in `<Placid Plastic Duck Simulator Instanll dir>\Placid Plastic Duck Simulator_Data\Managed` and both `MelonLoader.dll` `0Harmony.dll` from `<Placid Plastic Duck Simulator Instanll dir>\MelonLoader` after melonloader installation, 
list found in `Directory.Build.props`) into Libs folder, then use your IDE of choice to build.
* Libs folder is intended for newest libraries (MelonLoader 0.5.7)