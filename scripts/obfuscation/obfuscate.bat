@echo off
REM ===============================================
REM Script obfuscate my-shop - Core Business Layers
REM ===============================================

echo.
echo ========================================
echo  MY-SHOP OBFUSCATION SCRIPT (FIXED)
echo ========================================
echo.

set CONFUSER_PATH=.\tools\ConfuserEx\Confuser.CLI.exe
set APP_RELEASE_DIR=..\..\Release\my-shop.App
set API_RELEASE_DIR=..\..\Release\my-shop.API
set LAYERS_DIR=..\..\Release\Layers-Original
set OUTPUT_DIR=..\..\Release\Layers-Obfuscated

if not exist "%CONFUSER_PATH%" (
    echo [ERROR] ConfuserEx not found!
    pause
    exit /b 1
)

if not exist "%APP_RELEASE_DIR%\my-shop.App.dll" (
    echo [ERROR] App Release build not found!
    pause
    exit /b 1
)

if not exist "%API_RELEASE_DIR%\my-shop.API.dll" (
    echo [ERROR] API Release build not found!
    pause
    exit /b 1
)

echo [INFO] Preparing layers and dependencies...
if not exist "%LAYERS_DIR%" mkdir "%LAYERS_DIR%"
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

echo [INFO] Copying core layers and API...
copy /Y "..\..\src\my-shop.Application\bin\Release\net8.0\my-shop.Application.dll" "%LAYERS_DIR%\" >nul
copy /Y "..\..\src\my-shop.Domain\bin\Release\net8.0\my-shop.Domain.dll" "%LAYERS_DIR%\" >nul
copy /Y "..\..\src\my-shop.Infrastructure\bin\Release\net8.0\my-shop.Infrastructure.dll" "%LAYERS_DIR%\" >nul
copy /Y "..\..\src\my-shop.API\bin\Release\net8.0\my-shop.API.dll" "%LAYERS_DIR%\" >nul

echo [INFO] Copying dependencies for resolution...
copy /Y "%APP_RELEASE_DIR%\*.dll" "%LAYERS_DIR%\" >nul
copy /Y "%API_RELEASE_DIR%\*.dll" "%LAYERS_DIR%\" >nul
copy /Y "C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\8.0.22\*.dll" "%LAYERS_DIR%\" >nul
copy /Y "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\8.0.22\*.dll" "%LAYERS_DIR%\" >nul

set DOTNET_CORE_PATH=C:\Program Files\dotnet\shared\Microsoft.NETCore.App\8.0.22
set DOTNET_ASPNET_PATH=C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App\8.0.22

echo.
echo [INFO] Starting obfuscation process...
"%CONFUSER_PATH%" -n ConfuserEx.crproj -probe "%DOTNET_CORE_PATH%" -probe "%DOTNET_ASPNET_PATH%" -probe "%LAYERS_DIR%"

if errorlevel 1 (
    echo.
    echo [ERROR] Obfuscation failed!
    pause
    exit /b 1
)

echo.
echo [INFO] Applying obfuscated DLLs to App and API directories...
REM Update App
copy /Y "%OUTPUT_DIR%\my-shop.Application.dll" "%APP_RELEASE_DIR%\" >nul
copy /Y "%OUTPUT_DIR%\my-shop.Domain.dll" "%APP_RELEASE_DIR%\" >nul
copy /Y "%OUTPUT_DIR%\my-shop.Infrastructure.dll" "%APP_RELEASE_DIR%\" >nul

REM Update API
copy /Y "%OUTPUT_DIR%\my-shop.Application.dll" "%API_RELEASE_DIR%\" >nul
copy /Y "%OUTPUT_DIR%\my-shop.Domain.dll" "%API_RELEASE_DIR%\" >nul
copy /Y "%OUTPUT_DIR%\my-shop.Infrastructure.dll" "%API_RELEASE_DIR%\" >nul
copy /Y "%OUTPUT_DIR%\my-shop.API.dll" "%API_RELEASE_DIR%\" >nul

echo.
echo ========================================
echo  SUCCESS! App and API layers are obfuscated.
echo ========================================
echo.

echo [INFO] Cleaning up temporary files...
rd /S /Q "%LAYERS_DIR%"
rd /S /Q "%OUTPUT_DIR%"

pause
