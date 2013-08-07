@ECHO OFF
setlocal

IF "%2"=="" GOTO Usage
GOTO Run

:Usage
echo # 
echo # Invalid Argument
echo # Usage: %0 [KilnPath] [EtcPath]
echo # Example: %0 C:\kiln \Nbt\Nbt\NbtSchedService\bin
echo #   [KilnPath]: The local absolute root of Kiln (e.g., "C:\kiln" )
echo #   [EtcPath]: Relative directory in which to make etc dir and links
echo # 
GOTO End

:Run

echo %1%2
cd %1%2
mkdir etc
cd etc
del CswDbCfgInfo.xml
del CswSetupVbls.xml
del CswSetupVariables.json
mklink CswDbCfgInfo.xml %1\Common\CswConfigUX\bin\etc\CswDbCfgInfo.xml
mklink CswSetupVariables.json %1\Common\CswConfigUX\bin\etc\CswSetupVariables.json

:End
