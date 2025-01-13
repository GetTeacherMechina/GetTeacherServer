param(
    [Parameter(Mandatory=$true)]
    [string]$Ip
)

# Paths
$certPath = "./certificate_$Ip.crt"
$keyPath = "./private_$Ip.key"

Write-Host "Stopping Caddy server..." -ForegroundColor Green
caddy stop --config .\Caddyfile

Write-Host "Cleaning up temporary files..." -ForegroundColor Yellow
Remove-Item -Path $certPath -ErrorAction SilentlyContinue
Remove-Item -Path $keyPath -ErrorAction SilentlyContinue
Remove-Item -Path .\Caddyfile -ErrorAction SilentlyContinue