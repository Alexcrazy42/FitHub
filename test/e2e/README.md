@testla/screenplay

Суть:

Есть Актеры (Пользователь, Админ, Гость).
Есть Задачи (высокоуровневые действия: AddItemToCart).
Есть Взаимодействия (низкоуровневые: Click, Fill, WaitFor).

В пайплайне (GitLab CI / GitHub Actions) запускай Playwright не последовательно, а с шардингом:
npx playwright test --shard=1/3

