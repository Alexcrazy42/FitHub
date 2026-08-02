import allure
from clients.auth_client import role_ensure_auth
import logging
import time
from signalrcore.hub_connection_builder import HubConnectionBuilder
from consts.chat_consts import BASE_INDIVIDUAL_CHAT_ID
from clients.chat_client import send_message, update_message
from consts.user_consts import base_email

@allure.epic("Чат")
@allure.feature("Чат")
@allure.story("Отправка сообщения")
@allure.severity(allure.severity_level.CRITICAL)
def test_send_message():
    gym_admin_token = role_ensure_auth("gymAdmin")
    cms_admin_token = role_ensure_auth("admin")

    message_text = "test"

    server_url = f"http://localhost:5001/chathub?access_token={gym_admin_token}"

    gymAdminMessages = []

    def on_create_message_received(args):
        message = args[0]
        gymAdminMessages.append(message)

    def on_error(error):
        print(f"\nAn error occurred: {error}")
        raise RuntimeError(error)

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

    hub_connection.on("CreateMessage", on_create_message_received)

    hub_connection.on_error(on_error)

    hub_connection.start()

    created_message = send_message(cms_admin_token, {
        "chatId": BASE_INDIVIDUAL_CHAT_ID,
        "messageText": message_text,
        "replyMessageId": None
    })

    time.sleep(1)

    assert len(gymAdminMessages) == 1
    msg = gymAdminMessages[0]

    assert msg['id'] == created_message['id']
    assert msg['chatId'] == BASE_INDIVIDUAL_CHAT_ID
    assert msg['messageText'] == message_text
    assert msg['replyMessage'] is None
    assert len(msg['attachments']) == 0
    assert msg['createdBy']['email'] == base_email


@allure.epic("Чат")
@allure.feature("Чат")
@allure.story("Обновление сообщения")
@allure.severity(allure.severity_level.CRITICAL)
def test_update_message():
    gym_admin_token = role_ensure_auth("gymAdmin")
    cms_admin_token = role_ensure_auth("admin")

    message_text = "test"
    new_message_text = "test1"

    server_url = f"http://localhost:5001/chathub?access_token={gym_admin_token}"

    gymAdminMessages = []

    def on_create_message_received(args):
        message = args[0]
        gymAdminMessages.append(message)

    def on_update_message_received(args):
        message = args[0]
        
        # Проверяем, есть ли уже такое сообщение
        already_msg = next(
            (msg for msg in gymAdminMessages if msg["id"] == message['id']),
            None
        )

        if already_msg is not None:
            # Удаляем существующее сообщение
            gymAdminMessages[:] = [msg for msg in gymAdminMessages if msg['id'] != message['id']]
        
        gymAdminMessages.append(message)



    def on_error(error):
        print(f"\nAn error occurred: {error}")
        raise RuntimeError(error)

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

    hub_connection.on("CreateMessage", on_create_message_received)
    hub_connection.on("UpdateMessage", on_update_message_received)

    hub_connection.on_error(on_error)

    hub_connection.start()

    created_message = send_message(cms_admin_token, {
        "chatId": BASE_INDIVIDUAL_CHAT_ID,
        "messageText": message_text,
        "replyMessageId": None
    })

    update_message(cms_admin_token, {
        "id": created_message['id'],
        "messageText": new_message_text
    })

    time.sleep(2)

    assert len(gymAdminMessages) == 1
    msg = gymAdminMessages[0]

    assert msg['id'] == created_message['id']
    assert msg['chatId'] == BASE_INDIVIDUAL_CHAT_ID
    assert msg['messageText'] == new_message_text
    assert msg['replyMessage'] is None
    assert len(msg['attachments']) == 0
    assert msg['createdBy']['email'] == base_email

    