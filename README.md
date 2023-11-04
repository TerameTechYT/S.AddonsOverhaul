# S.AddonsOverhaul
**An overhaul of**
# Stationeers.Addons
**S.AddonsOverhaul** is a fully-fledged modding framework for [Stationeers](https://store.steampowered.com/app/544550/Stationeers/). Working just like the standard mods (XML) but with scripting and custom-content support!

## ***DO NOT ASK FOR HELP WITH THIS, I AM NOT CONTINUING THIS PROJECT FURTHER, I SIMPLY UPLOADED IT SO PEOPLE CAN USE IT FOR A REFERENCE WHATEVER***

## Download & Installation
If you have already installed the Addons, make sure to run file verification through Steam before installing a new version (go to Steam, click RMB on the game, open **Properties**, go to **Local Files** and click on **Verify integrity of game files...**)!
 
Go to [Releases](https://github.com/TerameTechYT/S.AddonsOverhaul/releases), select latest release and download zip file named 'S.AddonsOverhaul-vX.X-X.zip'. Now go to Steam, click RMB on the game, open **Properties**, go to **Local Files** and click on **BROWSE LOCAL FILES**. It should open new window for you. Next, you have to open the downloaded zip and drag all of its contents into the game folder (`AddonManager` folder and `version.dll`). And that's it! Enjoy your mods!

***Note:** After you've subscribed to an addon on the workshop, the game will restart for you, this is the best i can do for now. I might find a way to reload the xml data like the versions before the major refactor*

## Features
* Custom settings for features (Live reload, console, etc.)
* Config system (in-progress)
* Custom logging system
* External real-time log (Like BepInEx)
* Support for select stationeers.addons mods
* Reloads game when mod order changes
* Version check (Alert when your out-of-date)

**EVERYTHING CREATED BY ADDONS OVERHAUL IS IN THE STATIONEERS FOLDER IN `DOCUMENTS/MY GAMES`!**

## Links
* [Offical Discord](https://discord.gg/b6kFrUATdm)
* [Offical Trello](https://trello.com/b/zSHKh2XO/stationeersaddons)
* [Official Github](https://github.com/Erdroy/Stationeers.Addons)

## Building 
`Visual Studio` is required and `Visual Studio Tools for Unity` installation is recommended. 
* Open `Source/Stationeers.VS.props` file, and set the path to your game installation directory (has to end with a backslash). 
* Open `Source/S.AddonsOverhaul.sln` and start playing with it! See [Creating Addons](https://github.com/TerameTechYT/S.AddonsOverhaul#creating-addons) to find out more.
* Note: Open `Source/S.AddonsOverhaul.Native.sln` and copy `version.dll` to the game folder, as well as putting the other files in the other 2 release folders into `AddonManager` folder in the game directory to use your custom build.

## Creating addons
If you want to create your own addon, read here: [CREATING-ADDONS](Docs/CREATING-ADDONS.md).

## Debugging addons
If you want to debug your own addon, read here: [DEBUGGING-ADDONS](Docs/DEBUGGING-ADDONS.md).

## Addons
* [On Github](https://github.com/Erdroy/Stationeers.Addons#addons)

## Dependecies
* Mono.Cecil
* Harmony

## Contributions
We're accepting pull requests, look at our Trello board to find tasks that have not been completed, yet.
You can hop in, take some and help us evolve this modding framework!

Although, we want to keep mod consistency, we're suggesting to not release modified copy of this software on your own.
Anyway, this is certainly legal, if you would like to do that.

## License
MIT

___
***Stationeers.Addons** is not affiliated with RocketWerkz. All trademarks and registered trademarks are the property of their respective owners.*<br>
***Stationeers.Addons** (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors*