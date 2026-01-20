@echo off
REM =========================================
REM Script to download and setup ConfuserEx
REM =========================================

echo.
echo ========================================
echo  CONFUSEREX SETUP SCRIPT
echo ========================================
echo.

set TOOLS_DIR=.\tools
set CONFUSER_DIR=%TOOLS_DIR%\ConfuserEx

echo [INFO] This script will help you set up ConfuserEx 2
echo.
echo Please follow these steps:
echo.
echo 1. Download ConfuserEx 2 from GitHub:
echo    https://github.com/mkaring/ConfuserEx/releases
echo.
echo 2. Look for the latest release (ConfuserEx 2.x)
echo.
echo 3. Download the ZIP file (e.g., ConfuserEx-2.0.0.zip)
echo.
echo 4. Extract the contents to: %CONFUSER_DIR%\
echo.
echo 5. Make sure Confuser.CLI.exe is at:
echo    %CONFUSER_DIR%\Confuser.CLI.exe
echo.

pause

REM Create tools directory
if not exist "%TOOLS_DIR%" (
    echo [INFO] Creating tools directory...
    mkdir "%TOOLS_DIR%"
)

if not exist "%CONFUSER_DIR%" (
    echo [INFO] Creating ConfuserEx directory...
    mkdir "%CONFUSER_DIR%"
)

echo.
echo [INFO] Directory structure created
echo.
echo Now please extract ConfuserEx files to:
echo %CD%\%CONFUSER_DIR%\
echo.
echo After extraction, run obfuscate.bat to start obfuscation
echo.

pause
