Echo Off
CALL "C:\Program Files\AutoHotkey\AutoHotkey.exe" "C:\SWPAW\AHK\Demo.ahk" "%~1" %2
REM Echo %ErrorLevel%
REM ECHO Pause
Exit %Errorlevel%