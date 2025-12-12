@echo off
REM PostgreSQL Database Backup Script for Windows
REM Usage: backup-database.bat [backup-name]

setlocal enabledelayedexpansion

REM Load environment variables from .env file
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
REM Format: postgresql://user:password@host:port/database
for /f "tokens=1,2,3,4,5 delims=:/@?" %%a in ("%DATABASE_URL%") do (
    set "DB_USER=%%b"
    set "DB_PASSWORD=%%c"
    set "DB_HOST=%%d"
    set "DB_PORT=%%e"
    set "DB_NAME=%%f"
)

REM Remove leading slashes
set "DB_USER=%DB_USER:~2%"
set "DB_NAME=%DB_NAME:~1%"

REM Create backups directory
if not exist "..\..\backups" mkdir "..\..\backups"

REM Generate backup filename
set "TIMESTAMP=%date:~10,4%%date:~4,2%%date:~7,2%_%time:~0,2%%time:~3,2%%time:~6,2%"
set "TIMESTAMP=%TIMESTAMP: =0%"
set "BACKUP_NAME=%~1"
if "%BACKUP_NAME%"=="" set "BACKUP_NAME=myshop"
set "BACKUP_FILE=..\..\backups\%BACKUP_NAME%_%TIMESTAMP%.sql"

echo ?? Starting database backup...
echo ?? Host: %DB_HOST%:%DB_PORT%
echo ???  Database: %DB_NAME%
echo ?? Backup file: %BACKUP_FILE%

REM Set password for pg_dump
set "PGPASSWORD=%DB_PASSWORD%"

REM Perform backup with pg_dump
pg_dump -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% --no-owner --no-acl --clean --if-exists -F p -f "%BACKUP_FILE%"

if %ERRORLEVEL% EQU 0 (
    echo ? Backup completed successfully!
    echo ?? File: %BACKUP_FILE%
    
    REM Get file size
    for %%A in ("%BACKUP_FILE%") do set "BACKUP_SIZE=%%~zA"
    echo ?? Size: !BACKUP_SIZE! bytes
    
    REM List recent backups
    echo.
    echo ?? Recent backups:
    dir /B /O-D "..\..\backups\*.sql" | findstr /N "^" | findstr /B "1: 2: 3: 4: 5:"
) else (
    echo ? Backup failed!
    if exist "%BACKUP_FILE%" del "%BACKUP_FILE%"
    exit /b 1
)

REM Clean up old backups (keep last 10)
echo.
echo ?? Cleaning up old backups (keeping last 10)...
for /f "skip=10 delims=" %%F in ('dir /B /O-D "..\..\backups\*.sql"') do (
    del "..\..\backups\%%F"
    echo Deleted: %%F
)
echo ? Cleanup completed!

REM Unset password
set "PGPASSWORD="
endlocal
