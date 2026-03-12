# Video Upload — TODO

## 1. Poster generation не работает

`VideoService.ProcessAsync` содержит закомментированный код создания постера (`FFMpeg.SnapshotAsync`).
Постер не сканируется по неизвестной причине — надо разобраться и включить обратно.

Сейчас `video.MarkReady` вызывается с пустым `posterS3Key = ""`.

**Файлы:** `Application/Videos/VideoService.cs:227`