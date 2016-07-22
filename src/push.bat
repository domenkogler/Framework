FOR /D %%D IN ("C:\Nuget\%1\*") DO rmdir "%%D" /s /q
FOR /D %%D IN ("C:\Nuget\%1\*") DO rmdir c:\nuget\%1.%%~nxD\ /s /q
call nuget push %1\bin\Release\*.nupkg -s C:\\Nuget
FOR /D %%D IN (C:\Nuget\%1\*) DO xcopy c:\nuget\%1\%%~nxD\* c:\nuget\%1.%%~nxD\ /s /y