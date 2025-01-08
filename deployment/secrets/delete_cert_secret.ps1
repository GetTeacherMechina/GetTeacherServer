Write-Host "Deleting kubectl certificate secret..."
kubectl delete secret https-certificate --namespace=getteacher-app
