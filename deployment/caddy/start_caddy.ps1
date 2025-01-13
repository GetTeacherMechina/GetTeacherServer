param(
    [Parameter(Mandatory=$true)]
    [string]$Ip,
    [Parameter(Mandatory=$true)]
    [string]$Port
)

# Paths
$pfxPath = "../certificates/certificate_$Ip.pfx"
$certPath = "./certificate_$Ip.crt"
$keyPath = "./private_$Ip.key"
$password = "GetTeacherCertificate"

# Extract Certificate and Key from .pfx
Write-Host "Extracting certificate and key from .pfx..." -ForegroundColor Green
& openssl pkcs12 -in $pfxPath -out $certPath -nodes -nokeys -passin pass:$password
& openssl pkcs12 -in $pfxPath -out $keyPath -nodes -nocerts -passin pass:$password

# Create Caddyfile
Write-Host "Generating Caddyfile..." -ForegroundColor Green
@"
{
    debug
}

https://$($Ip):443 {
    tls $certPath $keyPath
    reverse_proxy http://127.0.0.1:$($Port)
}
"@ | Set-Content -Path .\Caddyfile

# Run Caddy
Write-Host "Starting Caddy server..." -ForegroundColor Green
caddy start --config .\Caddyfile
