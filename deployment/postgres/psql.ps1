param(
    [Parameter(Mandatory=$true)]
    [string]$Ip
)

Write-Host "Password is 'GetTeacherDatabase'"
docker run -it --rm postgres psql -h $Ip -U GetTeacher -d postgres -p 5432