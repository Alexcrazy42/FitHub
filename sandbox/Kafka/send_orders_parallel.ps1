$requests = 10000
$url = "http://localhost:5206/order"

1..$requests | ForEach-Object -Parallel {
    try {
        Invoke-RestMethod -Method POST -Uri $using:url
        Write-Output "Request $_ succeeded"
    } catch {
        Write-Output "Request $_ failed: $_"
    }
} -ThrottleLimit 10000