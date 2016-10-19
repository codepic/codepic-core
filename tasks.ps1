function Nginx
{
    Stop-Process -Name nginx -Force -ErrorAction SilentlyContinue
    cd ut*
    cd ng*
    Start-Process -FilePath ./nginx.exe -ArgumentList '-c ../../conf/nginx.conf'
}