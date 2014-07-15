@ECHO OFF
cls
echo -----------------------------
echo ** Initialize
SETLOCAL ENABLEEXTENSIONS
REM Initialize Constants
REM SET TSVN_INFO_FILE=./TSVN_INFO.tmp
SET TSVN_INFO_FILE=%4
REM Initialize script arguments
SET workDir=%1
SET template=%2
SET target=%3
REM Goto main entry
GOTO MAIN
echo =============================
echo -----------------------------
::Main entry
:MAIN
pushd %workDir%
SET workDir=./
REM ������
IF %workDir%=="" GOTO ARGUMENT_ERROR
IF %template%=="" GOTO ARGUMENT_ERROR
IF %target%=="" GOTO ARGUMENT_ERROR
echo ��ѯע���
echo TSVN_INFO_FILE %TSVN_INFO_FILE%
reg query HKEY_LOCAL_MACHINE\SOFTWARE\TortoiseSVN /v Directory > %TSVN_INFO_FILE% 2>NUL
REM ���� TSVN ·��
FOR /F "tokens=*" %%i IN (%TSVN_INFO_FILE%) DO (
  ECHO %%i | find "Directory" > NUL
  IF %ERRORLEVEL% == 0 (
    ECHO %%i > %TSVN_INFO_FILE%
    echo ----------------****
    ECHO %%i
  )
)

SET /P TSVN_PATH= < %TSVN_INFO_FILE%
SET /P TSVN_PATH= < %TSVN_PATH%

echo TSVN_PATH %TSVN_PATH%
SET TSVN_PATH=%TSVN_PATH:~23,-1%

echo TSVN_PATH %TSVN_PATH%


echo ���� TSVN �滻ģ��
echo ERRORLEVEL %ERRORLEVEL%
echo TSVN_PATH %TSVN_PATH%
IF NOT %ERRORLEVEL% == 0 GOTO UNKNOW_ERROR
"%TSVN_PATH%bin/SubWCRev.exe" %workDir% %template% %target% >NUL
IF NOT %ERRORLEVEL% == 0 GOTO UNKNOW_ERROR
GOTO SUCESSED
echo =============================
echo -----------------------------
echo ** Error handlers
:ARGUMENT_ERROR
ECHO ����Ĳ�����Ч��
GOTO FAIL
:NOT_FOUND_TSVN
echo ��ѯTortoiseSVN �İ�װ��Ϣʧ�ܡ�
GOTO FAIL
:UNKNOW_ERROR
ECHO ���ɳ�����Ϣ����δ֪����
:FAIL
echo =============================
echo -----------------------------
echo ** Program exit
:FAIL
DEL /Q %TSVN_INFO_FILE% 2>NUL
echo ���ɳ�����Ϣʧ�ܡ�
popd
EXIT 1
:SUCESSED
DEL /Q %TSVN_INFO_FILE% 2>NUL
ECHO ���ɳ�����Ϣ�ɹ���
popd
EXIT 0
::=============================