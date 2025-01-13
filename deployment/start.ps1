param(
    [Parameter(Mandatory=$true)]
    [string]$Ip
)

# Check if running as Administrator
if (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "This script must be run as an administrator!" -ForegroundColor Red
    Exit 1
}

# Function to check the exit code of the last command and exit if there's an error
function Check-ExitCode {
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: $($args[0])"
        exit 1
    }
}

# List to track all started processes
$BackgroundProcesses = @()

# Generate HTTPS certificate
Write-Host "Generating an HTTPS certificate..."
Push-Location ./certificates
./create_cert.ps1 $Ip
Check-ExitCode "Generating the HTTPS certificate"
Pop-Location

# Apply Kubernetes deployment
Write-Host "Applying Kubernetes deployment..."
Push-Location ./kubernetes
./create_deployment.ps1
Check-ExitCode "Applying the Kubernetes deployment"
Pop-Location

# Start Minikube Tunnel
Write-Host "Starting Minikube Tunnel in background..."
$minikubeTunnel = Start-Process -FilePath "minikube" -ArgumentList "tunnel" -NoNewWindow -PassThru
Write-Host "Started MiniKube tunnel: $($minikubeTunnel.Id)"
$BackgroundProcesses += $minikubeTunnel

# Wait for the Tunnel to Be Ready
Write-Host "Waiting for Minikube Tunnel to establish..."
Start-Sleep -Seconds 5

# Expose service URL via Minikube
Write-Host "Exposing service URL via Minikube..."
$tmpFolder = "$env:TEMP"
$outputFile = Join-Path $tmpFolder "minikube_service_output.txt"
$minikubeService = Start-Process -FilePath "minikube" -ArgumentList "service getteacher-service --url -n getteacher-app" -NoNewWindow -RedirectStandardOutput $outputFile -PassThru
Write-Host "Started MiniKube service: $($minikubeService.Id)"
$BackgroundProcesses += $minikubeService

# Wait for Minikube service to expose the port
Write-Host "Waiting for Minikube service to expose the port..."
Start-Sleep -Seconds 5

# Read the output file and extract the port
if (Test-Path $outputFile) {
    $minikubeOutput = Get-Content $outputFile
    if ($minikubeOutput -match "http://127\.0\.0\.1:(\d+)") {
        $port = $Matches[1]
        Write-Host "Extracted Port: $port"

        # Start Caddy
        Write-Host "Starting Caddy server..."
        Push-Location ./caddy
        $caddyProcess = Start-Process -FilePath "powershell.exe" -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File ./start_caddy.ps1 $Ip $port" -NoNewWindow -PassThru
        Write-Host "Started Caddy: $($caddyProcess.Id)"
        Check-ExitCode "Running Caddy"
        Pop-Location
    } else {
        Write-Host "Error: Could not extract port from Minikube output!" -ForegroundColor Red
    }
} else {
    Write-Host "Error: Minikube output file not found!" -ForegroundColor Red
}

# Wait for Caddy
Start-Sleep -Seconds 5

# Wait for all processes to complete
Write-Host "Deployment is running. Press Ctrl+C to terminate."
Wait-Process -Id ($BackgroundProcesses | ForEach-Object { $_.Id })
