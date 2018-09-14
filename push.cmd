@echo off

nuget push .\Plugin.Jobs\bin\Release\*.nupkg -Source https://www.nuget.org/api/v2/package
del .\Plugin.Jobs\bin\Release\*.nupkg

pause