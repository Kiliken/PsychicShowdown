@echo off

::if exist "%windir%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe" set compiler=%windir%\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe
::set refpath=C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0
if exist "%windir%\Microsoft.NET\Framework64\v4.0.30319\csc.exe" set compiler="%windir%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if not exist "%cd%\release" md release
:: %compiler% -t:winexe -out:"release\Program.exe" -r:"%refpath%\PresentationCore.dll" -r:"%refpath%\PresentationFramework.dll" -r:"%refpath%\WindowsBase.dll" src\*.cs
%compiler% -out:"%cd%\release\Server.exe" "%cd%\src\Server.cs"

%compiler% -out:"%cd%\release\Client.exe" "%cd%\src\Client.cs"


::%compiler%  "%projname%.csproj"

pause