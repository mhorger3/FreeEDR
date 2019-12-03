if not exist "C:\windows\config.xml" (
copy /z /y "\\freeedr.lab\SYSVOL\freeedr.lab\Sysmon\config.xml" "C:\windows\"
)

sc query "Sysmon" | Find "RUNNING"
If "%ERRORLEVEL%" EQU "1" (
goto startsysmon
)
:startsysmon
net start Sysmon

If "%ERRORLEVEL%" EQU "1" (
goto installsysmon
)
:installsysmon
"\\freeedr.lab\SYSVOL\freeedr.lab\Sysmon\sysmon.exe" /accepteula -i c:\windows\config.xml