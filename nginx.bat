powershell -Command "Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope CurrentUser"
powershell -Command "& { . .\tasks.ps1;Nginx; }"