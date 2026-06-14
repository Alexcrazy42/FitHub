import time
import logging
from typing import Callable, Any, Optional

logger = logging.getLogger(__name__)

def wait_until(
    condition: Callable[[], bool],
    timeout: float = 10.0,
    period: float = 0.5,
    message: str = "Condition not met",
    raise_on_timeout: bool = True
) -> bool:
    """Ожидает, что condition() вернёт True в течение timeout"""
    deadline = time.time() + timeout
    while time.time() < deadline:
        if condition():
            return True
        time.sleep(period)
    if raise_on_timeout:
        raise TimeoutError(f"{message} after {timeout}s")
    return False

def wait_for_value(
    getter: Callable[[], Any],
    expected: Any,
    timeout: float = 10.0,
    period: float = 0.5,
    message: Optional[str] = None
) -> None:
    """Ожидает, что getter() вернёт expected (сравнивает через ==)"""
    msg = message or f"Value {expected} not achieved"
    def condition():
        try:
            return getter() == expected
        except Exception:
            return False
    wait_until(condition, timeout, period, msg)

def retry(
    func: Callable,
    retries: int = 3,
    delay: float = 1.0,
    exceptions: tuple = (Exception,)
):
    """Повторяет вызов func при исключениях (для неидемпотентных операций — осторожно!)"""
    last_exc = None
    for attempt in range(1, retries + 1):
        try:
            return func()
        except exceptions as e:
            last_exc = e
            logger.warning(f"Attempt {attempt}/{retries} failed: {e}")
            if attempt < retries:
                time.sleep(delay)
    raise last_exc

# Пример использования в тесте:
# def test_order_event_in_kafka(kafka_consumer, order_id):
#     wait_until(
#         lambda: kafka_consumer.has_message(f"order_created_{order_id}"),
#         timeout=30,
#         message="Order created event not delivered"
#     )