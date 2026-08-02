import requests
from clients.auth_client import ensure_auth
from clients.base_models import ListResponse
from consts.api_consts import BASE_URL
from pydantic import BaseModel

class FileResponse(BaseModel):
    id: str
    fileName: str

FileListResponse = ListResponse[FileResponse]



def get_max_file_count(entityType):
    token = ensure_auth()
    
    url = f"{BASE_URL}/api/v1/entities?entityType={entityType}"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    
    response = requests.get(url, headers=headers)
    response.raise_for_status()
    
    return response.json()['maxFileCount']

def get_files(entityId, entityType) -> FileListResponse:
    token = ensure_auth()
    
    params = {
        'entityId': entityId,
        'entityType': entityType
    }

    url = f"{BASE_URL}/api/v1/files"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    
    response = requests.get(url, params=params, headers=headers)
    response.raise_for_status()
    
    return FileListResponse(**response.json())