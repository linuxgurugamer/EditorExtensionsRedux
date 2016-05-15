

mkdir R:\KSP_1.1.2_dev\GameData\EditorExtensionsRedux
mkdir R:\KSP_1.1.2_dev\GameData\EditorExtensionsRedux\Textures
mkdir R:\KSP_1.1.2_dev\GameData\EditorExtensionsRedux\PluginData
mkdir R:\KSP_1.1.2_dev\GameData\EditorExtensionsRedux\PluginData\StripSymmetry



set H=R:\KSP_1.1.2_dev
echo %H%

set d=%H%
if exist %d% goto one
mkdir %d%
:one
set d=%H%\Gamedata
if exist %d% goto two
mkdir %d%
:two
set d=%H%\Gamedata\EditorExtensionsRedux
if exist %d% goto three
mkdir %d%
:three
set d=%H%\Gamedata\EditorExtensionsRedux\Plugins
if exist %d% goto four
mkdir %d%
:four
set d=%H%\Gamedata\EditorExtensionsRedux\PluginData
if exist %d% goto five
mkdir %d%
:five
set d=%H%\Gamedata\EditorExtensionsRedux\PluginData\StripSymmetry
if exist %d% goto six
mkdir %d%
:six
set d=%H%\Gamedata\EditorExtensionsRedux\Textures
if exist %d% goto seven
mkdir %d%
:seven




copy /Y "%~dp0bin\Debug\EditorExtensionsRedux.dll" "%H%\GameData\EditorExtensionsRedux\Plugins"
copy /Y "%~dp0bin\Debug\Textures\*.png" "%H%\GameData\EditorExtensionsRedux\Textures"
copy /Y "%~dp0bin\Debug\EditorExtensionsRedux.version" "%H%\GameData\EditorExtensionsRedux"

copy /Y "%~dp0bin\Debug\License.txt" "%H%\GameData\EditorExtensionsRedux"
copy /Y "%~dp0bin\Debug\README.md" "%H%\GameData\EditorExtensionsRedux"
cd
copy /y "StripSymmetry\Gamedata\StripSymmetry\plugins\PluginData\StripSymmetry\config.xml"  "%H%\GameData\EditorExtensionsRedux\PluginData\StripSymmetry"
