function Nginx
{
    Stop-Process -Name nginx -Force -ErrorAction SilentlyContinue
    cd srv
    cd nginx
    Start-Process -FilePath ./nginx.exe -ArgumentList '-c ../../conf/nginx.conf'
}