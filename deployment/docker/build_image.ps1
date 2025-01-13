param(
    [Parameter(Mandatory=$true)]
    [string]$CertIp
)

Push-Location ../certificates
./create_cert.ps1 $CertIp
Pop-Location

Push-Location ../..
docker build --build-arg CERT_IP=$CertIp -t "getteacher:latest" -f deployment/docker/Dockerfile .
minikube image load getteacher:latest
Pop-Location
