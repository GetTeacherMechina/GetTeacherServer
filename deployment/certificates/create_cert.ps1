param(
    [Parameter(Mandatory=$true)]
    [string]$Ip
)

# Variables
$pfxPassword = "GetTeacherCertificate"   # Password for the .pfx file
$certDir = "."    # Directory to store the temporary and final certificate files
$pfxFile = "$certDir/certificate_$ip.pfx"
$privateKeyFile = "$certDir/private.key"
$certFile = "$certDir/certificate.crt"

if (Test-Path $pfxFile) {
	Write-Host "Certificate for IP $Ip already exists." -ForegroundColor Green
	exit 0
}

# Ensure OpenSSL is installed
if (-not (Get-Command "openssl" -ErrorAction SilentlyContinue)) {
    Write-Error "OpenSSL is not installed. Please install it to continue."
    exit 1
}

# Create certificates directory if it doesn't exist
if (-not (Test-Path $certDir)) {
    New-Item -ItemType Directory -Path $certDir | Out-Null
}

Write-Host "Generating a private key and self-signed certificate..."
openssl req -x509 -nodes -days 365 -newkey rsa:2048 `
    -keyout $privateKeyFile -out $certFile `
    -subj "/CN=$Ip" `
    -addext "subjectAltName=IP:$Ip" > $null 2>&1

Write-Host "Converting certificate to PFX format..."
openssl pkcs12 -export -out $pfxFile -inkey $privateKeyFile -in $certFile -password pass:$pfxPassword > $null 2>&1

# Remove intermediate files
Remove-Item -Force $privateKeyFile, $certFile

# Output only the path to the .pfx file
Write-Output $pfxFile
