@echo off

if exist "%cd%\Build\Test\GameTest.exe" set game="%cd%\Build\Test\GameTest.exe"

:: echo "-logFile <pathname>	Specify where Unity writes the standalone Player log file. To output to the console, specify - for the path name. On Windows, use -logfile to direct the output to stdout, which by default is not the console."


set /p commands="[Unity] "

%game% -logFile "%cd%\Build\Test\GameTestLog.txt" %commands%

pause