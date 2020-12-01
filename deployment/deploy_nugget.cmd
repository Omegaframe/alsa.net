@echo off 

echo creating temp package dir
if not exist packages mkdir packages

echo building nuget packages
dotnet build ../projects/Alsa.Net/ -c Release
dotnet pack ../projects/Alsa.Net/ -c Release -o packages

set /p "key="<"\\STORAGE\Services\KeyStore\NugetKey_Omegaframe.txt"

echo pushing to nuget.org
for /f %%f in ('dir /b packages') do dotnet nuget push -k %key% -s https://api.nuget.org/v3/index.json packages/%%f

echo cleanup
rmdir /S /Q packages

echo nugets published