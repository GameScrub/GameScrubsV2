@ECHO OFF

SETLOCAL EnableDelayedExpansion

SET _NET_ROOT=%~dp0..\
SET _MIGRATION_NAME=%~1
SET _MIGRATION_DB=GameScrubsV2DbContext
SET ASPNETCORE_ENVIRONMENT=%~2

IF NOT DEFINED ASPNETCORE_ENVIRONMENT (
	SET ASPNETCORE_ENVIRONMENT=Development
)

IF NOT DEFINED _MIGRATION_NAME (
	SET /p _MIGRATION_NAME="Please enter the name of the migration: "
)

PUSHD !_NET_ROOT!

ECHO [94mReverting to migration !_MIGRATION_NAME! in !ASPNETCORE_ENVIRONMENT!...[0m

dotnet ef database update !_MIGRATION_NAME! --context "!_MIGRATION_DB!" --project GameScrubsV2/GameScrubsV2.csproj

IF ERRORLEVEL 1 (
	ENDLOCAL
	POPD
	ECHO [91mError updating database[0m
	ECHO Error updating database
	EXIT /b 1
)

ENDLOCAL
POPD
ECHO [32mSuccess[0m
