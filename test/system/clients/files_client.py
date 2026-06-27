import requests
from pathlib import Path
from pydantic import BaseModel

from clients.auth_client import ensure_auth
from consts.api_consts import BASE_URL

def get_presigned_url(file_path):
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/files/get-presigned-url"
    
    headers = {
        "Authorization": f"Bearer {token}",
    }
    
    with open(file_path, 'rb') as f:
        files = {
            'File': (Path(file_path).name, f, 'application/octet-stream')
        }
        
        response = requests.post(
            url,
            headers=headers,
            files=files,
            timeout=30
        )
    
        response.raise_for_status()
        return response.json()

def upload_file_s3(file_path, presigned_url):
    with open(file_path, 'rb') as f:
        response = requests.put(
            presigned_url,
            data=f,
            headers={
                'Content-Type': 'application/octet-stream'
            }
        )
    

    if response.status_code not in [200, 204]:
        raise Exception("Не смогли загрузить файл")



def confirm_file_upload(file_id):
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/files/{file_id}/confirm-upload"
    headers = {
        "Authorization": f"Bearer {token}"
    }

    response = requests.post(url, headers=headers)
    response.raise_for_status()


class MakeFilesActiveRequest(BaseModel):
    fileIds: list[str]
    entityId: str
    entityType: str

def make_files_active(request: MakeFilesActiveRequest):
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/files/make-files-active"
    headers = {
        "Authorization": f"Bearer {token}"
    }

    response = requests.post(url, json=request.model_dump(), headers=headers)
    response.raise_for_status()