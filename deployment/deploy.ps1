# Function to check the exit code of the last command and exit if there's an error
function Check-ExitCode {
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: $($args[0])"
        exit 1
    }
}

Write-Host "Creating Kubernetes namespace..."
kubectl create namespace getteacher-app

# Generate HTTPS certificate
Write-Host "Generating an HTTPS certificate..."
Set-Location -Path "./certificates"  # Change to the certificates directory
./create_cert.ps1
Check-ExitCode "Generating the HTTPS certificate"
Set-Location -Path "../"  # Change back to the main directory

# Add Kubernetes HTTPS certificate secret
Write-Host "Adding Kubernetes HTTPS certificate secret..."
Set-Location -Path "./secrets"  # Change to the secrets directory
./create_cert_secret.ps1
Check-ExitCode "Generating the HTTPS certificate secret"
Set-Location -Path "../"  # Change back to the main directory

# Apply Kubernetes deployment
Write-Host "Applying Kubernetes deployment..."
Set-Location -Path "./k8s"  # Change to the k8s directory
./create_deployment.ps1
Check-ExitCode "Applying the Kubernetes deployment"
Set-Location -Path "../"  # Change back to the main directory
