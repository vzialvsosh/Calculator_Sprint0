@echo off
setlocal EnableExtensions DisableDelayedExpansion

rem Run from the repository root, even when opened by double-click.
pushd "%~dp0"
if errorlevel 1 (
    echo Cannot open the script directory.
    pause
    exit /b 1
)

where dotnet >nul 2>&1
if errorlevel 1 (
    echo dotnet was not found. Install the .NET 10 SDK.
    goto :failed
)

if not exist "Backend\Backend.csproj" (
    echo Backend\Backend.csproj was not found.
    echo Put start.bat next to the Backend and Frontend folders.
    goto :failed
)

rem Support either the UI contents or the whole UI folder in Frontend.
set "CALCULATOR_UI_PROJECT=Frontend\UI.csproj"
if not exist "%CALCULATOR_UI_PROJECT%" set "CALCULATOR_UI_PROJECT=Frontend\UI\UI.csproj"
if not exist "%CALCULATOR_UI_PROJECT%" (
    echo UI.csproj was not found in Frontend or Frontend\UI.
    goto :failed
)

rem Separate windows keep build errors and server logs visible.
start "Calculator Backend" cmd /d /k dotnet run --project "Backend\Backend.csproj" --no-launch-profile -- --urls "http://localhost:5000"
start "Calculator UI" cmd /d /k dotnet run --project "%CALCULATOR_UI_PROJECT%" --no-launch-profile

popd
exit /b 0

:failed
popd
pause
exit /b 1
