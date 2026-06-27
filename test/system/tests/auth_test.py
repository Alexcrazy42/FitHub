from clients.auth_client import check_email_available
from consts.user_consts import base_email

def test_check_base_email():
    available = check_email_available(base_email)
    assert available is False

def test_check_random_email():
    available = check_email_available("random@email.ru")
    assert available is True