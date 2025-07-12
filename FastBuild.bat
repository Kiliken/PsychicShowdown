@echo off

if exist "C:\Program Files\Unity\Hub\Editor\2022.3.56f1\Editor\Unity.exe" set unity="C:\Program Files\Unity\Hub\Editor\2022.3.56f1\Editor\Unity.exe"
if exist "D:\Program Files\Unity\Editor\2022.3.56f1\Editor\Unity.exe" set unity="D:\Program Files\Unity\Editor\2022.3.56f1\Editor\Unity.exe"


%unity% -quit -batchmode -projectPath %cd% -buildWindows64Player "%cd%\Build\Test\GameTest.exe"

pause