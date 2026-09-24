@echo off
rem ===========================================================================
rem  Serial Port Device Finder - one-click release packaging
rem  Double-click this file to build the release zip (+ installer if available).
rem  All user-visible messages live in build-release.ps1; this launcher is kept
rem  pure ASCII on purpose so it can never be garbled by code page issues.
rem ===========================================================================
chcp 65001 >nul
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build-release.ps1" %*
echo.
pause
