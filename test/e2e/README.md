# E2E тесты

В основном пишутся командой QA. Тестирование через браузер.

## Методология

Есть Акторы (Пользователь, Админ, Гость).
Есть Задачи (высокоуровневые действия: AddItemToCart).
Есть Взаимодействия (низкоуровневые: Click, Fill, WaitFor).

Page Object Model - инкапсулируем взаимодействие со страницей в удобные обертки

## Инструменты и окружение

Инструменты: playwright (@playwright/test)
Окружение: на локалке поднятие через docker-compose, а в основном - через дебаг и стейдж
Формирование отчетов: Xray (Jira), Allure Report, Report Portal

Запуск тестов с шардингом: npx playwright test --shard=1/3

browser - singleton на уровне одного worker
context/page - scoped для каждого теста и создаются из этого singleton browser


## Получение элементов со страницы

Мы не хотим переписывать тесты при малейшем изменении верстки, смене языка, смене фреймворка, замене ui библиотеки и тд, поэтому не получаем элемент со страницы через:

1. название внутри элемента
2. классы
3. классы ui библиотек, которые могут меняться со сменой версии
4. CSS-селекторы по структуре
5. nth(0) (порядок элементов)

Вместо этого используем:

1. data-testid
2. При невозможности добавить data-testid добавляем комбинацию ролей: кнопка + название
3. Ищем элементы в родительских элементах с четким  (кнопка в модалке по названию)

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

## UI элементы

input, date, select, range, checkbox, radio, file, image, video
button, color, datetime-local, month, week, time, search, password, email, number, textarea

+ валидация форм

search with suggestion
date picker/календарь (не нативный)
multi select/теги c чипами
Drag-and-Drop
File Upload with Preview
Modal / Dialog / Popup
Tabs / Accordion / Collapsible
Pagination / Пагинация / Filters (именно в плане поиска на странице нужного элемента)
Infinite Scroll / Ленивая загрузка
Toast / Notification / Snackbar
Progress Bar / Индикатор загрузки
Slider with Range (Double Slider)
Tree Select / Иерархический выбор
Chip / Tag Input
Captcha / reCAPTCHA
Tooltip / Popover
Stepper / Wizard (Пошаговый мастер)
Confirmation Dialog / Skeleton Screen / Undo Redo
Rich Text Editor WYSIWYG / Auto-save текста / Character Word Counter / Copy to Clipboard
Breadcrumbs / Sticky Header / Back to Top / Sidebar Drawer

## TODO

- тест привычных простых страниц и гридов +
- дебоунс ввод понемногу (чтобы тестить кол-во запросов) +
- перехват трафика и мок апи запросов +
- тест асинхронных вебсокетов в чате
- тест загрузки видео