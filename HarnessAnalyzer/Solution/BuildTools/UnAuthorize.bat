@echo off

echo Initializing ...

REM -------- Download script files --------

if not exist vdUnAuthorize.ps1 (
 powershell -Command "Invoke-WebRequest -OutFile vdUnAuthorize.ps1 -Uri https://ulm-dev.zuken.com/Team-Erlangen/scripts/raw/branch/trunk/vdAuthorize/vdUnAuthorize.ps1"
 if %ERRORLEVEL% NEQ 0 (
  pause
  exit -1
 )
)

REM -------- Download script files --------

powershell -executionpolicy unrestricted -File vdUnAuthorize.ps1 -License "%1"