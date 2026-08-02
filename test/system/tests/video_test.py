from pathlib import Path
from typing import List
import clients.video_client as videoClient
from utils.waiters import wait_until
import allure

@allure.epic("Видео")
@allure.feature("Управление видео")
@allure.story("Создание видео")
@allure.severity(allure.severity_level.CRITICAL)
def test_create_video():
    file_title = "vlastelin-koshachih-kolec.mp4"
    file_path = Path(__file__).parent / "images" / file_title
    total_bytes = Path(file_path).stat().st_size
    upload_request = videoClient.InitMultipartUploadRequest(
        title= file_title,
        fileExtension= 'mp4',
        fileSizeBytes=total_bytes
    )
    upload_response = videoClient.init_multipart_upload(upload_request)

    total_parts = len(upload_response.parts)
    part_size_bytes = total_bytes / total_parts

    parts : List[videoClient.ConfirmUploadPart] = []

    for part in upload_response.parts:
        part_number = part.partNumber
        start_byte = (part_number - 1) * part_size_bytes
        end_byte = min(start_byte + part_size_bytes - 1, total_bytes - 1)
        
        
        etag = videoClient.upload_video_part(part, file_path, start_byte, end_byte)

        upload_part = videoClient.ConfirmUploadPart(partNumber=part.partNumber, eTag=etag)

        parts.append(upload_part)
        

    videoClient.complete_multi_upload(upload_response.videoId, videoClient.ConfirmParts(parts=parts))

    video = videoClient.get_video(upload_response.videoId)

    assert video.status == videoClient.VideoStatus.PENDING

    wait_until(
        condition=lambda: videoClient.get_video_status(upload_response.videoId) == videoClient.VideoStatus.READY,
        timeout=40.0,
        message="Видео не удалось успешно преобразовать"
    )

