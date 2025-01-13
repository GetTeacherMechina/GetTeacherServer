param(
    [Parameter(Mandatory=$true)]
    [string]$Ip
)

# Function to check the exit code of the last command and exit if there's an error
function Check-ExitCode {
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: $($args[0])"
		Pop-Location
    }
}

# Stop caddy
Write-Host "Stopping caddy..."
Push-Location "./caddy"
./stop_caddy.ps1 $Ip
Check-ExitCode "Stopping caddy"
Pop-Location

# Apply Kubernetes deployment
Write-Host "Deleting Kubernetes deployment..."
Push-Location "./kubernetes"  # Change to the k8s directory
./delete_deployment.ps1
Check-ExitCode "Deleting the Kubernetes deployment"
Pop-Location
