from pathlib import Path

import allure
from clients.gym_client import get_gyms, create_gym, get_gym, delete_gym, update_gym, GymUpdate
from clients.files_client import get_presigned_url, upload_file_s3, confirm_file_upload, make_files_active, MakeFilesActiveRequest

@allure.epic("Спортзалы")
@allure.feature("Управление спортзалами")
@allure.story("Полученив залов")
@allure.severity(allure.severity_level.BLOCKER)
def test_get_gyms():
    gyms = get_gyms(1, 10)
    assert len(gyms['items']) != 0
    
@allure.epic("Спортзалы")
@allure.feature("Управление спортзалами")
@allure.story("Создание зала")
@allure.severity(allure.severity_level.BLOCKER)
def test_create_gym():
    test_name = 'test_name'
    test_desc = 'test_desc'
    gym = create_gym(test_name, test_desc)
    assert gym['name'] == test_name
    assert gym['description'] == test_desc
    assert gym['id'] is not None
    delete_gym(gym['id'])


@allure.epic("Спортзалы")
@allure.feature("Управление спортзалами")
@allure.story("Получение зала")
@allure.severity(allure.severity_level.BLOCKER)
def test_get_gym():
    test_name = 'test_name'
    test_desc = 'test_desc'
    gym = create_gym(test_name, test_desc)

    gym_id = gym['id']

    new_gym = get_gym(gym_id)
    assert new_gym['id'] == gym_id
    assert new_gym['name'] == gym['name']
    assert new_gym['description'] == gym['description']
    delete_gym(gym['id'])

@allure.epic("Спортзалы")
@allure.feature("Управление спортзалами")
@allure.story("Обновление зала")
@allure.severity(allure.severity_level.BLOCKER)
def test_update_gym():
    test_name = 'test_name'
    test_desc = 'test_desc'
    gym = create_gym(test_name, test_desc)

    gym_id = gym['id']
    update_name = 'update_name'
    update_desc = 'update_desc'

    updated_gym_model = GymUpdate(
        id= gym_id,
        name= update_name,
        description= update_desc
    )
    updated_gym = update_gym(updated_gym_model)

    assert updated_gym['id'] == gym_id
    assert updated_gym['name'] == update_name
    assert updated_gym['description'] == update_desc
    delete_gym(gym_id)




@allure.epic("Спортзалы")
@allure.feature("Управление спортзалами")
@allure.story("Удаление зала")
@allure.severity(allure.severity_level.BLOCKER)
def test_delete_gym():
    test_name = 'test_name'
    test_desc = 'test_desc'
    gym = create_gym(test_name, test_desc)

    gym_id = gym['id']

    delete_gym(gym_id)

    possible_gym = get_gym(gym_id)
    assert possible_gym is None


@allure.epic("Спортзалы")
@allure.feature("Управление спортзалами")
@allure.story("Добавление фото")
@allure.severity(allure.severity_level.BLOCKER)
def test_upload_gym_photo():
    entity_type = 'Gym'
    test_name = 'test_name_for_file_upload'
    test_desc = 'test_desc_for_file_upload'
    gym = create_gym(test_name, test_desc)
    gym_id = gym['id']

    file_path = Path(__file__).parent / "images" / "007.png"
    assert Path(file_path).is_file()

    presigned_url_response = get_presigned_url(file_path)

    assert presigned_url_response['url'] is not None

    presigned_url = presigned_url_response['url']
    file_id = presigned_url_response['fileId']
    upload_file_s3(file_path, presigned_url)

    confirm_file_upload(file_id)

    request = MakeFilesActiveRequest(
        fileIds=[file_id],
        entityId=gym_id,
        entityType=entity_type
    )
    make_files_active(request)

    updated_gym = get_gym(gym_id)
    assert updated_gym['imageFileId'] == file_id

    delete_gym(gym_id)

    
