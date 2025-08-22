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
ECHO ^| Presets:           ^|                      ^|
ECHO ^| -port              ^| fullscreen portrait  ^|
ECHO ^| -land              ^| fullscreen landscape ^|
ECHO ^| -land4k            ^| fs4k landscape       ^|
ECHO +--------------------+----------------------+


set /p commands="[Unity] "


if "%commands:~0,5%"=="-land" (
    set commands=-screen-fullscreen 1 -screen-height 1080 -screen-width 1920 -screen-quality Ultra
)

if "%commands:~0,5%"=="-port" (
    set commands=-screen-fullscreen 1 -screen-height 1920 -screen-width 1080 -screen-quality Ultra -scene AlphaPort
)

if "%commands:~0,7%"=="-land4k" (
    set commands=-screen-fullscreen 1 -screen-height 2160 -screen-width 3840 -screen-quality Ultra -scene AlphaPortFHD
)


%game% -logFile "%cd%\Build\Test\GameTestLog.txt" %commands%

pause