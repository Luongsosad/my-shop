@echo off
REM PostgreSQL Database Restore Script for Windows
REM Usage: restore-database.bat <backup-file>

setlocal enabledelayedexpansion

if "%~1"=="" (
    echo ? ERROR: No backup file specified
    echo Usage: restore-database.bat ^<backup-file^>
    echo.
    echo Available backups:
    dir /B /O-D "..\..\backups\*.sql" 2>nul
    exit /b 1
)

set "BACKUP_FILE=%~1"

if not exist "%BACKUP_FILE%" (
    echo ? ERROR: Backup file not found: %BACKUP_FILE%
    exit /b 1
)

REM Load environment variables
if exist "..\..\env" (
    for /f "usebackq tokens=1,2 delims==" %%a in ("..\..\env") do (
        if not "%%a"=="" if not "%%a"=="#" (
            set "%%a=%%b"
        )
    )
)

if "%DATABASE_URL%"=="" (
    echo ? ERROR: DATABASE_URL not found in environment
    exit /b 1
)

REM Parse DATABASE_URL
for /f "tokens=1,2,3,4,5 delims=:/@?" %%a in ("%DATABASE_URL%") do (
    set "DB_USER=%%b"
    set "DB_PASSWORD=%%c"
    set "DB_HOST=%%d"
    set "DB_PORT=%%e"
    set "DB_NAME=%%f"
)

set "DB_USER=%DB_USER:~2%"
set "DB_NAME=%DB_NAME:~1%"

echo ??  WARNING: This will DROP and RECREATE all tables in the database!
echo ?? Host: %DB_HOST%:%DB_PORT%
echo ???  Database: %DB_NAME%
echo ?? Backup file: %BACKUP_FILE%
echo.
set /p "CONFIRM=Are you sure you want to continue? (yes/no): "

if /i not "%CONFIRM%"=="yes" (
    echo ? Restore cancelled
    exit /b 0
)

REM Set password
set "PGPASSWORD=%DB_PASSWORD%"

echo ?? Restoring database...

REM Restore database
psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -f "%BACKUP_FILE%"

if %ERRORLEVEL% EQU 0 (
    echo ? Database restored successfully!
) else (
    echo ? Restore failed!
    exit /b 1
)

REM Unset password
set "PGPASSWORD="

echo.
echo ? Restore completed!
echo ?? Verify the restore with:
echo    cd src\my-shop.API
echo    dotnet run

endlocal
