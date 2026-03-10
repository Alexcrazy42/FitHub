# Video Upload & Processing Flow

End-to-end description of how videos are uploaded, processed, and played back in FitHub.

---

## Architecture Overview

```
Frontend                Backend (Host)          RabbitMQ        Backend (HostJobs)
   │                         │                     │                    │
   │─── POST init-upload ───>│                     │                    │
   │<── { videoId, putUrl } ─│                     │                    │
   │                         │                     │                    │
   │─── PUT file to S3 ─────────────────> [Minio]  │                    │
   │                         │                     │                    │
   │─── POST confirm-upload ─>│                    │                    │
   │<── 202 Accepted ────────│                     │                    │
   │                         │─── enqueue ────────>│                    │
   │                         │                     │──> VideoEncoding   │
   │                         │                     │    Consumer ──────>│
   │                         │<─── POST /process ──────────────────────│
   │                         │                     │                    │
   │  (polls every 8s)       │  [FFmpeg encodes]   │                    │
   │─── GET /videos ────────>│                     │                    │
   │<── status: Ready ───────│                     │                    │
   │                         │                     │                    │
   │─── GET /resolutions ───>│                     │                    │
   │<── [{ url, quality }] ──│                     │                    │
   │                         │                     │                    │
   │  [VideoPlayer plays]    │                     │                    │
```

---

## Step 1 — Init Upload

**Frontend** calls `POST /api/v1/videos/init-upload`:

```json
{ "title": "Leg Day Workout", "fileExtension": "mp4" }
```

**Backend** (`VideoService.InitUploadAsync`):
1. Creates a `FileEntity` record in DB (status: `Pending`)
2. Creates a `Video` record in DB (status: `Pending`) linked to the file
3. Generates a **presigned S3 PUT URL** (15-minute expiry) for the original file key `videos/{videoId}/original.mp4`
4. Returns:

```json
{ "videoId": "video_...", "presignedPutUrl": "http://minio:9000/files/videos/..." }
```

Relevant files:
- `Web/V1/Videos/VideoController.cs` — `InitUpload` action
- `Application/Videos/VideoService.cs` — `InitUploadAsync`
- `Application/Files/S3FileService.cs` — `GetPresignedUrlAsync`

---

## Step 2 — Direct Upload to S3

**Frontend** uploads the file **directly to Minio** using the presigned PUT URL — the backend is not involved:

```ts
await axios.put(presignedPutUrl, file, {
    headers: { 'Content-Type': file.type },
    onUploadProgress: (e) => setProgress(Math.round(e.loaded / e.total * 100))
});
```

The file lands in Minio at `videos/{videoId}/original.{ext}`.

Relevant files:
- `frontend/src/api/videoApi.ts` — `uploadToS3`
- `frontend/src/pages/admin/videos/VideosAdminPage.tsx` — upload modal with progress bar

---

## Step 3 — Confirm Upload

**Frontend** calls `POST /api/v1/videos/{id}/confirm-upload` after the S3 upload completes.

**Backend** (`VideoService.ConfirmUploadAsync`):
1. Loads the `Video` (must be in `Pending` status)
2. Marks `FileEntity` as `Active`
3. Publishes a `VideoEncodingMessage` to RabbitMQ:
   - Exchange: `video.encoding` (direct)
   - Routing key: `video.encoding.process`
   - Payload: `{ "videoId": "video_..." }`
4. Returns `202 Accepted`

Relevant files:
- `Web/V1/Videos/VideoController.cs` — `ConfirmUpload` action
- `Application/Videos/VideoService.cs` — `ConfirmUploadAsync`
- `Application/Videos/VideoEncodingQueue.cs` — publishes message
- `Queue.Contracts/Videos/VideoEncodingMessage.cs` — message contract

---

## Step 4 — Async Encoding (HostJobs)

`HostJobs` is a separate Worker Service that consumes messages from RabbitMQ.

**`VideoEncodingConsumer`** receives the message and calls `VideoClient.ProcessAsync(videoId)`, which makes an HTTP POST to `Host` at `/api/v1/videos/{id}/process`.

**Backend** (`VideoService.ProcessAsync`):

1. Loads video, marks it as `Processing`
2. Downloads original file from S3 to a temp directory:
   `%TEMP%/fithub_videos/{videoId}/original.{ext}`
3. Runs `FFProbe.AnalyseAsync` to get duration
4. Encodes 3 quality profiles with **FFmpeg (libx264/AAC)**:

| Quality | Resolution  | CRF | Preset  | Audio   | Target Bitrate |
|---------|-------------|-----|---------|---------|----------------|
| 360p    | 640×360     | 28  | faster  | 96 kbps | 500 kbps       |
| 720p    | 1280×720    | 23  | faster  | 128 kbps| 2500 kbps      |
| 1080p   | 1920×1080   | 20  | medium  | 192 kbps| 5000 kbps      |

Each resolution uses a `scale+pad` filter to preserve the original aspect ratio without cropping.

5. Uploads each encoded file to S3: `videos/{videoId}/360p.mp4`, `720p.mp4`, `1080p.mp4`
6. Saves a `VideoResolution` record per quality to the DB
7. Marks video as `Ready` with `durationSeconds`
8. Deletes the temp directory

On any error: marks video as `Failed` with the exception message; temp files are cleaned up in `finally`.

Relevant files:
- `HostJobs/Consumers/Videos/VideoEncodingConsumer.cs`
- `Clients/Videos/VideoClient.cs`
- `Application/Videos/VideoService.cs` — `ProcessAsync`

---

## Step 5 — Playback

**Frontend** polls `GET /api/v1/videos` every 8 seconds while any video is in `Pending` or `Processing` status.

Once `status === 'Ready'`, the user can click Play. The player calls:

```
GET /api/v1/videos/{id}/resolutions
```

Returns presigned **GET URLs** (2-hour expiry) for each encoded file:

```json
{
  "items": [
    { "quality": "Q360P", "qualityLabel": 360, "widthPx": 640, "heightPx": 360, "bitrateKbps": 500, "url": "http://..." },
    { "quality": "Q720P", "qualityLabel": 720, "widthPx": 1280, "heightPx": 720, "bitrateKbps": 2500, "url": "http://..." },
    { "quality": "Q1080P", "qualityLabel": 1080, "widthPx": 1920, "heightPx": 1080, "bitrateKbps": 5000, "url": "http://..." }
  ]
}
```

**`VideoPlayer`** component implements adaptive bitrate logic:
- On load, probes bandwidth by downloading a 200 KB test chunk and measuring speed
- Selects initial quality: `≥4 Mbps → 1080p`, `≥1.5 Mbps → 720p`, otherwise `360p`
- Every 4 seconds checks buffer:
  - Buffer > 15 s and not at max quality → upgrades
  - Buffer < 3 s and not at min quality → downgrades
- Quality switching preserves playback position and play state
- Manual quality override via dropdown (shown on hover)

Relevant files:
- `frontend/src/components/VideoPlayer/VideoPlayer.tsx`
- `frontend/src/api/videoApi.ts` — `getResolutions`
- `Application/Videos/VideoService.cs` — `GetResolutionUrlsAsync`

---

## S3 Key Structure

```
files/                          ← bucket
└── videos/
    └── {videoId}/
        ├── original.{ext}      ← raw uploaded file (kept after encoding)
        ├── 360p.mp4
        ├── 720p.mp4
        └── 1080p.mp4
```

---

## Video Status Lifecycle

```
Pending ──► Processing ──► Ready
   │
   └────────────────────────► Failed
```

| Status     | When set                                      |
|------------|-----------------------------------------------|
| `Pending`  | On `InitUpload` — video created in DB         |
| `Processing` | On `ProcessAsync` start — encoding begins   |
| `Ready`    | On `ProcessAsync` success — resolutions saved |
| `Failed`   | On `ProcessAsync` exception                   |

---

## Database Schema

```
video
├── id              UUID PK
├── title           text
├── original_file_id UUID FK → file_entity(id)  ON DELETE RESTRICT
├── status          text  (Pending | Processing | Ready | Failed)
├── duration_seconds int?
├── poster_s3key    text?   (currently not populated)
├── failure_reason  text?
└── created_at      timestamptz

video_resolution
├── id              UUID PK
├── video_id        UUID FK → video(id)  ON DELETE CASCADE
├── quality         text  (Q360P | Q720P | Q1080P)
├── s3_key          text
├── file_size_bytes bigint
├── width_px        int
├── height_px       int
└── bitrate_kbps    int
```

---

## API Reference

| Method | URL | Request | Response | Description |
|--------|-----|---------|----------|-------------|
| `POST` | `/api/v1/videos/init-upload` | `{ title, fileExtension }` | `{ videoId, presignedPutUrl }` | Start upload session |
| `PUT`  | `presignedPutUrl` | binary file | — | Upload file to S3 directly |
| `POST` | `/api/v1/videos/{id}/confirm-upload` | — | 202 | Enqueue for encoding |
| `GET`  | `/api/v1/videos` | — | `{ items: VideoResponse[] }` | List all videos |
| `GET`  | `/api/v1/videos/{id}` | — | `VideoResponse` | Get single video |
| `GET`  | `/api/v1/videos/{id}/resolutions` | — | `{ items: VideoResolutionUrlResponse[] }` | Get playback URLs |
| `DELETE` | `/api/v1/videos/{id}` | — | 204 | Delete video + S3 files |

---

## Configuration

```json
// appsettings.Development.json
{
  "AWS": {
    "ServiceURL": "http://localhost:9000",
    "AccessKey": "minio",
    "SecretKey": "minio123",
    "BucketName": "files",
    "NeedToEnsureBucketExists": true
  },
  "RabbitMQ": {
    "Nodes": ["localhost"],
    "Username": "rabbitmq",
    "Password": "rabbitmq123",
    "VirtualHost": "/",
    "NeedToPrepare": true
  },
  "Video": {
    "FFmpegBinaryFolder": "C:\\ffmpeg\\bin"
  }
}
```

`FFmpegBinaryFolder` — path to the directory containing `ffmpeg.exe` and `ffprobe.exe`.
Leave empty or omit if FFmpeg is on the system `PATH`.

---

## Known Limitations / TODOs

- **Poster generation** is currently disabled (`VideoService.ProcessAsync` has commented-out snapshot code)
- **`/api/v1/videos/{id}/process` is `[AllowAnonymous]`** — needs token-based protection so only HostJobs can call it
- **No re-encoding on failure** — a failed video stays `Failed` until manually deleted and re-uploaded
