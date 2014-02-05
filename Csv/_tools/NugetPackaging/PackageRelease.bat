@ECHO OFF

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Msbuild.exe /verbosity:m /nologo /p:Configuration=Release ..\..\Nortal.Utilities.Csv.csproj
pause
..\..\..\.nuget\nuget.exe pack -Outputdirectory output Nortal.Utilities.Csv.nuspec
pause