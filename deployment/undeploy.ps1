# Function to check the exit code of the last command and exit if there's an error
function Check-ExitCode {
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: $($args[0])"
		Pop-Location
    }
}

# Apply Kubernetes deployment
Write-Host "Deleting Kubernetes deployment..."
Push-Location "./k8s"  # Change to the k8s directory
./delete_deployment.ps1
Check-ExitCode "Deleting the Kubernetes deployment"
Pop-Location

# Add Kubernetes HTTPS certificate secret
Write-Host "Deleting Kubernetes HTTPS certificate secret..."
Push-Location "./secrets"  # Change to the secrets directory
./delete_cert_secret.ps1
Check-ExitCode "Deleting the HTTPS certificate secret"
Pop-Location

Write-Host "Deleting Kubernetes HTTPS certificate secret..."
Push-Location "./certificates"  # Change to the secrets directory
./delete_cert.ps1
Check-ExitCode "Deleting the HTTPS certificate"
Pop-Location

Write-Host "Deleting Kubernetes namespace..."
kubectl delete namespace getteacher-app
