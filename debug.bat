echo off
set path=%VS100COMNTOOLS%..\IDE;%path%
set path=%VS100COMNTOOLS%;%path%
devenv ECM.wpf.sln /build Debug

@rem devenv Setup.wpf.sln /build Debug

echo on
pause