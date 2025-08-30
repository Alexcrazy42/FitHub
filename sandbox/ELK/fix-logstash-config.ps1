$confPath = Join-Path $PSScriptRoot "logstash\pipeline\logstash.conf"

# Проверяем, существует ли файл
if (-Not (Test-Path $confPath)) {
    Write-Error "Файл $confPath не найден. Убедитесь, что вы запускаете скрипт из правильной папки."
    Read-Host "Нажмите Enter для выхода..."
    exit 1
}

# Читаем содержимое
$content = Get-Content $confPath -Raw

# Перезаписываем в UTF-8 без BOM
[IO.File]::WriteAllText($confPath, $content, [Text.UTF8Encoding]::new($false))

Write-Host "✅ Файл $confPath успешно пересохранён в UTF-8 без BOM." -ForegroundColor Green

# Проверка первых байтов (опционально)
$bytes = [System.IO.File]::ReadAllBytes($confPath)
Write-Host "Первые 3 байта: " -NoNewline
for ($i = 0; $i -lt 3 -and $i -lt $bytes.Length; $i++) {
    Write-Host ("{0:X2} " -f $bytes[$i]) -NoNewline
}
Write-Host ""

if ($bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
    Write-Warning "⚠️ BOM всё ещё присутствует!"
} else {
    Write-Host "✅ BOM отсутствует — всё в порядке." -ForegroundColor Green
}

$res = Read-Host "Нажмите Enter для завершения"