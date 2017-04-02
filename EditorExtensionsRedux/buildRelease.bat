@echo off
set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%

rem set /p HOMEDIR= "Enter Home directory, or <CR> for default: "

if "%HOMEDIR%" == "" (
set HOMEDIR=%DEFHOMEDIR%
)
echo %HOMEDIR%

SET _test=%HOMEDIR:~1,1%
if "%_test%" == ":" (
set HOMEDRIVE=%HOMEDIR:~0,2%
)

copy EditorExtensionsRedux.version a.version
set VERSIONFILE=a.version
rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/
c:\local\jq-win64  ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile

c:\local\jq-win64  ".VERSION.MINOR"  %VERSIONFILE% >tmpfile
set /P minor=<tmpfile

c:\local\jq-win64  ".VERSION.PATCH"  %VERSIONFILE% >tmpfile
set /P patch=<tmpfile

c:\local\jq-win64  ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile
del tmpfile
set VERSION=%major%.%minor%.%patch%
if "%build%" NEQ "0"  set VERSION=%VERSION%.%build%

echo Version:  %VERSION%
rem copy /Y README.md GameData\FuseBox

del a.version

cd ..

mkdir GameData\EditorExtensionsRedux
mkdir GameData\EditorExtensionsRedux\Textures
mkdir GameData\EditorExtensionsRedux\Plugins
mkdir GameData\EditorExtensionsRedux\PluginData
mkdir GameData\EditorExtensionsRedux\PluginData\StripSymmetry

pause
del /Q GameData\EditorExtensionsRedux
del /Q GameData\EditorExtensionsRedux\Textures


copy /Y "%~dp0bin\Release\EditorExtensionsRedux.dll" "GameData\EditorExtensionsRedux\Plugins"
copy /Y "%~dp0bin\Release\Textures\*.png" "GameData\EditorExtensionsRedux\Textures"
copy /Y "EditorExtensionsRedux\EditorExtensionsRedux.version" "GameData\EditorExtensionsRedux"

copy /Y "EditorExtensionsRedux\License.txt" "GameData\EditorExtensionsRedux"
copy /Y "README.md" "GameData\EditorExtensionsRedux"
copy /Y ..\MiniAVC.dll  "GameData\EditorExtensionsRedux"

copy /y "EditorExtensionsRedux\StripSymmetry\Gamedata\StripSymmetry\plugins\PluginData\StripSymmetry\config.xml"  "GameData\EditorExtensionsRedux\PluginData\StripSymmetry"
pause


set FILE="%RELEASEDIR%\EditorExtensionsRedux-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% Gamedata\EditorExtensionsRedux
pause