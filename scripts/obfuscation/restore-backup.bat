@echo off
REM =========================================
REM Restore files from backup
REM =========================================

echo.
echo ========================================
echo  RESTORE FROM BACKUP
echo ========================================
echo.

set BACKUP_ROOT=..\..\Backup

if not exist "%BACKUP_ROOT%" (
    echo [ERROR] No backup directory found!
    pause
    exit /b 1
)

echo Available backups:
echo.
dir /B /AD "%BACKUP_ROOT%"
echo.

set /p BACKUP_NAME="Enter backup folder name (or press Ctrl+C to cancel): "

set BACKUP_PATH=%BACKUP_ROOT%\%BACKUP_NAME%

if not exist "%BACKUP_PATH%" (
    echo [ERROR] Backup not found: %BACKUP_PATH%
    pause
    exit /b 1
)

set RESTORE_PATH=..\..\src\my-shop.App\bin\Release\net8.0-windows10.0.19041.0\win-x64

echo.
echo [WARNING] This will restore files to:
echo %RESTORE_PATH%
echo.
echo Press any key to continue or Ctrl+C to cancel...
pause >nul

echo.
echo [INFO] Restoring files...

xcopy /E /I /Y "%BACKUP_PATH%\*.*" "%RESTORE_PATH%\" >nul

if errorlevel 1 (
    echo [ERROR] Restore failed!
    pause
    exit /b 1
)

echo [SUCCESS] Files restored successfully!
echo.

pause
