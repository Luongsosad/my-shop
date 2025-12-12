@echo off
REM Database Migration Management Tool for Windows
REM Usage: migrate.bat [command] [options]

setlocal enabledelayedexpansion

set "SCRIPT_DIR=%~dp0"
set "API_PROJECT=%SCRIPT_DIR%..\..\src\my-shop.API"
set "INFRA_PROJECT=%SCRIPT_DIR%..\..\src\my-shop.Infrastructure"

if "%~1"=="" goto show_help
if "%~1"=="help" goto show_help
if "%~1"=="--help" goto show_help
if "%~1"=="-h" goto show_help

if "%~1"=="add" goto migrate_add
if "%~1"=="list" goto migrate_list
if "%~1"=="apply" goto migrate_apply
if "%~1"=="rollback" goto migrate_rollback
if "%~1"=="status" goto migrate_status
if "%~1"=="script" goto migrate_script
if "%~1"=="remove" goto migrate_remove

echo ? Unknown command: %~1
goto show_help

:show_help
echo ?? Database Migration Management Tool
echo.
echo Usage: migrate.bat [command] [options]
echo.
echo Commands:
echo   add ^<name^>        Create a new migration
echo   list              List all migrations
echo   apply             Apply pending migrations
echo   rollback ^<name^>   Rollback to specific migration
echo   status            Show migration status
echo   script            Generate SQL script for pending migrations
echo   remove            Remove last migration (if not applied)
echo   help              Show this help message
echo.
echo Examples:
echo   migrate.bat add AddProductImages
echo   migrate.bat list
echo   migrate.bat apply
echo   migrate.bat status
goto :eof

:migrate_add
if "%~2"=="" (
    echo ? Migration name required
    echo Usage: migrate.bat add ^<MigrationName^>
    exit /b 1
)

set "MIGRATION_NAME=%~2"
echo ?? Creating migration: %MIGRATION_NAME%

cd "%INFRA_PROJECT%"
dotnet ef migrations add "%MIGRATION_NAME%" --startup-project "%API_PROJECT%" --output-dir Data\Migrations

if %ERRORLEVEL% EQU 0 (
    echo ? Migration created successfully!
    echo ?? Location: src\my-shop.Infrastructure\Data\Migrations\
) else (
    echo ? Failed to create migration
    exit /b 1
)
goto :eof

:migrate_list
echo ?? Listing all migrations
cd "%INFRA_PROJECT%"
dotnet ef migrations list --startup-project "%API_PROJECT%" --no-build
goto :eof

:migrate_apply
echo ??  Applying pending migrations...
cd "%INFRA_PROJECT%"
dotnet ef database update --startup-project "%API_PROJECT%"

if %ERRORLEVEL% EQU 0 (
    echo ? Migrations applied successfully!
) else (
    echo ? Failed to apply migrations
    exit /b 1
)
goto :eof

:migrate_rollback
if "%~2"=="" (
    echo ? Target migration required
    echo Usage: migrate.bat rollback ^<MigrationName^>
    exit /b 1
)

set "TARGET=%~2"
echo ??  Rolling back to: %TARGET%
echo WARNING: This will delete data! Make sure you have a backup!
set /p "CONFIRM=Are you sure? (yes/no): "

if /i not "%CONFIRM%"=="yes" (
    echo ? Rollback cancelled
    exit /b 0
)

cd "%INFRA_PROJECT%"
dotnet ef database update "%TARGET%" --startup-project "%API_PROJECT%"

if %ERRORLEVEL% EQU 0 (
    echo ? Rollback completed
) else (
    echo ? Rollback failed
    exit /b 1
)
goto :eof

:migrate_status
echo ?? Migration Status
cd "%INFRA_PROJECT%"

echo.
echo Applied Migrations:
dotnet ef migrations list --startup-project "%API_PROJECT%" --no-build 2>nul | findstr "Applied"

echo.
echo Pending Migrations:
dotnet ef migrations list --startup-project "%API_PROJECT%" --no-build 2>nul | findstr "Pending"
goto :eof

:migrate_script
echo ?? Generating SQL script for pending migrations

set "OUTPUT_FILE=%SCRIPT_DIR%..\..\migration-script.sql"

cd "%INFRA_PROJECT%"
dotnet ef migrations script --startup-project "%API_PROJECT%" --idempotent --output "%OUTPUT_FILE%"

if %ERRORLEVEL% EQU 0 (
    echo ? SQL script generated!
    echo ?? File: %OUTPUT_FILE%
) else (
    echo ? Failed to generate script
    exit /b 1
)
goto :eof

:migrate_remove
echo ??  Removing last migration...
echo WARNING: This will delete the last migration file!
set /p "CONFIRM=Are you sure? (yes/no): "

if /i not "%CONFIRM%"=="yes" (
    echo ? Operation cancelled
    exit /b 0
)

cd "%INFRA_PROJECT%"
dotnet ef migrations remove --startup-project "%API_PROJECT%"

if %ERRORLEVEL% EQU 0 (
    echo ? Migration removed
) else (
    echo ? Failed to remove migration
    exit /b 1
)
goto :eof

endlocal
