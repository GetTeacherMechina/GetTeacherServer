Write-Host "Creating kubectl certificate secret..."
kubectl create secret generic https-certificate --from-file=certificate.pfx=../certificates/certificate.pfx --namespace=getteacher-app
exit 0 # For a case where the secret already exists