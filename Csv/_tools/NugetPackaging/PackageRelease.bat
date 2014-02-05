@ECHO OFF
..\..\..\.nuget\nuget.exe pack -Outputdirectory output -Build -Properties Configuration=Release ..\..\Nortal.Utilities.Csv.csproj
pause