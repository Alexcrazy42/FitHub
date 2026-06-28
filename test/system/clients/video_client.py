from enum import Enum
from pydantic import BaseModel
from typing import Any, Dict, List, Optional

import requests

from clients.auth_client import ensure_auth
from consts.api_consts import BASE_URL

class InitMultipartUploadRequest(BaseModel):
    title: str
    fileExtension: str
    fileSizeBytes: int

class UploadPart(BaseModel):
    partNumber: int
    url: str

class ConfirmUploadPart(BaseModel):
    partNumber: int
    eTag: str

class ConfirmParts(BaseModel):
    parts: List[ConfirmUploadPart]

class InitMultipartUploadResponse(BaseModel):
    videoId: str
    parts: List[UploadPart]

class VideoStatus(str, Enum):
    """Статус видео"""
    PENDING = 'Pending'
    PROCESSING = 'Processing'
    READY = 'Ready'
    FAILED = 'Failed'


class ResolutionResponse(BaseModel):
    quality: str
    qualityLabel: int
    widthPx: int
    heightPx: int
    bitrateKbps: int
    fileSizeBytes: int

class VideoResponse(BaseModel):
    id: str
    title: str
    status: VideoStatus
    durationSeconds: Optional[int]
    posterUrl: Optional[str]
    failureReason: Optional[str]
    createdAt: str
    resolutions: List[ResolutionResponse]

def init_multipart_upload(request: InitMultipartUploadRequest) -> InitMultipartUploadResponse:
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/videos/init-multipart-upload"
    
    headers = {
        "Authorization": f"Bearer {token}",
    }

    response = requests.post(url, json=request.model_dump(), headers=headers)
    response.raise_for_status()

    return InitMultipartUploadResponse(**response.json())

def upload_video_part(part: UploadPart, file_path: str, start_byte: int, end_byte: int) -> Optional[str]:
    """
    Загрузить одну часть видео
    
    Args:
        part: Информация о части (номер и URL)
        file_path: Путь к локальному файлу
        start_byte: Начальный байт (включительно)
        end_byte: Конечный байт (включительно)
    
    Returns:
        ETag части или None при ошибке
    """
    # Открываем файл и читаем нужный диапазон
    with open(file_path, 'rb') as f:
        # Перемещаемся к начальному байту
        f.seek(int(start_byte))
        
        # Читаем нужное количество байт
        chunk_size = end_byte - start_byte + 1
        data = f.read(int(chunk_size))
        
        # Отправляем PUT запрос к presigned URL
        response = requests.put(
            part.url,
            data=data,
            headers={
                'Content-Type': 'application/octet-stream',
                'Content-Length': str(len(data))
            },
            timeout=300  # Большой таймаут для больших частей
        )
        
        if response.status_code not in [200, 204]:
            raise Exception("Ошибка загрузка части")
    
        etag = response.headers.get('ETag', '')
        print(f"✅ Часть {part.partNumber} загружена. ETag: {etag[:20]}...")
        return etag

def complete_multi_upload(video_id: str, parts: ConfirmParts):
    """
    Завершить Multipart Upload
    
    Args:
        video_id: ID видео
        parts: Список частей с partNumber и eTag
    
    Returns:
        Ответ сервера
    """
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/videos/{video_id}/complete-multipart"
    
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }

    
    response = requests.post(url, json=parts.model_dump(), headers=headers)
    response.raise_for_status()

def get_video(videoId) -> VideoResponse:
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/videos/{videoId}"
    
    headers = {
        "Authorization": f"Bearer {token}",
    }

    response = requests.get(url, headers=headers)
    response.raise_for_status()

    return VideoResponse(**response.json())

def get_video_status(videoId) -> VideoStatus:
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/videos/{videoId}"
    
    headers = {
        "Authorization": f"Bearer {token}",
    }

    response = requests.get(url, headers=headers)
    response.raise_for_status()

    video = VideoResponse(**response.json())
    return video.status