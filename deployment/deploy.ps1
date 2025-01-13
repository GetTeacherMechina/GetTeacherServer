param(
    [Parameter(Mandatory=$true)]
    [string]$Ip
)

# Check if running as Administrator
if (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "This script must be run as an administrator!" -ForegroundColor Red
    Exit 1
}

# Register an event to handle Ctrl+C
function Cleanup {
    Write-Host "`nCtrl+C detected! Running stop script..." -ForegroundColor Yellow
    try {
        ./stop.ps1 $Ip
    } catch {
        Write-Host "Failed to run stop script: $_" -ForegroundColor Red
    } finally {
        Exit 1
    }
}

try {
    Write-Host "Running start script..."
	
	Register-EngineEvent -SourceIdentifier ConsoleControlC -Action {
		Cleanup
	}

    ./start.ps1 $Ip
} finally {
    Cleanup
}
