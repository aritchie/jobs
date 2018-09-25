
@echo off

nuget push .\Plugin.Jobs\bin\Release\*.nupkg -Source https://www.nuget.org/api/v2/package
del .\Plugin.Jobs\bin\Release\*.nupkg

nuget push .\Plugin.Jobs.Autofac\bin\Release\*.nupkg -Source https://www.nuget.org/api/v2/package
del .\Plugin.Jobs.Autofac\bin\Release\*.nupkg

pause