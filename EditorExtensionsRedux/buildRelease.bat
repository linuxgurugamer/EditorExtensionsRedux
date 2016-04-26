@echo off
set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%

set /p HOMEDIR= "Enter Home directory, or <CR> for default: "

if "%HOMEDIR%" == "" (
set HOMEDIR=%DEFHOMEDIR%
)
echo %HOMEDIR%

SET _test=%HOMEDIR:~1,1%
if "%_test%" == ":" (
set HOMEDRIVE=%HOMEDIR:~0,2%
)


type EditorExtensionsRedux.version
set /p VERSION= "Enter version: "


mkdir %HOMEDIR%\install\GameData\EditorExtensionsRedux
mkdir %HOMEDIR%\install\GameData\EditorExtensionsRedux\Textures
mkdir %HOMEDIR%\install\GameData\EditorExtensionsRedux\PluginData
mkdir %HOMEDIR%\install\GameData\EditorExtensionsRedux\PluginData\StripSymmetry


del %HOMEDIR%\install\GameData\EditorExtensionsRedux
del %HOMEDIR%\install\GameData\EditorExtensionsRedux\Textures


copy /Y "%~dp0bin\Release\EditorExtensionsRedux.dll" "%HOMEDIR%\install\GameData\EditorExtensionsRedux"
copy /Y "%~dp0bin\Release\Textures\*.png" "%HOMEDIR%\install\GameData\EditorExtensionsRedux\Textures"
copy /Y "EditorExtensionsRedux.version" "%HOMEDIR%\install\GameData\EditorExtensionsRedux"

copy /Y "License.txt" "%HOMEDIR%\install\GameData\EditorExtensionsRedux"
copy /Y "..\README.md" "%HOMEDIR%\install\GameData\EditorExtensionsRedux"
copy /Y MiniAVC.dll  "%HOMEDIR%\install\GameData\EditorExtensionsRedux"

copy /y "StripSymmetry\Gamedata\StripSymmetry\plugins\PluginData\StripSymmetry\config.xml"  "%HOMEDIR%\install\GameData\EditorExtensionsRedux\PluginData\StripSymmetry"

%HOMEDRIVE%
cd %HOMEDIR%\install

set FILE="%RELEASEDIR%\EditorExtensionsRedux-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% Gamedata\EditorExtensionsRedux
