##
## Editor Extensions 
##
##

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
* Rapid zoom mode when using keys to zoom (when using stock zoom mode)

#### Vertical/Horizontal snap:
* Place the part, then once the part is placed, hover over the part with your mouse and press the Vertical or Horizontal snap hotkey.
* For vertical snap, part will center itself on the part lengthwise in the SPH

#### Master Snapping
* Hold down "left control" and select the "master" part.
* Hover the cursor over the child part and press "v" for vertical snap or "h" for a horizontal snap. This will snap the child part to the master part's vertical or horizontal position.
* Click on an empty space to turn off Master Snap.
* This should prove to be very useful for VTOLs where you can't apply engines in symmetry.

#### Strut & Fuel line alignment
* Place the strut, then hover over the base/start of the strut (the first end placed) with the mouse, and press the hotkey.
* Strut/FL start and end with be snapped to the closest of either the middle, quarter, or end of the part, aligned directly between the two parts.
* Mod/Alt-U will reposition the strut/FL directly between the parts, but only level out the strut from the start/parent part.

#### Rapid Zoom mode
* Zoom in / out more rapidly by double-tapping the zoom in / out hotkey.  i.e. If you just hold down the "zoom in" hotkey, 
  it works like normal.  But if you double-tap-hold, it zooms in at 5x the speed.  Releasing the hotkey sets zoom back to normal


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

### Warning on Keybindings
* Note in very rare circumstances the symmetry and angle snap keybindings might be lost.  This would only occur 
  if another mod triggers the game to save its settings while in the VAB/SPH, *and* the game subsequently crashes 
  or is killed before exiting the building.  If it happens, you can easily restore the keybindings (X and C by 
  default) by hovering over the toolbar button and selecting the menu option "Reset Mode & Snap keys".

### Stock keybinding (change in stock config screen)
 * **R**			- Toggle symmetry mode from SPH to VAB style (mirror to radial).
#### Strip Symmetry
* **Alt + Shift + Left Click** on the part and symmetry will be stripped from it
  ***On Linux, the keys are:  **Left-Alt Left-Shift Left-Click**

#### No Offset Limits
* The offset tool now does not have any limits.  Nothing needs to be done, this just works

#### Select Root 2 Behaviour
* This simplifies the select root functionality.  No need to click the same part multiple times.
* **4**			- Activates the stock code, or click on the select-root icon
			      Click on the part you want to be the new root.  It will be made root AND will be selected,
			      so you need to position the ship and click to drop it

In this version there is also a still-incomplete feature: A part-zoom/part camera orbit - the numpad . key will focus and orbit the camera around the part under the mouse. hitting numpad . again with no part under the mouse will reset the camera back to normal. Currently in the focus mode dragging parts gets skewed so it is only good for viewing the part from another perspective, and not editing or moving parts.

###Installation
In your KSP GameData folder, delete any existing EditorExtensions folder.
Download the zip file to your KSP GameData folder and unzip.

[KSP Forum Thread](http://forum.kerbalspaceprogram.com/index.php?/topic/127378-editor-extensions-redux-301-released-with-selectroot-merge-stripsymmetry-nooffsetlimits/)

Released under MIT license.
Source available at GitHub: [https://github.com/MachXXV/EditorExtensions](https://github.com/MachXXV/EditorExtensions)
