cd %1\bin\Release
call nupack %1 %2
cd %~dp0
call push %1