@echo off

powershell -ExecutionPolicy Bypass -command "%~dp0build.ps1" %*
exit /b %ERRORLEVEL%
