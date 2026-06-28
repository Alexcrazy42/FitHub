from pathlib import Path
import clients.equipment_client as client
import clients.entities_client as entityClient
from clients.files_client import MakeFilesActiveRequest, confirm_file_upload, get_presigned_url, make_files_active, upload_file_s3

def test_get_equipments():
    response = client.get_equipments(1, 10)

    assert len(response.items) > 0

def test_create_equipment():
    request = client.CreateEquipmentRequest(
        brandId=client.BASE_BRAND_ID,
        name='test',
        description='test',
        additionalDescroption='',
        instructionAddBefore='2026-01-01',
        isActive=True
    )

    equipment = client.create_equipment(request)

    assert equipment.name == 'test'
    assert equipment.description == 'test'
    assert equipment.additionalDescroption == ''
    assert equipment.instructionAddBefore == '2026-01-01'
    assert equipment.isActive is True
    assert equipment.brand.id == client.BASE_BRAND_ID

    client.delete_equipment(equipment.id)


def test_upload_photo():
    entity_type = 'Equipment'
    maxFileCount = entityClient.get_max_file_count(entity_type)

    assert maxFileCount >= 1

    request = client.CreateEquipmentRequest(
        brandId=client.BASE_BRAND_ID,
        name='test',
        description='test',
        additionalDescroption='',
        instructionAddBefore='2026-01-01',
        isActive=True
    )

    equipment = client.create_equipment(request)

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
        entityId=equipment.id,
        entityType=entity_type
    )
    make_files_active(request)

    files = entityClient.get_files(equipment.id, entity_type)

    assert len(files.items) > 0
    assert files.items[0].id == file_id

def test_double_file_upload():
    entity_type = 'Equipment'
    maxFileCount = entityClient.get_max_file_count(entity_type)

    assert maxFileCount >= 1

    request = client.CreateEquipmentRequest(
        brandId=client.BASE_BRAND_ID,
        name='test',
        description='test',
        additionalDescroption='',
        instructionAddBefore='2026-01-01',
        isActive=True
    )

    equipment = client.create_equipment(request)

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
        entityId=equipment.id,
        entityType=entity_type
    )
    make_files_active(request)

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
        entityId=equipment.id,
        entityType=entity_type
    )
    make_files_active(request)

    files = entityClient.get_files(equipment.id, entity_type)

    assert len(files.items) == 2