import requests
from consts.api_consts import BASE_URL

def send_message(token, message):
    """
`    # {
#     "chatId": "019f3280-5fdc-7f21-a604-7de03de9d8e2",
#     "messageText": "тест",
#     "replyMessageId": null,
#     "links": [],
#     "tags": [],
#     "photos": [],
#     "stickers": [],
#     "documents": []
# }`

    #{
    #     "id": "019f3295-20d8-7344-b62f-f662e149ca08",
    #     "chatId": "019f3280-5fdc-7f21-a604-7de03de9d8e2",
    #     "messageText": "тест",
    #     "replyMessage": null,
    #     "forwardedMessage": null,
    #     "attachments": [],
    #     "createdAt": "2026-07-05T14:01:01.9428119+00:00",
    #     "createdBy": {
    #         "id": "a88a98f0-35e8-46c4-a38e-bf88bd5c9ebc",
    #         "surname": "Мамедов",
    #         "name": "Александр",
    #         "email": "alexcrazy42@mail.ru",
    #         "isActive": true,
    #         "startActiveAt": null,
    #         "roleNames": [
    #             "CmsAdmin"
    #         ]
    #     },
    #     "updatedAt": "2026-07-05T14:01:01.9428119+00:00",
    #     "updatedBy": {
    #         "id": "a88a98f0-35e8-46c4-a38e-bf88bd5c9ebc",
    #         "surname": "Мамедов",
    #         "name": "Александр",
    #         "email": "alexcrazy42@mail.ru",
    #         "isActive": true,
    #         "startActiveAt": null,
    #         "roleNames": [
    #             "CmsAdmin"
    #         ]
    #     }
    # }
    """

    url = f"{BASE_URL}/api/v1/messages"

    headers = {
        "Authorization": f"Bearer {token}"
    }

    response = requests.post(url, json=message, headers=headers)
    response.raise_for_status()
    
    return response.json()



def update_message(token, message):
    # PUT http://localhost:5001/api/v1/messages/019f32a0-0477-7735-869d-346a2b7ee3c5

#    {
#        "messageText": "test1",
#        "replyMessageId": null,
#        "links": [],
#        "tags": [],
#        "photos": []
#    }

# {
#     "id": "019f32a0-0477-7735-869d-346a2b7ee3c5",
#     "chatId": "019f3280-5fdc-7f21-a604-7de03de9d8e2",
#     "messageText": "test1",
#     "replyMessage": null,
#     "forwardedMessage": null,
#     "attachments": [],
#     "createdAt": "2026-07-05T14:12:55.546656+00:00",
#     "createdBy": {
#         "id": "a88a98f0-35e8-46c4-a38e-bf88bd5c9ebc",
#         "surname": "Мамедов",
#         "name": "Александр",
#         "email": "alexcrazy42@mail.ru",
#         "isActive": true,
#         "startActiveAt": null,
#         "roleNames": [
#             "CmsAdmin"
#         ]
#     },
#     "updatedAt": "2026-07-05T14:36:44.0914992+00:00",
#     "updatedBy": {
#         "id": "a88a98f0-35e8-46c4-a38e-bf88bd5c9ebc",
#         "surname": "Мамедов",
#         "name": "Александр",
#         "email": "alexcrazy42@mail.ru",
#         "isActive": true,
#         "startActiveAt": null,
#         "roleNames": [
#             "CmsAdmin"
#         ]
#     }
# }

    message_id = message['id']
    url = f"{BASE_URL}/api/v1/messages/{message_id}"

    headers = {
        "Authorization": f"Bearer {token}"
    }

    response = requests.put(url, json=message, headers=headers)
    response.raise_for_status()
    
    return response.json()