import requests
from consts.user_consts import base_email, base_pass
from consts.api_consts import BASE_URL

_token = ""

def login(email, password):
    global _token
    url = f"{BASE_URL}/api/v1/auth/login"
    body = {
        "username": email,
        "password": password
    }
    response = requests.post(url, json=body)
    response.raise_for_status()

    data = response.json()

    if 'jwtToken' not in data or not data['jwtToken']:
        raise ValueError("Токен не получен. Проверьте логин и пароль.")
    
    _token = data['jwtToken']
    return _token

def ensure_auth() -> str:
    global _token
    if _token and len(_token) > 1:
        return _token
    return login(base_email, base_pass)

def check_email_available(email: str) -> bool:
    """Проверить, доступен ли email"""

    token = ensure_auth()

    url = f"{BASE_URL}/api/v1/emails/available"
    params = {
        "email": email
    }
    headers = {
        "accept": "text/plain",
        "Authorization": f"Bearer {token}"
    }
    
    response = requests.get(url, params=params, headers=headers)
    response.raise_for_status()
    
    return response.text.lower() == "true"