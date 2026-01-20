@echo off
REM =========================================
REM Build Release and Obfuscate
REM =========================================

echo.
echo ========================================
echo  BUILD AND OBFUSCATE MY-SHOP
echo ========================================
echo.

cd ..\..

echo [INFO] Building App (Release)...
dotnet build -c Release src\my-shop.App\my-shop.App.csproj

echo [INFO] Building API (Release)...
dotnet build -c Release src\my-shop.API\my-shop.API.csproj

if errorlevel 1 (
    echo [ERROR] Build failed!
    cd scripts\obfuscation
    pause
    exit /b 1
)

echo.
echo [SUCCESS] All projects built successfully!
echo.

cd scripts\obfuscation

echo [INFO] Starting obfuscation...
echo.

call obfuscate.bat

exit /b %errorlevel%
