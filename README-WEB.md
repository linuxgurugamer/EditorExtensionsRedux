I just released a redux of the original editorExtensions along with the SelectRoot mod:

http://spacedock.info/mod/48/Editor%20Extensions%20Redux

Source:  https://github.com/linuxgurugamer/EditorExtensionsRedux

Please note that this mod is version specific.  I just released an update for the new 1.1.1 1.1.2 release of KSP

 

This version WILL only work on KSP 1.4.1.  There is no support for KSP 1.4.0

Please note that the best way to tell if it works is to download and install it, if it isn't compatible, it will tell you.  It's been known to happen that I update the mod and forget to update this OP
Dependencies

 Click Through Blocker
Toolbar Controller
CKAN has been updated to install both automatically, but if installing manually, you must install them for this to work

Also, be sure you are installing the latest versions

Installation

In your KSP GameData folder, delete any existing EditorExtensions folder.Download the zip file to your KSP GameData folder and unzip.

or use CKAN when it becomes available

License: MIT (both of the original mods for this used the MIT license)

Donations gratefully accepted

Patreon.png

https://www.patreon.com/linuxgurugamer

New Feature (8/6/2018)

Added button to Settings page 2 to "Reset Symmetry Mode & Angle Snap keys
New features

Added ability to disable the SelectRoot functionality, so you can use the stock SelectRoot to change the root on a shadow assembly
Added Master Snap mode.  This allows you to snap parts to any random part, not just the parent.  This works for both horizontal and vertical snapping.  The part selected as the master will be highlighted.  See below for info on usage
Snapping is shown visually via a smooth movement of the part from the original location to the destination
Features

Allows custom levels of radial symmetry beyond the stock limitations.
Horizontally and vertically center parts.
Adds radial/angle snapping at 1,5,15,22.5,30,45,60, and 90 degrees. Angles are customizable.
Toggle part clipping (From the cheat options)
Re-Align placed struts and fuel lines between parts 
Toggle radial and node attachment of parts
Reset hangar camera view
Customize hotkeys
KSP-AVC versioning support
Fine adjustments of rotation and translation now working
Vertical/Horizontal snap:

Place the part, then once the part is placed, hover over the part with your mouse and press the Vertical or Horizontal snap hotkey.
For vertical snap, part will center itself on the part lengthwise in the SPH
Master Snapping

Hold down "left control" and select the "master" part 
Hover the cursor over the child part and press "v" for vertical snap or "h" for a horizontal snap. This will snap the child part to the master part's vertical or horizontal position
Click on an empty space to turn off Master Snap
This should prove to be very useful for VTOLs where you can't apply engines in symmetry.

Strut & Fuel line alignment

Place the strut, then hover over the base/start of the strut (the first end placed) with the mouse, and press the hotkey.
Strut/FL start and end with be snapped to the closest of either the middle, quarter, or end of the part, aligned directly between the two parts.
Mod/Alt-U will reposition the strut/FL directly between the parts, but only level out the strut from the start/parent part.
Fine Adjustments

Open Fine Adjust window, then select a part with either the rotation or translation gizmo
The window will show the correct info for the current gizmo when you select a part.  If you change gizmos while not selecting a new part, the window will not update for the new gizmo, but you can select the right one by clicking the button at the top of the window
Keys will allow adjusting the rotation and translation with greater precision
Default Keybindings

V- Vertically center a part. Place the part, hover over it with the mouse, and press the hotkey.
H- Horizontally center the part. Place the part, hover over it with the mouse, and press the hotkey.
U- Place the strut, then hover over the base/start of the strut (the first end placed) with the mouse, and press the hotkey.
X, Shift+X- Increase/Decrease symmetry level (Based on KSP's key map)
Alt+X- Reset symmetry level (Based on KSP's key map)
C, Shift+C- Increase/Decrease angle snap (Based on KSP's key map)
Alt+C- Reset angle snap (Based on KSP's key map)
T- Attachment mode: Toggle between surface and node attachment modes for all parts, and when a part is selected, will toggle surface attachment even when that part's config usually does not allow it.
Alt+Z- Toggle part clipping (CAUTION: This is a cheat option)
Space- When no part is selected, resets camera pitch and heading (straight ahead and level)
B - Horizontally center the root part in the editor, and vertically position the vessel so the bottom part is 5m high (new)
Warning on Keybindings

Note in very rare circumstances the symmetry and angle snap keybindings might be lost.  This would only occur   if another mod triggers the game to save its settings while in the VAB/SPH, *and* the game subsequently crashes  or is killed before exiting the building.  If it happens, you can easily restore the keybindings (X and C by  default) by hovering over the toolbar button and selecting the menu option "Reset Mode & Snap keys".
Stock keybinding (change in stock config screen)

 R - Toggle symmetry mode from SPH to VAB style (mirror to radial).
Fine Adjust Translation Default Keybindings

Up-arrow - translates up
Down-arrow - translates down
Left-arrow - translates left
Right-arrow - translates right
Right-Shift - translates forward
Right-Control - translates backward
Fine Adjust Rotation Default Keybindings

Up-arrow - rotates forward
Down-arrow - rotates backward
Left-arrow - rotates left
Right-arrow - rotates right
Right-Shift - rotates clockwise
Right-Control - rotates counterclockwise
Strip Symmetry

Alt-Shift left-click on the part and symmetry will be stripped from it
No Offset Limits

The offset tool now does not have any limits.  Nothing needs to be done, this just works
Select Root Functionality

Squad added a root part selection mode to stock. But it's a bit awkward to use. This plugin fixes the following things:

Only one click needed. Just click on the part you want to be the new root.
Works if you're already hovering the new part. No more mouse wiggling to get the selection to register.
Drops the new root, so you can grab something else immediately.
Select Root Usage: Enter root selection mode and click on the new root part. Root selection mode is one of the buttons on top next to the rotate and offset gizmos. Or just press 4

Note: that for ghosted subassembly re-root to work, you need to deactivate "1 click reroot" feature.  Once back to "vanilla 2 click re-root", you can click re-root, click the ghosted subassembly and then click the engine on the ghosted subassembly.

 

Demonstration of strut alignment:

https://youtu.be/vsn25o4QSf4
 