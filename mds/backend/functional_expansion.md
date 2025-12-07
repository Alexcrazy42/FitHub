по дефолту: zero-downtime migration, ef core

не ломать текущий код
не вызывать даунтайм
перенести данные из старой схемы в новую
постепенно переключить EF Core и бизнес-логику на новую структуру
убрать старый столбец/связь в самом конце

все, что написано может происходить в несколько этапов/релизов

1. 1 ко многим -> многие ко многие

шаг 1. добавляем новую ManyToMany таблицу, не трогая старую
шаг 2. обновляем модель, сохраняя обратную совместимость. при этом около старого поля OneToMany помечаем флагом, было ли оно перенесено в новую структуру
шаг 3. пишем новый код под новую структуру. при этом обязательно "compability".

важные правила:
1. дублирующая запись
при создании или обновлении Visitor: пишем в старую таблицу Visitor.GymId + пишем в новую таблицу VisitorGyms
чтение должно быть tolerant (читаем новую, fallback на старую)

```
public GymId? GetDefaultGym(Visitor visitor)
{
    // 1) Новый источник
    var defaultGym = visitor.Gyms.FirstOrDefault(x => x.IsDefaultGym);
    if (defaultGym != null)
        return defaultGym.GymId;

    // 2) Fallback на старое поле
    return visitor.GymId;
}
```

Это гарантирует:
✔ новый код работает
✔ старые данные поддерживаются
✔ миграция данных в фоне не ломает приложение

шаг 4. Переносим все в новую структуру. возможно даже неплохо это джобой
```
INSERT INTO VisitorGyms (VisitorId, GymId, IsDefault)
SELECT Id AS VisitorId,
        GymId AS GymId,
        1 AS IsDefault
FROM Visitors
WHERE GymId IS NOT NULL and Transfered = false
```
шаг 5. переносим все инстансы на "терпимую версию"
шаг 6. отключаем двойную запись, добившись 100% переноса
шаг 7. удаляем старое поле из модели EF

но все это можно упростить, просто введя понятие - дефолтное значение. аля "основной зал" и "все остальные". тогда все что нужно сделать -> добавить новую связь и все

2. многие ко многим без доп полей -> многие ко многим с доп полями
это переход от shadow table -> полноценная сущность


Проблема:
EF Core не умеет автоматически преобразовывать shadow-таблицу в join-entity.

Когда ты изменяешь модель EF так, что:

```
HasMany().WithMany()
```

становится:

```
HasMany().WithMany().UsingEntity<VisitorGym>()
```

EF попытается создать новую таблицу, потому что shadow-table не совпадает с тем, что ты определил вручную.
➡ нужно грамотно перевести структуру, не теряя данные.

шаги:

1. создать join entity, например VisitorGymRelation
2. обновить модели Visitor и Gym
3. Настраивание explicit many-to-many:

```
builder.Entity<VisitorGym>(e =>
{
    e.HasKey(x => new { x.VisitorId, x.GymId });

    e.HasOne(x => x.Visitor)
        .WithMany(v => v.Gyms)
        .HasForeignKey(x => x.VisitorId);

    e.HasOne(x => x.Gym)
        .WithMany(v => v.Visitors)
        .HasForeignKey(x => x.GymId);
});
```

4. создание миграции, НО:
EF увидит новую сущность
увидит старую shadow-table
подумает, что это разные вещи
попытается создать НОВУЮ таблицу VisitorGym, и возможно удалить старую

5. ❗Правильно модифицируешь автоматически сгенерированную миграцию

По умолчанию EF создаст CreateTable("VisitorGym").

- Переименовать существующую shadow-table, если её имя не совпадает с новым
- Изменить миграцию: удалить CreateTable, заменить его на RenameTable (если требуется), добавить новые столбцы


```
protected override void Up(MigrationBuilder migrationBuilder)
{
    // EF создал бы CreateTable — мы его УДАЛЯЕМ

    // 1. Добавляем недостающие столбцы
    migrationBuilder.AddColumn<bool>(
        name: "IsDefault",
        table: "VisitorGym",
        nullable: false,
        defaultValue: false);

    migrationBuilder.AddColumn<int>(
        name: "VisitCount",
        table: "VisitorGym",
        nullable: false,
        defaultValue: 0);
}
```

6. Применяешь миграцию
Данные сохраняются:
(VisitorId, GymId) остаются
новые поля появляются
join-table становится «настоящей»

7. Переводишь код на новую модель

visitor.Gyms.Add(new VisitorGym 
{
    VisitorId = visitor.Id,
    GymId = gym.Id,
    IsDefault = true,
});

.Include(v => v.Gyms).ThenInclude(g => g.Gym)