# Stickers API — Frontend Integration Guide

All requests require a `Bearer` JWT token in the `Authorization` header (the same token used for the chat SignalR connection).

Base URL: `http://localhost:5000/api/v1`

---

## Roles

| Role | Can do |
|---|---|
| Any authenticated user | Read sticker groups and stickers |
| CMS admin | Create, update, delete groups and stickers |

---

## Entity Type

The sticker file upload flow uses `EntityType = 3` (integer enum value for `Sticker`).
Pass this value wherever `entityType` is required.

---

## Full Flow: Add a sticker (admin)

The correct sequence is: **upload file → confirm upload → create sticker**.
Never save a sticker referencing a file that hasn't been confirmed yet.

### Step 1 — Get a presigned S3 upload URL

```
POST /api/v1/files/get-presigned-url
Content-Type: multipart/form-data

file: <binary>          # the .gif, .png, or .webp file
```

Response:

```json
{
  "fileId": "fithub_file_01ABCDEF...",
  "url": "https://s3.example.com/bucket/key?X-Amz-...",
  "objectKey": "uploads/01ABCDEF.gif"
}
```

Save `fileId` — you will need it in steps 2 and 3.

### Step 2 — Upload the file directly to S3

```
PUT <url from step 1>
Content-Type: image/gif   # match the actual file type
Body: <raw binary>
```

This is a direct S3 PUT — no auth header, no JSON. On success S3 returns `200 OK` with an empty body.

### Step 3 — Confirm the upload

```
POST /api/v1/files/{fileId}/confirm-upload
Authorization: Bearer <token>
```

Response: `204 No Content`

### Step 4 — Create a sticker group (if one doesn't exist yet)

```
POST /api/v1/sticker-groups
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "Reactions"
}
```

Response:

```json
{
  "id": "fithub_sticker-group_01ABCDEF...",
  "name": "Reactions",
  "isActive": false
}
```

Groups start **inactive**. Users will not see the group in the picker until it is explicitly activated (see below).

### Step 5 — Add the sticker to the group

```
POST /api/v1/stickers
Authorization: Bearer <token>
Content-Type: application/json

{
  "groupId": "fithub_sticker-group_01ABCDEF...",
  "fileId":  "fithub_file_01ABCDEF...",
  "name":    "thumbs up"
}
```

Response:

```json
{
  "id":       "fithub_sticker_01ABCDEF...",
  "name":     "thumbs up",
  "groupId":  "fithub_sticker-group_01ABCDEF...",
  "fileId":   "fithub_file_01ABCDEF...",
  "position": 0
}
```

`position` is zero-based and assigned automatically (count of existing stickers in the group at insert time).

### Step 6 — Activate the group when it's ready

```
POST /api/v1/sticker-groups/{id}/activate
Authorization: Bearer <token>
```

Response: the updated `StickerGroupResponse` with `"isActive": true`.
Only active groups should be shown in the chat sticker picker.

---

## Sticker Group endpoints

### List all groups

```
GET /api/v1/sticker-groups
Authorization: Bearer <token>
```

Returns all groups that have not been deleted (both active and inactive).
Filter on the frontend: show only `isActive === true` to regular users; show all to admins.

Response:

```json
{
  "items": [
    { "id": "...", "name": "Reactions", "isActive": true },
    { "id": "...", "name": "Draft pack", "isActive": false }
  ],
  "currentPage": null,
  "pageSize": null,
  "totalItems": null,
  "totalPages": null
}
```

### List stickers in a group

```
GET /api/v1/sticker-groups/{id}/stickers
Authorization: Bearer <token>
```

Returns all stickers for that group ordered by `position` ascending.

Response:

```json
{
  "items": [
    { "id": "...", "name": "thumbs up",   "groupId": "...", "fileId": "...", "position": 0 },
    { "id": "...", "name": "thumbs down", "groupId": "...", "fileId": "...", "position": 1 }
  ],
  "currentPage": null,
  "pageSize": null,
  "totalItems": null,
  "totalPages": null
}
```

### Update group name (admin)

```
PUT /api/v1/sticker-groups/{id}
Authorization: Bearer <token>
Content-Type: application/json

{ "name": "New name" }
```

Response: updated `StickerGroupResponse`.

### Delete a group (admin)

```
DELETE /api/v1/sticker-groups/{id}
Authorization: Bearer <token>
```

Response: `200 OK` (empty body).
Soft-deletes the group and removes all its stickers and their files from S3.

---

## Sticker endpoints

### List all stickers (paged)

```
GET /api/v1/stickers?pageNumber=1&pageSize=50
Authorization: Bearer <token>
```

Ordered by `position`. Useful for admin overview pages.

Response:

```json
{
  "items": [ ... ],
  "currentPage": 1,
  "pageSize": 50,
  "totalItems": 120,
  "totalPages": 3
}
```

### Rename a sticker (admin)

```
PUT /api/v1/stickers/{id}/name
Authorization: Bearer <token>
Content-Type: application/json

{ "name": "heart eyes" }
```

Response: updated `StickerResponse`.

### Replace a sticker image (admin)

Run steps 1–3 of the upload flow first to get a new confirmed `fileId`, then:

```
PUT /api/v1/stickers/{id}/photo
Authorization: Bearer <token>
Content-Type: application/json

{ "newFileId": "fithub_file_NEWID..." }
```

The old file is deleted from S3 automatically. Response: updated `StickerResponse`.

### Delete a sticker (admin)

```
DELETE /api/v1/stickers/{id}
Authorization: Bearer <token>
```

Response: `200 OK` (empty body). Removes the sticker and its S3 file.

---

## Render a sticker image

To display a sticker from a `fileId`:

```
GET /api/v1/files/{fileId}
Authorization: Bearer <token>
```

Returns the raw file bytes with the correct `Content-Type` header.
You can set this directly as an `<img>` `src` only if your HTTP client attaches the auth header (e.g. via a blob URL approach). The simplest pattern:

```ts
// fetch the blob, create an object URL, assign to <img>
const res = await fetch(`/api/v1/files/${fileId}`, {
  headers: { Authorization: `Bearer ${token}` }
});
const blob = await res.blob();
const url = URL.createObjectURL(blob);
imgElement.src = url;
// Remember to call URL.revokeObjectURL(url) when the component unmounts.
```

---

## Error responses

All errors follow the same shape:

```json
{
  "message": "Стикер не найден!"
}
```

| HTTP status | Meaning |
|---|---|
| `400` | Validation error — missing or invalid field |
| `401` | No or expired JWT token |
| `403` | Authenticated but insufficient role |
| `404` | Entity not found |
| `500` | Unexpected server error |

---

## ID format

All IDs are prefixed strings, not plain GUIDs:

| Entity | Example ID |
|---|---|
| StickerGroup | `fithub_sticker-group_<guid>` |
| Sticker | `fithub_sticker_<guid>` |
| File | `fithub_file_<guid>` |

Pass them as-is in URL segments and request bodies — do not strip the prefix.
