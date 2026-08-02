import requests
from consts.api_consts import BASE_URL
from pydantic import BaseModel, Field
from datetime import date
from typing import Optional
from clients.base_models import ListResponse
from clients.auth_client import ensure_auth

BASE_BRAND_ID = '019f0d93-e0cf-7306-95d7-fc03a17b1487'

class BrandResponse(BaseModel):
    id: str
    name: str
    description: str

class EquipmentResponse(BaseModel):
    id: str
    name: str
    description: Optional[str] = None
    additionalDescroption: Optional[str] = None
    instructionAddBefore: Optional[str] = None
    isActive: bool
    brand: BrandResponse

EquipmentListResponse = ListResponse[EquipmentResponse]

class CreateEquipmentRequest(BaseModel):
    brandId: str
    name: str
    description: str
    additionalDescroption: str
    instructionAddBefore: Optional[str]
    isActive: bool


def get_equipments(page, pageSize):
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/equipments"
    
    params = { "PageNumber": page, "PageSize": pageSize }

    headers = {
        "Authorization": f"Bearer {token}",
    }

    response = requests.get(url, params=params, headers=headers)

    response.raise_for_status()

    return EquipmentListResponse(**response.json())

    
def create_equipment(request: CreateEquipmentRequest) -> EquipmentResponse:
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/equipments"

    headers = {
        "Authorization": f"Bearer {token}",
    }

    response = requests.post(url, json=request.model_dump(), headers=headers)

    response.raise_for_status()

    return EquipmentResponse(**response.json())

def delete_equipment(id):
    token = ensure_auth()
    url = f"{BASE_URL}/api/v1/equipments/{id}"

    headers = {
        "Authorization": f"Bearer {token}",
    }

    response = requests.delete(url, headers=headers)

    response.raise_for_status()