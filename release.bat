echo off
set tmppath=%path%
set path=%VS110COMNTOOLS%..\IDE;%path%
set path=%VS110COMNTOOLS%;%path%
@rem cmd


devenv ECM.wpf.release.sln /rebuild RELEASE
if not errorlevel 0 goto error

set path=%VS100COMNTOOLS%..\IDE;%tmppath%
set path=%VS100COMNTOOLS%;%path%

devenv Setup.wpf.sln /rebuild RELEASE
if not errorlevel 0 goto error

goto end
:error

echo ���ִ���δ����ȷ���
goto end2
:end
echo ��ȷ���
:end2
pause
echo on