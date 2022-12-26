# ALAN-13 featurepack
## Summary
This project contains an abstracted part of the source of the game **ALAN-13 reformation**, which meant to help other Godot developers to kickstart their own isometric grid based games.
This is a Godot (mono version) project, using C#. The code was built for our own specific use case, so some of these solutions might not be suited or seem to be an overkill for your needs.
## Features
* Pannable, zoomable tile-based game-world
* Character movement implemented both for direct control (by the use directional keys) and for pathfinding by clicking on a destination cell
* Event driven state-machine for character movement 
* Use-case specific Manhattan A* path finding algorithm
* Steamwork integration 
* Achievement management for Steamwork
* Blender projects for dummy assests
* C# helper and extension classes for easier navigation bewteen Godot nodes
* Debugging tools 
* Input handling extensions
## Getting started
To use this feature pack, you need the mono version of the Godot game engine, which is downloadable from the official [Godot engine portal](https://godotengine.org/download).
I also recommend to use Microsoft Visual Studio or Visual Studio Code for writing and editing your code instead of the built-in IDE.
For steam integration the steam_api64.dll is needed which is distributed with the steamworks SDK and can be found [here] https://partner.steamgames.com/doc/sdk/api/example
Other relevant steamworks features are inclded from rlabrecque/Steamworks.NET library. 
## Support
To support the further development and extension of this free project you can buy the original game ALAN-13 on [steam]
(https://store.steampowered.com/app/1888130/ALAN13_Reformation/)
