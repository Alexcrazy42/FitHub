# E2E тесты

В основном пишутся командой QA. Тестирование через браузер.

## Методология

Есть Акторы (Пользователь, Админ, Гость).
Есть Задачи (высокоуровневые действия: AddItemToCart).
Есть Взаимодействия (низкоуровневые: Click, Fill, WaitFor).

Page Object Model - инкапсулируем взаимодействие со страницей в удобные обертки

## Инструменты и окружение

Инструменты: playwright
Окружение: на локалке поднятие через docker-compose, а в основном - через дебаг и стейдж
Формирование отчетов: Xray (Jira), Allure Report, Report Portal

## Примеры, что проверяем

Сценарии, которые проходят через несколько ролей (в CRM сменили статус заказа, в ЛК пользователя подгрузился новый статус)

Бизнес-процессы (создание заказа → оплата → получение статуса).

## Типичные сценарии

Happy path:
Создать заказ → оплатить → получить подтверждение → проверить в ЛК статус

Негативный сценарий с моком:
Мокнуть платёжный шлюз на 500 (мок апи браузера). Создать заказ → попробовать оплатить → проверить, что на странице отобразились верные данные

Сценарий с eventual consistency:
Отправить команду → подождать 5 секунд (с повтором на странице) → проверить, что данные появились через время

Idempotency:
Отправить два одинаковых запроса со страницы → убедиться, что не создалось все сущности

## Allure

Референсы:

https://habr.com/ru/companies/ru_mts/articles/720692
https://habr.com/ru/companies/clevertec/articles/822583/

## TODO

input, date, select, range, checkbox, radio, file, image, video
button, color, datetime-local, month, week, time, search, password, email, number, textarea

search with suggestion
date picker/календарь (не нативный)
multi select/теги c чипами
Drag-and-Drop
File Upload with Preview
Modal / Dialog / Popup
Tabs / Accordion / Collapsible
Pagination / Пагинация (именно в плане поиска на странице нужного элемента)
Infinite Scroll / Ленивая загрузка
Toast / Notification / Snackbar
Progress Bar / Индикатор загрузки
Slider with Range (Double Slider)
Tree Select / Иерархический выбор
Chip / Tag Input
Captcha / reCAPTCHA
Tooltip / Popover
Stepper / Wizard (Пошаговый мастер)


Суть:
В пайплайне (GitLab CI / GitHub Actions) запускай Playwright не последовательно, а с шардингом:
npx playwright test --shard=1/3

browser - singleton
context/page - scoped для каждого теста и создаются из этого singleton browser

- тест привычных простых страниц и гридов
- дебоунс ввод понемногу (чтобы тестить кол-во запросов)
- перехват трафика и мок апи запросов
- проверка что браузер делает не больше n запросов
- тест асинхронных вебсокетов в чате
- тест загрузки видео