import json
import allure
import pytest
import requests
from clients.auth_client import role_ensure_auth
from consts.api_consts import BASE_DOMAIN
import asyncio
import websockets
import logging
import time
from signalrcore.hub_connection_builder import HubConnectionBuilder


def signalr_negotiate(domain: str, hub: str, token: str) -> dict:
    """Выполнить negotiate с SignalR"""
    url = f"http://{domain}/{hub}/negotiate?negotiateVersion=1"
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }
    
    response = requests.post(url, headers=headers)
    response.raise_for_status()
    return response.json()

async def connect_to_signalr(domain: str, hub: str, token: str):
    """Подключение к SignalR через WebSocket"""
    
    # 1. Negotiate
    negotiate_data = signalr_negotiate(domain, hub, token)
    
    connection_id = negotiate_data['connectionId']
    # connection_token = negotiate_data.get('connectionToken')
    
    # 2. Формируем WebSocket URL с правильными параметрами
    ws_url = f"ws://{domain}/{hub}"
    params = {
        "id": connection_id,
        "access_token": token,
        # "connectionToken": connection_token,  # если нужен
    }
    
    # Собираем URL с параметрами
    import urllib.parse
    query = urllib.parse.urlencode(params)
    full_url = f"{ws_url}?{query}"
    
    # 3. Подключаемся
    async with websockets.connect(full_url) as websocket:
        # SignalR ожидает специальный протокол
        # Отправляем {"protocol": "json", "version": 1}
        await websocket.send(json.dumps({
            "protocol": "json",
            "version": 1
        }))
        
        # Получаем ответ
        response = await websocket.recv()

        response_text = response.decode('utf-8') if isinstance(response, bytes) else response

        clean_response = response_text.rstrip('\x1e')

        #data = json.loads(response)
        data = json.loads(clean_response)
        
        if data.get("type") == 1:
            print("✅ Подключено!")
            return websocket, connection_id
        
        raise Exception(f"Ошибка подключения: {data}")

array = []

@allure.epic("Чат")
@allure.feature("Чат")
@allure.story("Подключение")
@allure.severity(allure.severity_level.BLOCKER)
@pytest.mark.asyncio
async def test_5():
    """Тест: успешное подключение к чату"""
    
    # 1. Получаем токен
    token = role_ensure_auth("admin")
    
    array.append("test1")

def test_4():
    token = role_ensure_auth("admin")

    server_url = f"http://localhost:5001/chathub?access_token={token}"
    
    # 2. Configure Logging (Optional but helpful for debugging)
    logger = logging.getLogger("signalrcore")
    logger.setLevel(logging.INFO)

    # 3. Setup Callback Handlers for Server events
    def on_message_received(args):
        """Triggered when the C# server runs Clients.All.SendAsync('ReceiveMessage', ...)"""
        user = args[0]
        message = args[1]
        print(f"\n[Broadcast Received] {user}: {message}")

    def on_connect():
        print("\nConnection successfully opened and handshake completed.")

    def on_disconnect():
        print("\nConnection closed.")

    def on_error(error):
        print(f"\nAn error occurred: {error}")

    # 4. Build Hub Connection
    hub_connection = HubConnectionBuilder()\
        .with_url(server_url, options={"verify_ssl": False})\
        .configure_logging(logging.INFO)\
        .with_automatic_reconnect({
            "type": "raw",
            "keep_alive_interval": 10,
            "reconnect_interval": 5,
            "max_attempts": 5
        })\
        .build()

    # 5. Register Server-to-Client Event Listeners
    # This MUST match the string name used in the C# server's SendAsync call
    hub_connection.on("ReceiveMessage", on_message_received)

    # 6. Register Lifecycle Event Listeners
    hub_connection.on_open(on_connect)
    hub_connection.on_close(on_disconnect)
    hub_connection.on_error(on_error)

    # 7. Start Connection
    print("Connecting to server...")
    hub_connection.start()

    # Wait briefly for connection stabilization
    #time.sleep(2) 

    # 8. Send data Client-to-Server
    if hub_connection.transport.state.name == "connected":
        print("Sending message to C# hub...")
        # First argument is the C# hub method name. Second argument is an array of payload arguments.
        hub_connection.send("Heartbeat", [])

    #time.sleep(2)

    array.append("test2")