@ECHO OFF

"%PROGRAMFILES(x86)%\MSBuild\14.0\Bin\Msbuild.exe" /verbosity:m /nologo /p:Configuration=Release ..\..\Nortal.Utilities.Csv.csproj
pause
..\..\..\.nuget\nuget.exe pack -Outputdirectory output Nortal.Utilities.Csv.nuspec
pause