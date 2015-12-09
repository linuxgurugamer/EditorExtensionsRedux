## Editor Extensions v3.00 For KSP v1.0.5

### Changes in v3.00 - 9 December 2015
* Recompiled for 1.0.5
* Merged with SelectRoot mod, written by FW Industries


## Editor Extensions v2.12 For KSP v1.0.3

### Changes in v2.12 - 23 June 2015
* Fixed conflict with new angle snap behavior in KSP v1.0.3 editor. May introduce a flicker/twitch when changing angles, but has no negative effect.

### Changes in v2.11 - 22 June 2015
* Fixed conflict with new symmetry mode behavior in KSP v1.0.3 editor. May introduce a flicker/twitch when changing symmetry modes 1-8, but has no negative effect.
* Recompiled against KSP v1.0.3 binaries
* skipped v2.9/.10 due to deployment/versioning issues

### Changes in v2.8 - 21 May 2015
* New strut and fuel line alignment logic
* U snaps strut/fuel line directly between parts, with each end at nearest top, bottom, middle or one-quarter position.
* Alt/Mod-U aligns strut at current height straight between parts, does not snap height on either end.
* Vertical/Horizontal alignment takes no action if part is currently attached to a node (green ball)
* Numpad . (period) centers camera around part under cursor. Incomplete feature - while refocused part dragging is offset. Hitting period with no part under cursor resets back to normal.

####[Download v2.12 via GitHub](https://github.com/MachXXV/EditorExtensions/releases/download/v2.12/EditorExtensions_v2.12.zip)

####Alternate Downloads
* [KerbalStuff](https://kerbalstuff.com/mod/442)
* [Curseforge](http://kerbal.curseforge.com/ksp-mods/230776)
* Via the [KSP CKAN Package Manager](http://forum.kerbalspaceprogram.com/threads/100067)

### Features
* Allows custom levels of radial symmetry beyond the stock limitations.
* Horizontally and vertically center parts.
* Re-Align placed struts and fuel lines between parts 
* Adds radial/angle snapping at 1°,5°,15°,22.5°,30°,45°,60°, and 90°. Angles are customizable.
* Toggle part clipping (From the cheat options)
* Toggle radial and node attachment of parts
* Reset hangar camera view
* Customize hotkeys
* CKAN & KSP-AVC versioning support

#### Vertical/Horizontal snap:
* Place the part, then once the part is placed, hover over the part with your mouse and press the Vertical or Horizontal snap hotkey.
* For vertical snap, part will center itself on the part lengthwise in the SPH

#### Strut & Fuel line alignment
* Place the strut, then hover over the base/start of the strut (the first end placed) with the mouse, and press the hotkey.
* Strut/FL start and end with be snapped to the closest of either the middle, quarter, or end of the part, aligned directly between the two parts.
* Mod/Alt-U will reposition the strut/FL directly between the parts, but only level out the strut from the start/parent part.

### Default Keybindings
* **V** 			- Vertically center a part. Place the part, hover over it with the mouse, and press the hotkey.
* **H** 			- Horizontally center the part. Place the part, hover over it with the mouse, and press the hotkey.
* **U** 			- Place the strut, then hover over the base/start of the strut (the first end placed) with the mouse, and press the hotkey.
* **Mod/Alt-U**		- Strut will be aligned level with its starting position
* **X, Shift+X** 	- Increase/Decrease symmetry level (Based on KSP's key map)
* **Alt+X** 		- Reset symmetry level (Based on KSP's key map)
* **C, Shift+C** 	- Increase/Decrease angle snap (Based on KSP's key map)
* **Alt+C**			- Reset angle snap (Based on KSP's key map)
* **T** 			- Attachment mode: Toggle between surface and node attachment modes for all parts, and when a part is selected, will toggle surface attachment even when that part's config usually does not allow it.
* **Alt+Z** 		- Toggle part clipping (CAUTION: This is a cheat option)
* **Space** 		- When no part is selected, resets camera pitch and heading (straight ahead and level)

In this version there is also a still-incomplete feature: A part-zoom/part camera orbit - the numpad . key will focus and orbit the camera around the part under the mouse. hitting numpad . again with no part under the mouse will reset the camera back to normal. Currently in the focus mode dragging parts gets skewed so it is only good for viewing the part from another perspective, and not editing or moving parts.

###Installation
In your KSP GameData folder, delete any existing EditorExtensions folder.
Download the zip file to your KSP GameData folder and unzip.

[KSP Forum Thread](http://forum.kerbalspaceprogram.com/threads/38768)

Released under MIT license.
Source available at GitHub: [https://github.com/MachXXV/EditorExtensions](https://github.com/MachXXV/EditorExtensions)

