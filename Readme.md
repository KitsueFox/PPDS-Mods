# Placid Plastic Duck Simulator - Mods 
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/KitsueFox/PPDS-Mods/dotnet.yml?style=for-the-badge)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/KitsueFox/PPDS-Mods?style=for-the-badge)
![MelonLoader Version](https://img.shields.io/badge/Melonloader-0.5.7-red?style=for-the-badge&)
![GitHub](https://img.shields.io/github/license/KitsueFox/PPDS-Mods?style=for-the-badge)

This repository contains my mods
for [Placid Plastic Duck Simulator](https://store.steampowered.com/app/1999360/Placid_Plastic_Duck_Simulator/). I'm not 
responsible for you save data being corrupted, Please backup before modding the game! Updates for the Mods will be slow due to work and do not request mods, I will not make them! You can Fork the project and make a pull request if you want.

# **You need at least MelonLoader v0.5.7!**

# Current Mods

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

## Custom Names
Instructions: 
* Copy a name to your clipboard
* Press Num0
* Profit!

This mod allows better customization for naming the ducks and even change the color and style of the name tag with [Rich Text](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html) as shown down below
![Placid_Plastic_Duck_Simulator_2T1JtRodId](https://user-images.githubusercontent.com/80623201/221664098-8278bb96-da26-464d-89a2-20a516d8ba23.jpg)

It's also allows the same duck to have a different name and longer names as shown in the video below
[![CustomName Demo](https://img.youtube.com/vi/kDhJ9IOkwtg/0.jpg)](https://www.youtube.com/watch?v=kDhJ9IOkwtg)

## Event Reenabler
Adds back the Ducks from the Christmas Event. Incase you missed it or accidentally lost your save file.

## Installation
To install these mods, you will need to install [MelonLoader](https://discord.gg/2Wn3N2P) (discord link, see \#how-to-install).
Then, you will have to put mod .dll files from [releases](https://github.com/KitsueFox/PPDS-Mods/releases) into the `Mods` folder of your game directory

## Building
To build these, drop required libraries (found in `<Placid Plastic Duck Simulator Instanll dir>\Placid Plastic Duck Simulator_Data\Managed` and both `MelonLoader.dll` `0Harmony.dll` from `<Placid Plastic Duck Simulator Instanll dir>\MelonLoader` after melonloader installation, 
list found in `Directory.Build.props`) into Libs folder, then use your IDE of choice to build.
* Libs folder is intended for newest libraries (MelonLoader 0.5.7)
