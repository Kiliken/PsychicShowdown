::Other possible path
:: "D:\Program Files\Unity\Editor\2022.3.56f1\Editor\Unity.exe"
:: "C:\Program Files\Unity\Hub\Editor\2022.3.56f1\Editor\Unity.exe"
set unity="C:\Program Files\Unity\Hub\Editor\2022.3.56f1\Editor\Unity.exe"
%unity% -quit -batchmode -projectPath %cd% -buildWindowsPlayer "%cd%\Build\Test\GameTest.exe"
pause