@echo off

if exist "%cd%\Build\Test\GameTest.exe" set game="%cd%\Build\Test\GameTest.exe"

:: echo "-logFile <pathname>	Specify where Unity writes the standalone Player log file. To output to the console, specify - for the path name. On Windows, use -logfile to direct the output to stdout, which by default is not the console."
ECHO +--------------------+----------------------+
ECHO ^| -monitor           ^| 1/2                  ^|
ECHO ^| -screen-fullscreen ^| 0/1                  ^|
ECHO ^| -screen-height     ^| 1080/ 1920           ^|
ECHO ^| -screen-width      ^| 1920/1080            ^|
ECHO ^| -screen-quality    ^| Very Low/Low/Medium  ^|
ECHO ^|                    ^| High/Very High/Ultra ^|
ECHO +--------------------+----------------------+

set /p commands="[Unity] "

%game% -logFile "%cd%\Build\Test\GameTestLog.txt" %commands%

pause