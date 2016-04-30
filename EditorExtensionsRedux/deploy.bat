

mkdir R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux
mkdir R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux\Textures
mkdir R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux\PluginData
mkdir R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux\PluginData\StripSymmetry


copy /Y "%~dp0bin\Debug\EditorExtensionsRedux.dll" "R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux"
copy /Y "%~dp0bin\Debug\Textures\*.png" "R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux\Textures"
copy /Y "%~dp0bin\Debug\EditorExtensionsRedux.version" "R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux"

copy /Y "%~dp0bin\Debug\License.txt" "R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux"
copy /Y "%~dp0bin\Debug\README.md" "R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux"
cd
copy /y "StripSymmetry\Gamedata\StripSymmetry\plugins\PluginData\StripSymmetry\config.xml"  "R:\KSP_1.1.1_dev\GameData\EditorExtensionsRedux\PluginData\StripSymmetry"
