import requests
from clients.auth_client import ensure_auth
from consts.api_consts import BASE_URL


def get_max_file_count(entityType):
    token = ensure_auth()
    
    url = f"{BASE_URL}/api/v1/entities?entityType={entityType}"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    
    response = requests.get(url, headers=headers)
    response.raise_for_status()
    
    return response.json()['maxFileCount']