@ECHO OFF

REM Navigate to root directory
PUSHD %~dp0\..

REM Format the solution
ECHO [94mFormatting GameScrubsV2 solution...[0m
dotnet format .\GameScrubsV2.sln --severity info

REM Navigate back to the original working directory
POPD
