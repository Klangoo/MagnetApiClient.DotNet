@echo off
rem ========================================================
rem               Build Klangoo.Client
rem ========================================================

rem dotnet build for the frameworks: net40;net45;net46;net461;netcoreapp2.0;netstandard2.0
echo dotnet build
dotnet build Klangoo.Client.csproj -c Release

rem MsBuild for the framework net20
echo msbuild for net20
SET MSBUILD="C:\Windows\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe"
%MSBUILD% MS.Klangoo.Client.csproj /t:Build /p:Configuration=Release_net20 /p:TargetFrameworkVersion=v2.0

@echo off