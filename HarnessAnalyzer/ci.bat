@echo off

set myDir=%~dp0

if not exist run-yaml.ps1  (
 echo Downloading script file ...
 powershell -Command "Invoke-WebRequest -OutFile '%myDir%\run-yaml.ps1' -Uri 'https://ulm-dev.zuken.com/Team-Erlangen/scripts/raw/branch/trunk/build/run-yaml.ps1' -ErrorAction SilentlyContinue"
 if not exist "%myDir%\run-yaml.ps1" (
    echo Download failed, maybe you are not logged in ?
    pause
    exit -1
 )


 REM errorlevel = 1 when found, if not found errorlevel = 0
 find "<title>Sign in to your account</title>" "%myDir%\run-yaml.ps1"
 if errorlevel 1 goto start

 cls
 echo Download failed: you not logged in! Aborting... 
 del "%myDir%\run-yaml.ps1"
 pause
 exit -1
)

:start
powershell -noprofile -executionpolicy unrestricted -File "%myDir%\run-yaml.ps1" -Path "%myDir%\.ci.yml"
if not errorlevel 0 (
 pause
 goto end
)

if exist "%myDir%\run-yaml.ps1" (
 del "%myDir%\run-yaml.ps1"
)

goto end
:end

