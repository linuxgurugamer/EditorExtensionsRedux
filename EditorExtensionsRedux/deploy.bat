

mkdir R:\KSP_1.0.5_Dev\GameData\EditorExtensionsRedux
mkdir R:\KSP_1.0.5_Dev\GameData\EditorExtensionsRedux\Textures

copy /Y "%~dp0bin\Debug\EditorExtensionsRedux.dll" "R:\KSP_1.0.5_Dev\GameData\EditorExtensionsRedux"
copy /Y "%~dp0bin\Debug\Textures\*.png" "R:\KSP_1.0.5_Dev\GameData\EditorExtensionsRedux\Textures"
copy /Y "%~dp0bin\Debug\EditorExtensionsRedux.version" "R:\KSP_1.0.5_Dev\GameData\EditorExtensionsRedux"

copy /Y "%~dp0bin\Debug\License.txt" "R:\KSP_1.0.5_Dev\GameData\EditorExtensionsRedux"
copy /Y "%~dp0bin\Debug\README.md" "R:\KSP_1.0.5_Dev\GameData\EditorExtensionsRedux"