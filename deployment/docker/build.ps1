param(
    [string]$ImageName = "getteacher"
)

Push-Location ../..
docker build -t "$($ImageName):latest" -f deployment/docker/Dockerfile .
Pop-Location 
