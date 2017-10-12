%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe bin\Debug\TLJ_MySqlService.exe
Net Start TLJ_MySqlService
sc config TLJ_MySqlService start= auto

pause