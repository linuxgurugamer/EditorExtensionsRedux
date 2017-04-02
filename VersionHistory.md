##
## See the README.md for current version info
##
## Editor Extensions v3.02 For KSP v1.1
## Changes in 3.0.2
Recompiled for 1.1
Rewrote all Reflection code in Strip Symmetry, Select RootPart and NoOffsetLimits
Added Strip Symmetry
Added NoOffsetLimits

## Editor Extensions v3.00 For KSP v1.0.5

### Changes in v3.00 - 9 December 2015
* Recompiled for 1.0.5
* Merged with SelectRoot mod, written by FW Industries

### Previous Version History

### Changes in v2.12 - 23 June 2015
* Fixed conflict with new angle snap behavior in KSP v1.0.3 editor. May introduce a flicker/twitch when changing angles, but has no negative effect.

### Changes in v2.11 - 22 June 2015
* Fixed conflict with new symmetry mode behavior in KSP v1.0.3 editor. May introduce a flicker/twitch when changing symmetry modes 1-8, but has no negative effect.
* Recompiled against KSP v1.0.3 binaries

v2.9/v2.10 not released due to deployment/versioning issues on 3rd party sites.

### Changes in v2.8 - 21 May 2015
* New strut and fuel line alignment logic
* U snaps strut/fuel line directly between parts, with each end at nearest top, bottom, middle or one-quarter position.
* Alt/Mod-U aligns strut at current height straight between parts, does not snap height on either end.
* Vertical/Horizontal alignment takes no action if part is currently attached to a node (green ball)
* Numpad . (period) centers camera around part under cursor. Incomplete feature - while refocused part dragging is offset. Hitting period with no part under cursor resets back to normal.

### Changes in v2.7 - 2 May 2015
* Recompile against KSP v1.0.2 binaries
* Updated KSP-AVC version file to include wildcard for minor 1.0.* KSP versions
* Otherwise no changes from v2.6, not necessary to download if you are not getting messages from version-aware plugins.

### Changes in v2.6 - 28 April 2015
* KSP v1.0 support
* Recompile against KSP 1.0 libraries
* Changed angle snap (C) and symmetry (X) functions to use game's current key mapping
* Modifier keys Alt/Mod/Shift now use game's key mapping for better Linux support
* On screen messages now use the stock font/layout

### Changes in v2.5 - 27 December 2014
* Added strut & fuel line alignment options

### Changes in v2.4 - build 2.4.5472.4071 - 25 December 2014
* Bugfix release for a CPU/Framerate issue in v2.3, otherwise no changes from v2.3

### Changes in v2.3 - build 2.3.5471.37348 - 24 December 2014
* Revised vertical/horizontal snap
 * Now works with rotated/skewed parts - centers the part along the parent part axis.
 * Vertical snap will center itself lengthwise for horizontal parts in the SPH
 * Snapping disabled while a gizmo is active
* Bugfixes
 * Part movements made with snaps now work with undo/redo in the editor
 * Fixed an issue where snapping, using a gizmo, and pressing space to reset the gizmo warped the part away by 2x its original position.

### v2.2 - 2.2.5470.2764 - 23 December 2014

Bugfix: v2.1 caused a GUI error when opening the vessel loading screen. This was caused by a conflict between this mod's GUI skin and KSP's GUI skin.
This mod has been reverted to use the generic GUI skin to work around this issue. Otherwise no functional changes vs. v2.1

### v2.1 - build 2.1.5469.41325 - 22 December 2014
* Added new vertical snap (V)
 1. Place parts like normal.
 2. Once part is placed, hover over the part with your mouse and hit V. Part will align itself vertically to the middle of its parent part.
* Added new horizontal snap (H)
 1. Place parts like normal.
 2. Once part is placed, hover over the part with your mouse and hit H. Part will align itself horizontally to the middle of its parent part.
* Changed attachment mode toggling (T): now only toggles surface attachment of selected part.
* Added toolbar icon ("EEX") to bring up settings menu
 * Customize hotkeys
 * Customize angle snap values
 * Customize symmetry level
* Added KSP-AVC versioning support

### v2.0 - build 2.0.5466.37911 - 19 December 2014
* KSP 0.90 support
* Changed attachment mode toggling:
 * Changed Hotkey to "T", previous hotkey conflicted with new stock symmetry mode toggle (R)
 * Now toggles between surface attachment and node attachment. When surface attachment is enabled, part will not attach to regular nodes (green dots).
* Veritcal snap removed, new 0.90 editor does not have the same functionality. Currently working on new method of providing automatic vertical snap without having to use the translation gizmo.
* Removed launchpad/runway toggle due to incompatibility with KSP 0.90
* Removed redundant symmetry mode (radial/mirror) toggle, now in stock editor with R hotkey.
* This version is not compatible with any KSP versions prior to 0.90, for 0.25 use v1.4 which can be downloaded at the github page under releases.

#### 1.4 - 7 October 2014
  *   Recompiled against KSP 0.25 assemblies

#### 1.3 - 18 July 2014
  *   Recompiled against Mono/.NET 3.5 target to align with KSP runtime

#### #1.2 - 18 July 2014
*   Updated Text position for 0.24

#### #1.1 - 2 April 2014
*   Merged changes from forum user Ratzap
*   Updated plugin to use KSPAddonFixed
*   Code cleanup
*   Re-enabled VAB size increase

#### 1.0 beta 2 April 2014
*   Recompiled against 23.5 binaries and latest frameworks

#### 0.6 Beta - 5 August 2013
*   Symmetry mode will not wrap/loop around at maximum and minimum - stops at 50 on the way up with X, stops at 1 on the way down with Shift+X. Alt+X resets to 1

#### 0.5 Beta - 24 July 2013
*   Fixed compatibility with 0.21
*   Disabled VAB/SPH size modifications no longer working in 0.21
*   Removed feature to ingore hotkeys in ship name window no longer working in 0.21

#### 0.4 Alpha - 8 July 2013
*   Added on-screen messages when settings are toggled.
*   Space resets camera when no part is selected
*   Tab toggles between VAB and SPH editor modes while staying in the same building, and toggles launching to runway/launchpad.
*   Alt+C resets angle snap
*   Consolidated surface/radial attachment toggle to just use Alt+R and be context aware for when a part is selected

#### 0.3 Alpha - 5 July 2013
*   Added 1 degree to the angle snap options
*   Improved veritcal snap toggle behavior - automatically enables angle snap when vertical snap is enabled
*   Fixed radial/surface attachment toggling
-- Alt+R toggles normal surface attachment for all parts
-- Shift+Alt+R toggles surface attachment for the selected part, and also enables parts that usually can't be attached to a non-node surface (like hub connectors)

#### 0.2 Alpha - 4 July 2013
*   Added VAB/SPH interior space/camera limit increase
*   Known issues:
-- Radial/surface attachment not toggling properly.

#### 0.1 Alpha - 2 July 2013 - Initial release
*   Basic features implemented.
*   Known issues:
-- Radial/surface attachment not toggling properly.
-- Spaceplane hangar mode not tested
-- Mirror symmetry not tested
-- (Non-breaking/not user affecting) Debug log reports an error for out of bounds sprite animation when symmetry mode > 8
-- (Non-breaking/not user affecting) Debug log reports DivideByZero exception when angle snap is set to 0 and there is a part selected.
