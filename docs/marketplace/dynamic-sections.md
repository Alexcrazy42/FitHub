# Dynamic sections: управление боковой панелью по контексту пользователя

Документ описывает, как сейчас работает боковая панель FitHub и как её можно развить до динамических секций: пункты меню зависят не только от роли, но и от текущего зала, прав, включённых модулей, состояния пользователя и бизнес-контекста.

## Как работает сейчас

Сайдбар находится в `frontend/src/components/Sidebar/Sidebar.tsx`.

Текущий алгоритм:

```text
useAuth()
  -> user.currentRole
  -> выбрать один menuConfig
  -> transformToAntdMenu()
  -> Ant Design Menu
```

Выбор меню:

```text
CmsAdmin    -> adminMenuConfig
GymVisitor  -> userMenuConfig
GymAdmin    -> gymAdminMenuConfig
```

Конфиги лежат в:

```text
frontend/src/routes/adminMenuConfig.tsx
frontend/src/routes/gymAdminMenuConfig.tsx
frontend/src/routes/userMenuConfig.tsx
```

`MenuItem` сейчас выглядит так:

```ts
export interface MenuItem {
  key: string;
  label: string;
  icon?: React.ReactNode;
  path?: string;
  element?: React.ReactNode;
  children?: MenuItem[];
}
```

Эти же menu config используются роутером в `frontend/src/routes/router.tsx`: `extractRoutesFromMenu()` проходит по пунктам меню и создаёт `RouteObject`. То есть сейчас меню и маршруты связаны через один объект.

Авторизация маршрутов работает отдельно через `ProtectedRoute`:

```text
/admin/*      -> CmsAdmin
/user/*       -> GymVisitor
/gym-admin/*  -> GymAdmin
```

При логине, если у пользователя несколько ролей, `Login.tsx` показывает выбор роли. После выбора роль сохраняется как `user.currentRole` в `AuthProvider`.

Для `GymAdmin` `AuthProvider` дополнительно грузит:

```text
GET v1/gym-admins/me
  -> gymAdmin
  -> currentGym = gymAdmin.gyms[0]
```

Это уже хорошая точка для будущей динамики: сайдбар может зависеть не только от роли, но и от `currentGym`.

## Что в текущем подходе хорошо

- Простая схема: одна роль -> один menu config.
- Меню и route config не расходятся, потому что routes извлекаются из меню.
- Добавить новый пункт для роли легко: добавить item в нужный `*MenuConfig.tsx`.
- `ProtectedRoute` не даёт открыть чужие role-level разделы напрямую по URL.
- Есть основа для multi-role входа: `user.roles` и `user.currentRole`.

## Ограничения текущей реализации

### 1. Только role-based меню

Сейчас сайдбар умеет различать только `currentRole`. Этого мало для сценариев вроде:

- `GymAdmin` управляет несколькими залами;
- у одного `GymAdmin` есть доступ к расписанию, но нет доступа к финансам;
- marketplace включён не для всех залов;
- видео-модуль доступен только после настройки storage;
- новый раздел включён feature flag'ом;
- пользователь заблокирован или не завершил onboarding.

### 2. Нет permission/capability слоя

Роль отвечает на вопрос "кто пользователь", но не всегда отвечает на вопрос "что ему можно делать".

Например оба пользователя могут быть `GymAdmin`, но:

```text
GymAdmin A -> может управлять расписанием и тренерами
GymAdmin B -> может только смотреть отчёты
```

Сейчас такой разницы в сайдбаре выразить нельзя без хардкода.

### 3. Нет Trainer menu branch

В `UserRole` есть `Trainer`, и в `roleRoutes` есть `/trainer`, но в `Sidebar.tsx` нет выбора `trainerMenuConfig`, а в `router.tsx` нет `/trainer/*` ветки. Значит пользователь с ролью `Trainer` после логина потенциально попадает в маршрут, для которого нет полноценного layout/menu flow.

### 4. `key` и `selectedKeys` могут расходиться

`Sidebar.tsx` передаёт:

```tsx
selectedKeys={[location.pathname]}
```

Но в `adminMenuConfig` и `gymAdminMenuConfig` ключи выглядят так:

```text
dashboard
gyms
equipments
```

А `location.pathname` выглядит так:

```text
/admin/home
/gym-admin/schedule
```

Из-за этого активный пункт меню может не подсвечиваться. Для стабильности лучше делать `key = path` для route-пунктов или вычислять selected key через поиск item по `path`.

### 5. Navigation metadata смешана с React element

`MenuItem` содержит и navigation metadata, и `element`.

Это удобно для маленького проекта, но в будущем начинает мешать:

- sidebar должен знать только label/icon/path/guards;
- router должен знать component/layout/loaders;
- feature flags и permissions проще проверять на уровне route registry;
- badge/count/loading-состояния не должны тянуть за собой route element.

### 6. Нет server-side navigation contract

Backend сейчас не говорит frontend'у, какие модули доступны пользователю. Frontend сам решает по роли.

Для демо это нормально, но если появятся тарифы, permissions, настройки зала, marketplace rollout или module flags, frontend будет вынужден дублировать backend-логику.

## Целевая идея: dynamic sections

Вместо "роль -> один массив пунктов" лучше думать так:

```text
UserNavigationContext
  -> registry доступных разделов
  -> фильтрация по guards
  -> sidebar sections
```

Контекст:

```ts
type UserNavigationContext = {
  user: {
    id: string;
    roles: UserRole[];
    currentRole: UserRole;
  };
  currentGym?: {
    id: string;
    status?: 'Active' | 'Blocked' | 'SetupRequired';
    modules?: string[];
  };
  permissions: string[];
  featureFlags: string[];
  counters?: {
    unreadChats?: number;
    pendingOrders?: number;
    expiringReservations?: number;
  };
};
```

Пункт меню:

```ts
type NavigationItem = {
  id: string;
  label: string;
  path: string;
  icon: React.ReactNode;
  allowedRoles?: UserRole[];
  requiredPermissions?: string[];
  requiredFeatures?: string[];
  requiredGymModules?: string[];
  hidden?: (ctx: UserNavigationContext) => boolean;
  disabled?: (ctx: UserNavigationContext) => boolean;
  badge?: (ctx: UserNavigationContext) => number | string | null;
};
```

Секция:

```ts
type NavigationSection = {
  id: string;
  label?: string;
  order: number;
  items: NavigationItem[];
};
```

Фильтрация:

```text
1. Взять registry для currentRole.
2. Убрать item, если currentRole не входит в allowedRoles.
3. Убрать item, если нет requiredPermissions.
4. Убрать item, если выключен requiredFeature.
5. Убрать item, если у currentGym нет requiredGymModules.
6. Убрать item, если hidden(ctx) вернул true.
7. Оставить disabled item только если бизнесу нужно показать недоступный раздел с причиной.
8. Убрать пустые sections.
```

## Что может управлять сайдбаром

### Роль

Базовый уровень.

```text
CmsAdmin   -> управление сетью, залами, пользователями, каталогом marketplace
GymAdmin   -> управление конкретным залом, расписание, оборудование, покупка товаров
GymVisitor -> профиль, свои тренировки/заказы
Trainer    -> расписание тренера, клиенты, видео, чат
```

### Текущий зал

Для `GymAdmin` меню должно учитывать `currentGym`.

Примеры:

- если у пользователя несколько залов, в header нужен gym switcher;
- при смене `currentGym` sidebar пересчитывается;
- если зал неактивен, скрыть операции покупки и редактирования расписания;
- если для зала не включён marketplace, не показывать marketplace-раздел;
- если зал не настроил адрес/координаты, показывать настройку зала перед доставкой.

### Permissions

Permissions нужны, когда одной роли недостаточно.

Примеры:

```text
marketplace.catalog.manage
marketplace.orders.view
marketplace.orders.assemble
marketplace.checkout.create
gym.schedule.manage
gym.equipment.manage
gym.users.manage
chat.read
video.manage
```

`CmsAdmin` может видеть каталог marketplace только при `marketplace.catalog.manage`, а `GymAdmin` может видеть покупку товаров только при `marketplace.checkout.create`.

### Feature flags

Feature flags нужны для постепенного включения модулей.

Примеры:

```text
marketplace
marketplace.checkout
marketplace.delivery
courier-tracking
video-upload
stickers
```

Это особенно полезно для marketplace: каталог можно включить раньше оплаты и доставки.

### Business state

Пункты могут зависеть от состояния процесса.

Примеры:

- показывать "Корзина" только если есть активные items или reservations;
- показывать badge "Заказы" для `CmsAdmin`, если есть оплаченные заказы на сборку;
- показывать badge "Оплата" для `GymAdmin`, если есть резервация, которая скоро истечёт;
- скрывать "Доставка" до тех пор, пока не появятся оплаченные заказы;
- показывать "Настройка склада", если marketplace включён, но warehouse не настроен.

## Как это применить к marketplace

Для `CmsAdmin`:

```text
Marketplace
  Каталог товаров
  Категории и атрибуты
  Остатки
  Заказы
  Курьеры и доставка
  Настройки склада
```

Условия:

```text
Каталог товаров       -> marketplace + marketplace.catalog.manage
Остатки               -> marketplace + marketplace.inventory.manage
Заказы                -> marketplace + marketplace.orders.view
Курьеры и доставка    -> marketplace.delivery + marketplace.delivery.manage
Настройки склада      -> marketplace + marketplace.settings.manage
```

Для `GymAdmin`:

```text
Marketplace
  Каталог
  Корзина / резервации
  Мои заказы
  Трекинг доставки
```

Условия:

```text
Каталог               -> marketplace + marketplace.catalog.view
Корзина / резервации  -> marketplace.checkout + marketplace.checkout.create
Мои заказы            -> marketplace.orders.view
Трекинг доставки      -> marketplace.delivery + есть активная доставка
```

Если доставка делается позже, её можно не показывать до включения `marketplace.delivery`.

## Рекомендуемый путь улучшения

### Шаг 1. Починить selectedKeys

Для route-items использовать `key = path`.

Пример:

```text
key: '/gym-admin/schedule'
path: '/gym-admin/schedule'
```

Или оставить произвольные `id`, но добавить функцию:

```text
findSelectedMenuKey(menuConfig, location.pathname)
```

### Шаг 2. Добавить metadata guards в MenuItem

Расширить `MenuItem`:

```ts
export interface MenuItem {
  key: string;
  label: string;
  icon?: React.ReactNode;
  path?: string;
  element?: React.ReactNode;
  children?: MenuItem[];
  allowedRoles?: UserRole[];
  requiredPermissions?: string[];
  requiredFeatures?: string[];
  requiredGymModules?: string[];
  hidden?: (ctx: UserNavigationContext) => boolean;
  badge?: (ctx: UserNavigationContext) => number | string | null;
}
```

После этого `Sidebar` не выбирает один config вручную, а вызывает:

```text
buildNavigation(ctx)
```

### Шаг 3. Разделить route registry и sidebar view

Долгосрочно лучше сделать так:

```text
routes/registry.tsx
  -> все route definitions

navigation/registry.tsx
  -> sections/items для sidebar

navigation/buildNavigation.ts
  -> фильтрация по ctx
```

Router и Sidebar используют один источник прав доступа, но не один и тот же UI-объект.

### Шаг 4. Добавить backend capabilities endpoint

Когда появятся permissions и feature flags:

```text
GET /api/v1/me/navigation-context
```

Response:

```json
{
  "userId": "user-id",
  "currentRole": "GymAdmin",
  "permissions": [
    "marketplace.catalog.view",
    "marketplace.checkout.create",
    "chat.read"
  ],
  "featureFlags": [
    "marketplace",
    "marketplace.checkout"
  ],
  "currentGym": {
    "id": "gym-id",
    "status": "Active",
    "modules": ["marketplace", "video"]
  },
  "counters": {
    "unreadChats": 3,
    "pendingOrders": 0,
    "expiringReservations": 1
  }
}
```

Frontend по этому response строит меню. Backend всё равно остаётся источником истины для доступа: скрытый пункт меню не заменяет авторизацию endpoint'ов.

## Как управлять этим с backend

Есть три уровня backend-управления сайдбаром. Их можно вводить постепенно.

### Уровень 1. Backend отдаёт capabilities

Это самый практичный вариант для FitHub.

Backend не говорит frontend'у "нарисуй пункт меню с такой иконкой и таким label". Он отдаёт только то, что является бизнес-истиной:

```text
roles
permissions
featureFlags
gymModules
businessState
counters
```

Frontend сам владеет:

```text
label
icon
path
React element
grouping
visual order
```

Пример backend response:

```json
{
  "currentRole": "GymAdmin",
  "permissions": [
    "gym.schedule.manage",
    "gym.equipment.manage",
    "marketplace.catalog.view",
    "marketplace.checkout.create"
  ],
  "featureFlags": [
    "marketplace",
    "marketplace.checkout"
  ],
  "currentGym": {
    "id": "gym-id",
    "status": "Active",
    "modules": ["marketplace", "video"],
    "setup": {
      "hasAddress": true,
      "hasGeoPoint": true,
      "hasPaymentAccount": true
    }
  },
  "businessState": {
    "hasActiveCart": true,
    "hasActiveReservation": false,
    "hasActiveDelivery": false,
    "warehouseConfigured": false
  },
  "counters": {
    "unreadChats": 4,
    "pendingMarketplaceOrders": 2,
    "expiringReservations": 0
  }
}
```

Frontend registry остаётся статическим, но фильтруется через этот контекст:

```text
Marketplace item visible if:
  featureFlags includes marketplace
  permissions includes marketplace.catalog.view
  currentGym.modules includes marketplace
  currentGym.status = Active
```

Плюсы:

- frontend остаётся нормальным React-приложением;
- backend не знает про иконки и Ant Design;
- легко тестировать guards на frontend;
- backend остаётся источником прав и feature flags;
- меньше риска сломать UI из-за неверного server-driven menu payload.

Минусы:

- frontend всё ещё содержит список потенциальных разделов;
- при добавлении нового раздела нужен frontend deploy.

### Уровень 2. Backend отдаёт navigation policy

На этом уровне backend уже не просто отдаёт permissions, а возвращает готовое решение по доступности известных frontend-разделов.

Frontend и backend договариваются о стабильных ids:

```text
dashboard
gym.users
gym.schedule
marketplace.catalog
marketplace.checkout
marketplace.orders
marketplace.delivery
chat
videos
```

Backend response:

```json
{
  "items": [
    {
      "id": "gym.schedule",
      "visible": true,
      "enabled": true,
      "reason": null,
      "badge": null
    },
    {
      "id": "marketplace.delivery",
      "visible": false,
      "enabled": false,
      "reason": "FeatureDisabled",
      "badge": null
    },
    {
      "id": "marketplace.orders",
      "visible": true,
      "enabled": true,
      "reason": null,
      "badge": 2
    }
  ]
}
```

Frontend registry:

```ts
const navigationRegistry = {
  'marketplace.orders': {
    label: 'Мои заказы',
    path: '/gym-admin/marketplace/orders',
    icon: <ShoppingOutlined />,
    element: <MarketplaceOrdersPage />
  }
};
```

Frontend берёт `id` из backend policy и применяет `visible/enabled/badge`.

Плюсы:

- backend точнее управляет видимостью;
- удобно для сложных бизнес-условий;
- можно показывать disabled пункты с причиной, например "Настройте адрес зала";
- меньше дублирования бизнес-условий на frontend.

Минусы:

- нужен стабильный словарь navigation ids;
- если backend вернул id, которого нет во frontend registry, его нужно игнорировать и логировать;
- frontend и backend должны версионировать этот контракт.

### Уровень 3. Backend отдаёт полностью server-driven menu

Backend возвращает структуру меню:

```json
{
  "sections": [
    {
      "id": "marketplace",
      "label": "Marketplace",
      "items": [
        {
          "id": "marketplace.catalog",
          "label": "Каталог",
          "path": "/gym-admin/marketplace/catalog",
          "icon": "shopping",
          "visible": true,
          "enabled": true,
          "badge": null
        }
      ]
    }
  ]
}
```

Для FitHub я бы не начинал с этого уровня.

Плюсы:

- backend полностью управляет меню;
- можно менять порядок, группы и labels без frontend deploy;
- удобно для white-label или SaaS с разными конфигурациями для разных клиентов.

Минусы:

- backend начинает знать про UI-структуру;
- нужен словарь иконок, локализация, fallback'и;
- path из backend должен соответствовать frontend routes;
- выше риск получить битую навигацию;
- всё равно нельзя создавать новые React-страницы без frontend deploy.

Этот подход имеет смысл только если меню реально должно конфигурироваться из админки или отличаться для разных организаций на уровне labels/order/groups.

## Backend-модель для capabilities

На backend можно завести сервис:

```csharp
public interface IUserNavigationContextService
{
    Task<UserNavigationContextResponse> GetAsync(
        UserId userId,
        UserRole currentRole,
        Guid? currentGymId,
        CancellationToken ct);
}
```

Он собирает данные из нескольких источников:

```text
User/Roles         -> текущая роль и все роли
Permissions        -> что можно делать
FeatureFlags       -> какие модули включены глобально
GymMembership      -> к каким залам есть доступ
GymSettings        -> включены ли модули у конкретного зала
Marketplace        -> есть ли активные резервации/заказы
Chat               -> unread counters
Delivery           -> активная доставка, если модуль включён
```

Endpoint:

```text
GET /api/v1/me/navigation-context?role=GymAdmin&gymId={gymId}
```

Правила:

- `role` должен входить в роли текущего пользователя;
- `gymId` должен быть доступен текущему пользователю;
- если `gymId` не передан для `GymAdmin`, backend может вернуть default/current gym;
- response можно кэшировать коротко, например 30-60 секунд;
- counters можно грузить отдельно, если они дорогие.

Пример DTO:

```csharp
public sealed record UserNavigationContextResponse(
    string CurrentRole,
    IReadOnlyCollection<string> Permissions,
    IReadOnlyCollection<string> FeatureFlags,
    NavigationGymContext? CurrentGym,
    NavigationBusinessState BusinessState,
    NavigationCounters Counters);
```

Для `CmsAdmin` `CurrentGym` может быть `null`, потому что он управляет сетью, а не одним залом.

## Backend авторизация всё равно обязательна

Динамический сайдбар - это UX, а не безопасность.

Даже если пункт меню скрыт, backend endpoint должен проверять доступ:

```text
GET /api/v1/marketplace/admin/orders
  -> requires marketplace.orders.view

POST /api/v1/marketplace/checkout/reservations
  -> requires marketplace.checkout.create

POST /api/v1/gym-admin/schedule
  -> requires gym.schedule.manage
```

Frontend guard нужен, чтобы не показывать лишнее. Backend authorization нужен, чтобы нельзя было открыть endpoint напрямую.

Минимальная схема:

```text
Sidebar guard
  -> скрывает пункт

Route guard
  -> не даёт открыть страницу

API authorization
  -> не даёт выполнить действие
```

Если эти три уровня расходятся, источником истины должен быть backend.

### Шаг 5. Добавить Trainer branch

Так как `Trainer` уже есть в типах, нужно решить:

- либо полноценно добавить `trainerMenuConfig` и `/trainer/*`;
- либо временно убрать/не выдавать роль `Trainer` при логине, пока UI не готов.

## Итоговая рекомендация

Для ближайшей реализации достаточно гибридного подхода:

```text
frontend static navigation registry
  + backend navigation context/capabilities
  + guards на role/permission/feature/currentGym
```

Не нужно сразу делать полностью server-driven меню, где backend присылает label/path/icon. Для FitHub лучше, чтобы frontend владел структурой UI, иконками и route elements, а backend присылал только права, flags, modules и counters.

Так сайдбар останется естественным для React-приложения, но станет достаточно гибким для marketplace, доставки, multi-gym сценариев и будущих permissions.
