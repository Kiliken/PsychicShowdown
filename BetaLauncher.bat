@echo off

if exist "%cd%\Build\Test\GameTest.exe" set game="%cd%\Build\Test\GameTest.exe"

set commands="-screen-fullscreen 1 -screen-height 1080 -screen-width 1920 -screen-quality Ultra"

%game% -logFile "%cd%\Build\Test\GameTestLog.txt" %commands%