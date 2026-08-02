import requests
from consts.api_consts import BASE_URL
from clients.auth_client import ensure_auth
from pydantic import BaseModel

def get_gyms(page, pageSize):
    """Проверить, доступен ли email"""
    url = f"{BASE_URL}/api/v1/gyms"
    params = { "PageNumber": page, "PageSize": pageSize }
    
    response = requests.get(url, params=params)
    response.raise_for_status()
    
    return response.json()

def create_gym(name, description):
    token = ensure_auth()
    
    url = f"{BASE_URL}/api/v1/gyms"
    params = { "name": name, "description": description }
    headers = {
        "Authorization": f"Bearer {token}"
    }
    
    response = requests.post(url, json=params, headers=headers)
    response.raise_for_status()
    
    return response.json()

def get_gym(id):
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/gyms/{id}"
    headers = {
        "Authorization": f"Bearer {token}"
    }

    response = requests.get(url, headers=headers)
    if response.status_code == 404:
        return None

    return response.json()



class GymUpdate(BaseModel):
    id: str
    name: str
    description: str

def update_gym(gym: GymUpdate):
    token = ensure_auth()
    
    url = f"{BASE_URL}/api/v1/gyms"
    headers = {
        "Authorization": f"Bearer {token}"
    }
    
    response = requests.put(url, json=gym.model_dump(), headers=headers)
    response.raise_for_status()
    
    return response.json()
    

def delete_gym(id):
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/gyms/{id}"
    headers = {
        "Authorization": f"Bearer {token}"
    }

    response = requests.delete(url, headers=headers)
    response.raise_for_status()