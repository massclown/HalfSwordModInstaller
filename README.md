# Half Sword Mod Installer

It helps to install the known mods for Half Sword demo v0.3 ([Steam release](https://store.steampowered.com/app/2397300/Half_Sword/)).

USE AT YOUR OWN RISK.

# How to use it

## Download the fresh installer here at https://github.com/massclown/HalfSwordModInstaller/releases/latest/download

## Simple mode
When the game is not running, start the installer, and press the big button and wait. Don't press the chair.

![Alt text](images/ui_basic.png?raw=true "Screenshot of installer in simple mode")

That is it!

## Advanced mode
Alternatively, if you have any problems or know what you are doing, open the advanced mode by selecting the corresponding tab.

Here you can download, install and enable individual mods, as well as uninstall or disable them, too.

![Alt text](images/ui_advanced.png?raw=true "Screenshot of installer in advanced mode")

The table on the screen shows all the mods that are known by the installer, and their status.

UE4SS is listed separately, it is not a mod, but it is a requirement for all other mods.

* "Download" will actually download the latest version of the mod (a ZIP file) from its URL.
* "Install/Uninstall" will either install the mod by placing its files in the correct place inside the game folder,
or uninstall it by removing its files from the game folder.
    * Attempting to install a mod without installing its dependencies (e.g. UE4SS) will result in an error message telling you to install UE4SS first.
* "Enable/Disable" will turn the mod or on off in the "mods.txt" file. For UE4SS, disabling is done by renaming the DLL files of UE4SS so that the game doesn't load them.
    * UE4SS is enabled automatically when installed. The other, actual mods are not.
* The "Uninstall all mods" button will do exactly what it says (including UE4SS)
* The other buttons allow you to view log files, and copy their full filenames (so you can then upload the logs somewhere)

## Supported mods
* Half Sword Trainer Mod -- https://github.com/massclown/HalfSwordTrainerMod
* Half Sword Split Screen Mod -- https://github.com/massclown/HalfSwordSplitScreenMod

## Notes

* The installer expects Half Sword demo installed from Steam. It won't work with other demos installed from somewhere else.

* The installer will attempt to repair known issues such as two UE4SS versions installed together, etc.

* In case of any issues, try clicking uninstall on UE4SS and then reinstall what you need again.

* The installer creates a folder in `%LOCALAPPDATA%\HalfSwordModInstaller\` (`C:\Users\YOUR_USERNAME\AppData\Local\HalfSwordModInstaller\`).

* The installer log file is in `installer.log` in that folder.

* The downloaded mod zip files are kept in `%LOCALAPPDATA%\HalfSwordModInstaller\downloads`
(`C:\Users\YOUR_USERNAME\AppData\Local\HalfSwordModInstaller\downloads`).

* Every mod is stored in a separate folder, every version is in a subfolder, for example:

```
C:\Users\YOUR_USERNAME\AppData\Local\HalfSwordModInstaller\
    ...
    installer.log
    downloads\
        UE4SS\
            v3.0.1\
                UE4SS_v3.0.1.zip
        HalfSwordTrainerMod\
            v0.8\
                HalfSwordTrainerMod_v0.8.zip
    ...

```


# License

Distributed under the MIT License. See `LICENSE` file for more information.

# Acknowledgments
* The stool icon and picture is "Stool" by artworkbean from [Noun Project](https://thenounproject.com/) (CC BY 3.0)