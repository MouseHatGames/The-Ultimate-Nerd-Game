A game from Mouse Hat Games
Programming & Game Design by Jimmy Cushnie
Sound Design by Emmanuel Lagumbay
Title art by Daniel John
Additional graphic design by Ben Longley
Quality Assurance: Ben Longley & Daniel John


Thank you so much for playing our game! I hope you enjoy it :)
Remember you can press F2 for a list of controls!

Get emails about updates to this game:
Join the community on reddit:
Chat about TUNG on discord:
Connect with us on Twitter:

If you'd like to contact me, please send me an email: jimmycushnie@gmail.com

======================================

Music:

"Vasquez Rock", "Antelope Valley", "Zion National Park"
Lauren X. Pham (soundcloud.com/laurenxpham)
Used with permission

"Snowflake", "Passion", "Gratitude", "Pleasure", "Calm", "Floating", "Light", "Plants", "Sun", "Moon", "Changing", "71017"
Borrtex (borrtex.com)
Used with permission

"Bittersweet", "Dreamer", "Immersed", "Inspired", "Pamgaea", "Silver Flame", "Wallpaper"
Kevin MacLeod (incompetech.com)
Licensed under Creative Commons: By Attribution 3.0
http://creativecommons.org/licenses/by/3.0/

"Brooks"
Kai Engel (kai-engel.com)
Licensed under Creative Commons: By Attribution 4.0
https://creativecommons.org/licenses/by/4.0/

"Divider", "What Does Anybody Know About Anything", "The Temperature of the Air on the Bow of the Kaleetan", "Readers! Do You Read?", "Out of the Skies, Under the Earth", "CGI Snake", "Cylinder Three", "Cylinder Six""
Chris Zabriskie (chriszabriskie.com)
Licensed under Creative Commons: By Attribution 4.0
https://creativecommons.org/licenses/by/4.0/

======================================

Third party assets used:

* Wispy Skybox by Mundus Limited
* Outline Effect by cakeslice
* UGUI Kit: Flat by Lovatto Studios
* Easy Save 3 by Moodkie Interactive
* AQUAS by Dogmatic Games
* Thread Ninja by Ciela Spike

======================================

Changelog:

v0.2.4 | 2018-03-28

* added a hotkey to undo deleting a board. Default binding is backspace
* backups of boards you delete are now saved in /backups/_____deletedboards. By default, the 10 most recently deleted boards are saved, but you can increase that number with MaxBoardBackups in settings.txt
* the crosshair's color is now configurable using CrosshairColor in settings.txt
* the crosshair's size is now configurable using CrosshairSize in settings.txt
* added a button to disable player collision with circuitry and boards, so you can walk/fly through them and reach otherwise inaccessable positions. By default there is no key bound to this function, but you can assign it in the launcher (I recommend the ` key)
* added some hooks in the code to allow placing and saving of custom components added by mods
* in the The Ultimate Nerd Game directory there is now a file called TimePlayed.txt which keeps track of the total time you have spent in-game
* added lines about how to select the root board and how to delete a non-empty board to the help menu
* (hopefully) fixed a bug where everything would completely break when you loaded a world
* the readme.txt file downloaded with the game now includes instructions for installation



v0.2.3 | 2018-03-25

* fixed anything that changes color not being affected by shadows for a brief period after it changes color
* added some keyboard navigation to the Load Board menu: you can now press tab to cycle forwards and shift + tab to cycle backwards
* the Load, Rename, and Delete buttons in the Load Board menu are no longer interactable until you select a board
* made the first chord of a particularly startling music track a little quieter
* the Toggle Gameplay UI key no longer hides the help menu. This is to prevent a bug that could screw up the size of the help menu text
* you can now toggle the gameplay UI at any time
* fixed the rotation lock text sometimes not properly updating
* fixed snapping pegs appearing in the wrong position under specific circumstances
* fixed placing a board under specific circumstances causing lots of crazy glitches
* the game is now available on Linux! Sorry it took so long :)



v0.2.2 | 2018-03-23

* button and lever hitboxes are slightly bigger; you no longer need to look exactly at them to interact with them
* player movement is much smoother; the player's position is now updated every frame instead of at a fixed rate of 50 times per second
* the fullscreen checkboxes in the options menu and the game launcher are now synchronized
* added a hotkey to toggle fullscreen, default binding F11
* you can no longer create a new board if you're not looking at a valid location to start placing one
* rotation lock no longer resets to off when you place a board
* further reduced visual errors on displays and color displays
* fixed sometimes placing a component when flipping a switch
* fixed snapping pegs sometimes snapping when they shouldn't



v0.2.1 | 2018-03-20 (but later!)

* pressing the non-numpad enter in the New Board menu now places the board you're creating
* whether or not to show Component ghosts is no longer persistent and will be reset to true whenever a save is loaded
* reduced visual errors on displays and color displays
* fixed Stack Board iterations not being properly reset to 1
* fixed the component you're placing freaking out if you rotated a Mount in a specific way
* fixed an issue where certain saves would lock up the game
* fixed sometimes not being able to delete snapping pegs
* fixed the About menu being empty if accessed from the main menu
* fixed typo in the About menu



v0.2 | 2018-03-20
1500 downloads!! Holy cow!!! Thank you SO MUCH to everyone who's been playing the game and talking about it and building cool stuff in it. This update is dedicated to you <3

COMPONENTS
* added Color Displays, and of course a panel variant - these components have three pegs, each of which corresponds to a color, for a total of 7 colors they can switch between while on
* added Noisemakers - click on them to select the tone they'll play when powered
* newly placed noisemakers will set their tone to the most recently set tone
* by default, noisemakers stop the game music when you play them. This behavior can be disabled with "AllowNoisemakersToInterruptMusic" in settings.txt
* added Snapping Pegs, to help you build modularly - a snapping peg will automatically create a connection with another snapping peg placed adjacently to it
* added Mounts, to help fit circuitry into tight spaces
* buttons no longer have a fixed time that they stay on for; they will stay down as long as you hold your real life button down on them (minimum 2 ticks)
* you can change the color of non-color displays by clicking on them. There are seven colors to choose from, same as are available with multi-color displays.
* when you place a new display, it will be the color of the most recently chosen color in the display color chooser menu
* when you place a new label, it will have the most recently chosen font size in the edit label text menu
* off displays are darker to have more contrast with white displays

BUILDING
* while placing components, there is now a ghost of where you'll place the component
* added key to toggle component placing ghost, default binding is caps lock
* added "Pick Component" key; press it while looking a component to switch the selection menu to that component. Default binding is middle mouse or I
* added "Flip Component" key; press it while looking at a component that goes through a board to make it go through in the other direction. Default binding is F
* "AllowFlippingOneSidedComponents" in settings.txt can be enabled to allow flipping components that do not go through the board
* added Stack Board to the board menu, an easy way of tiling your designs
* added Save Board and Load Board to the board menu. Saved boards are persistent across saves and can be shared with other players
* you can now mirror boards along either axis, default bindings N and M. Only works with single boards, you cannot mirror boards with other boards attached
* holding the mod key (default ctrl) will make things you're placing flat even when they are on non-flat terrain
* when looking around, pegs you can connect to will be highlighted in blue. This initial highlight can be toggled with mod key + toggle placing ghost key
* when in the middle of connecting, looking at a peg will highlight it
* when in the middle of connecting, the wire you will create will have a ghost
* the component placing ghost (if enabled) will automatically be hidden when you start placing a wire and shown again when you finish. This behavior can be toggled with "AutoHidePlacingGhostWhileConnecting" in settings.txt
* new connection mode: Chained. Behaves like Multi-Phase but automatically selects the final peg after one connection as the initial peg of the next connection.
* added a hotkey to toggle between connection modes, default binding F8
* when creating a wire between a peg and a blot, the peg will now always be used to determine the default rotation of the wire
* you can now connect to pegs hidden behind wires
* when placing a board at a right angle to another board, offset can now be used to place it against or between the grid instead of just in the center of it
* all placing ghosts are green if the placement is valid, red if it is invalid
* new settings.txt option: "PlacementValidityRecalculationInterval" (default 4). Whether the outline of the thing you're placing should be red or green is recalculated every this number of frames
* new settings.txt option: "MaxComplexityToCalculateOutlinesFor" (default 1000). If you are placing a board with more than this number of cubes, it will only be partially highlighted in blue instead of fully highlighted in red or green. This is to save performance
* the distance the player can reach is now configurable in settings.txt under the field "ReachDistance"
* you can no longer place stuff where it would break wires when it is placed
* you can no longer rotate a placed component into a position where it would intersect something
* you can no longer place stuff more than 30cm below the floor of the world
* board placing offset is no longer reset when you place the board

MOVEMENT
* added flying! Press B (default) to toggle
* press tab (default) while flying to toggle altitude lock
* Fly Up and Fly Down are by default bound to space and shift, respectively
* press the flying key and the mod key at the same time to toggle fast flight
* the run key is now bindable
* fully disabled collisions between the player and wires. You can walk right through them now
* the player can now fit into gaps as skinny as 40cm (was previously 100cm)
* new values in settings.txt to change movement speed: "WalkSpeed", "RunSpeed", "JumpSpeed", "FlyingSpeed", "FlyingVerticalSpeed", "FastFlyingSpeed", and "FastFlyingVerticalSpeed"

UI
* horizontally scrolling menus now wrap around
* new option, on by default, to tap twice to use the board menu instead of holding down the key
* in the board menu, when an action is selected that will immediately affect a board, the board it will affect and everything attached to it is now highlighted in blue
* holding the mod key while cloning or moving a board will clone or move the entire hierarchy  that board is in. The highlights in the board menu change to reflect that
* pressing the place button in the board menu will now execute the selected board action
* pressing esc in the board menu will now close it
* you can now use the number select keys in the paint board menu
* in the paint menu, looking at a board will highlight it and show a preview of the new color
* paint board menu icons are now smaller and closer together
* paint board menu icons are now closer in color to the colors they'll apply to a board
* the default list of colors for the paint board menu has been expanded to 22 colors, up from 12 in 0.1
* you can now apply a paint color with the Place button
* items in the selection menu appear much brighter
* items in the selection menu have been squished closer together to make room for the new ones
* the time that the selection menu stays open after you scroll or pressing a number key is now editable in settings.txt under the field "SelectionMenuOpenTime"
* adjusted the order of the items in the selection menu
* when you backspace out a number in the new board menu, it is no longer immediately replaced by a 1. You can proceed to type in your own number
* all dropdowns in the options menu have much more space between the options
* when adjusting mouse sensitivity in the options menu, you can adjust both axes at once by holding the mod key
* the maximum value of the shadow distance slider is now 300 (was 200)
* the FPS counter now updates every 0.1 seconds instead of every frame
* added an epilepsy warning when the game is first launched
* added a popup message that appears every five times the game is launched that encourages players to check out the email list, subreddit, and discord. Each popup has a "Don't show me again" button.
* the path the camera takes in the main menu has been slightly adjusted to spend more time looking at the cool display part
* the main menu music will no longer start playing until the main menu has finished loading
* clicking the Mouse Hat Games logo in the main menu now opens the MHG twitter page
* pressing F2 in the Load Game menu will now open the rename dialog for the selected save
* the Load, Rename, Delete and Duplicate buttons in the load game menu are now disabled until you select a save
* added the game's logo to the about menu
* added a link to the official discord in the about menu
* about menu text is smaller
* about menu now uses Inconsolata, a much nicer font for large blocks of text
* about menu is now much more sensitive to scrolling
* crosshair is smaller
* all UI now scales better to different aspect ratios
* replaced many low resolution icons with high resolution versions made by Ben Longley. Thanks Ben!

SOUND
* you will no longer hear the same music track twice in a row
* the default time between music is now 200 seconds (was 100 seconds)
* new music tracks will now play while the game is paused (previously the current one would finish playing and nothing new would play unless you unpaused the game)
* many new music tracks!
	Vasquez Rock, Antelope Valley, and Zion National Park by Lauren X. Pham
	Gratitude, Pleasure, Calm, Floating, Light, Plants, Sun, Moon, Changing, and 71017 by Borrtex
	The Temperature of the Air on the Bow of the Kaleetan; Readers! Do You Read?; Out of the Skies, Under the Earth; CGI Snake; Cylinder Three; and Cylinder Six by Chris Zabriskie

	THANK YOU to all those artists for letting me use your beautiful music!
* many new sound effects made by the talented Emmanuel Lagumbay
	place something on a board
	place something on terrain
	rotate something
	delete something
	wire creation
	take a screenshot
	click a UI button
	walking & jumping, with different footstep sounds for each type of terrain. The old 0.1 footstep sounds are now used for walking on stuff you build

VISUALS
* wires now have thickness instead of being sheets of paper. You can use the old, thin wires with "UseThinWires" in settings.txt
* tweaked the shade of the colors for green, red and blue highlights
* highlight borders now have corners for a cleaner look. This can impact performance on some systems and can be disabled with "CornerOutlines" in settings.txt
* highlight borders are thinner
* ambient occlusion is now on by default, at a lower intensity
* fixed distant terrain shadows flickering

TECHNICAL
* the set of circuit components that checks for an update every tick is now a dynamic size. Components will be added to it when circuitry connect to them changes state, and removed from it after they have updated.
* combined meshes now have a maximum complexity (number of vertices). If a combined mesh reaches its maximum complexity, a new combined mesh of its type will be created
* two new fields in settings.txt related to the above system: "MaxVerticesPerStaticMegaMesh" (stuff that doesn't change color during circuitry updates, i.e. circuit boards) and "MaxVerticesPerDynamicMegaMesh" (stuff that changes color during circuitry updates, i.e. displays)
* there is a maximum number of combined meshes that can be recalculated per second. It can be configured with "MaxMeshGroupRecalculationsPerSecond" in settings.txt, default is 20.
* the entire codebase has been combed through and cleaned up. Future features will be easier and faster to add, both officially and through mods
* music is now streamed from disk, saving RAM
* various other optimizations
* when placing, rotating or moving an object, its position will be rounded to the nearest millimeter and its rotation will be rounded to the nearest 1/36 of a degree. This is to combat floating point errors that accumulate over time.
* new save format that's smaller and faster - thank you Stenodyon for your help here!
* autosaves and auto backups now write data on a separate thread
* the game will no longer be saved when quitting from the pause menu, as it was already saved when the game was paused. It will still save when quitting via alt-F4 ect
* saves using the old format will have a notification in the Load Game menu
* when loading an old save, a backup will be created and then the save will be converted to the new format
* the game will no longer lock up and corrupt your save if, when you load a save, a wire can't find which pegs it's connected to
* save backups are now stored in The Ultimate Nerd Game/backups (previously The Ultimate Nerd Game/saves/backups)
* all files generated by the game with a timestamp are now named much more nicely, in the format yyyy-mm-dd_hh-mm-ss

MISCELLANEOUS
* added a zoom key, default binding Y
* when on the main menu the game will continuously check for new save files in the saves folder, and if any are detected they will be added to the load game menu
* adjusted the demo save to compensate for the new button behavior
* you can now bind the key to interact with stuff in the world (levers, buttons etc) separately from the Place key
* Look Through Board now makes boards more transparent than before
* Look Through Board is now bound to G by default
* the default screenshot supersize value is now 2
* the game, by default, no longer pauses and mutes when the window is defocused. Pause on defocus can be turned on with "PauseOnDefocus" in settings.txt
* the screen now goes dark while the game is loading something. Will be replaced with a proper loading screen in the future
* small tweaks to the structure of the readme file
* updated help menu for all the new features
* fixed the game being totally broken on Mac
* fixed a crap ton of bugs
* added some new bugs to fix later



v0.1.5 | 2018-01-23

* fixed a bug where a connection between two inputs, one or more of which was part of a through peg, would sometimes not be saved



v0.1.4 | 2018-01-12

* fixed a save-corrupting bug that occured when you tried to make very very short wires
* added "PaintColors" key to settings.txt that lets you modify what colors are available in the paint board menu
* the UI scales better at small aspect ratios



v0.1.3 | 2018-01-09

* wire hitboxes now extend vertically for a bit. Makes it easier to delete/rotate them when viewed at shallow angles
* added new value editable in settings.txt, EnableMainMenuCameraPan. Setting this to false disables the camera movement in the main menu for people who get motion sick from it
* the game no longer saves when you unpause via the escape key
* fixed being able to get wires into invalid positions by rotating components
* fixed being unable to lock rotation on panel switches, panel buttons, and through blotters
* fixed typo in the input configuration dialog (Rotate Clcokwise -> Rotate Clockwise)
* fixed me forgetting to update the in-game version number :P



v0.1.2 | 2018-01-08

* properly fixed being able to place wires in places where they would be deleted when the save was loaded. In 0.1.1 the game limited wires to a certain length, now it actually checks if the wire can be loaded successfully. Prevents some edge cases where you could still place non-persistent wires.
* fixed sometimes not being able to delete empty boards
* fixed inputs sometimes becoming invisible
* you can now rotate wires in the same way you rotate other game objects. Handy for when the default rotation puts them at an angle that's hard to see.



v0.1.1 | 2018-01-07 (but later)
Thanks to everybody who's played the game and left feedback!
* fixed a bug that let you place wires in places where they would be deleted when the save was loaded
* the player no longer collides with wires; makes it easier to navigate (messy) circuits
* doubled the reach distance of the player to 20 meters. This will be customizable in the future
* doubled the time the selection menu stays open after scrolling or pressing a number key to 0.8 seconds. This will be customizable in the future



v0.1 | 2018-01-07

* initial release