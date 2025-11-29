@ECHO OFF

SETLOCAL EnableDelayedExpansion

SET _NET_ROOT=%~dp0..\
SET _MIGRATION_NAME=%~1
SET _MIGRATION_DB=GameScrubsV2DbContext

IF NOT DEFINED _MIGRATION_NAME (
	SET /p _MIGRATION_NAME="Please enter the name of the migration: "
)

PUSHD "%_NET_ROOT%"

ECHO [94mAdding new migration !_MIGRATION_NAME![0m

dotnet ef migrations add !_MIGRATION_NAME! --context !_MIGRATION_DB! --project GameScrubsV2/GameScrubsV2.csproj

if ERRORLEVEL 1 (
	ENDLOCAL
	POPD
	ECHO [91mError adding database migration[0m
	EXIT /b 1
)

ENDLOCAL
POPD
ECHO [32mSuccess[0m
