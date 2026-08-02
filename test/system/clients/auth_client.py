import requests
from consts.user_consts import base_email, base_pass, gym_admin_email, gym_admin_pass
from consts.api_consts import BASE_URL

_token = ""

_tokens = {
    "admin": "",
    "gymAdmin": ""
}

creds = {
    "admin": [base_email, base_pass],
    "gymAdmin": [gym_admin_email, gym_admin_pass]
}

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

def role_login(role):
    if role not in creds:
        raise ValueError("Невалидная роль")

    cred = creds[role]

    url = f"{BASE_URL}/api/v1/auth/login"
    body = {
        "username": cred[0],
        "password": cred[1]
    }
    response = requests.post(url, json=body)
    response.raise_for_status()

    data = response.json()

    if 'jwtToken' not in data or not data['jwtToken']:
        raise ValueError("Токен не получен. Проверьте логин и пароль.")
    
    token = data['jwtToken']

    
    _tokens[role] = token
    return token


def ensure_auth() -> str:
    global _token
    if _token and len(_token) > 1:
        return _token
    return login(base_email, base_pass)

def role_ensure_auth(role) -> str:
    if role not in creds:
        raise ValueError("Невалидная роль")
    
    token = _tokens[role]

    if token and len(token) > 1:
        return token
    
    return role_login(role)

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