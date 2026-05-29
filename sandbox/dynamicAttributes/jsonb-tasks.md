# Теория

## 1. Операторы

Работа с `jsonb` в PostgreSQL строится вокруг нескольких групп операторов:

- `?` — проверка существования ключа.
- `?|` — проверка существования хотя бы одного ключа из списка.
- `?&` — проверка существования всех ключей из списка.
- `@>` — проверка, содержит ли JSONB указанный фрагмент.
- `<@` — обратная проверка вхождения.
- `->` — извлечение значения как `jsonb`.
- `->>` — извлечение значения как `text`.
- `#>` — извлечение значения по пути как `jsonb`.
- `#>>` — извлечение значения по пути как `text`.
- `@?` — проверка по `jsonpath`, возвращает `true`, если найдено хотя бы одно совпадение.
- `@@` — проверка JSONPath-предиката.

Важно помнить, что `->` и `#>` возвращают JSON-значение, а `->>` и `#>>` — текст. Поэтому операции вроде `jsonb_array_length`, `jsonb_typeof`, `@>`, `?` и JSONPath обычно работают именно с `jsonb`, а не с текстом.

## 2. Функции и отсутствие значений

При работе с JSONB нужно учитывать, что функции по-разному ведут себя при отсутствии ключа, `null` и неправильном типе данных.

- `jsonb_typeof(...)` возвращает тип JSON-значения, например `object`, `array`, `string`, `number`, `boolean`, `null`.
- Если путь не существует, результат часто будет `NULL`.
- `jsonb_array_length(...)` требует именно массив; если передать объект, `null` или текст, будет ошибка.
- `jsonb_set(...)` позволяет заменить или добавить значение по пути; параметр `create_missing` управляет тем, создавать ли отсутствующие элементы

Отдельно важно различать JSON `null` и SQL `NULL`: это не одно и то же, и сравнивать их нужно аккуратно.

## 3. JSONPath и подавление ошибок

Операторы `@?` и `@@` позволяют писать запросы по JSONPath и при этом подавляют часть типичных ошибок, связанных с разной структурой документов.

Они умеют спокойно переживать:

- отсутствие поля;
- отсутствие элемента массива;
- несоответствие типа;
- некоторые ошибки чисел и дат.

Это особенно полезно, когда в одной таблице лежат JSON-документы с немного разной структурой. Вместо громоздких `EXISTS` и `jsonb_typeof(...)` можно написать компактный JSONPath-запрос.

## 4. Индексы

Для `jsonb` чаще всего используют два подхода к индексированию:

- `GIN` по всему JSONB-столбцу — хорошо подходит для `@>`, `?`, `?|`, `?&`, а также `@?` и `@@`
- `B-tree` по выражению — подходит, когда ты часто фильтруешь по конкретному полю, например по `Payload->>'Name'`.

Если запросы часто обращаются к одному и тому же полю, выражение лучше индексировать отдельно. Если же поиск идёт по содержимому JSON в целом, обычно нужен `GIN`

***

## Добавление записи

Пример вставки записи в таблицу:

```sql
INSERT INTO public."Log1" ("Id", "Payload")
VALUES (
  gen_random_uuid(),
  '{"Name": "Joe", "Age": 25, "Orders": [{"Price": 10}, {"Price": 1}]}'::jsonb
);
```

```sql
INSERT INTO public."Log1" ("Id", "Payload")
SELECT
    gen_random_uuid(),
    jsonb_build_object(
        'Name', 'User_' || gs,
        'Age', 18 + (gs % 60),
        'Orders', jsonb_build_array(
            jsonb_build_object('Price', (gs % 200)),
            jsonb_build_object('Price', ((gs * 3) % 200))
        ),
        'Status', CASE WHEN gs % 2 = 0 THEN 'Active' ELSE 'Inactive' END,
        'City', CASE
            WHEN gs % 3 = 0 THEN 'Berlin'
            WHEN gs % 3 = 1 THEN 'Paris'
            ELSE 'Tokyo'
        END
    )
FROM generate_series(1, 5000000) AS gs;
```

Здесь `Payload` передаётся как JSONB-литерал, а `gen_random_uuid()` используется для генерации идентификатора.
***

## Базовые упражнения

### 1. Проверка существования ключа

```sql
SELECT *
FROM "Log1"
WHERE "Payload" ? 'Age';
```

Этот запрос ищет записи, где в `Payload` есть ключ `Age`.

### 2. Проверка нескольких ключей

```sql
SELECT *
FROM "Log1"
WHERE "Payload" ?| array['Age', 'Address'];
```

Этот вариант возвращает строки, где есть хотя бы один из ключей. Если нужно проверить, что есть все ключи, используй `?&`.

### 3. Проверка вхождения объекта

```sql
SELECT *
FROM "Log1"
WHERE "Payload" @> '{"Name":"Joe","Age":25}'::jsonb;
```

План запроса:

```sql
Bitmap Heap Scan on "Log1"  (cost=92.72..2010.23 rows=500 width=198) (actual time=0.150..0.152 rows=4 loops=1)
  Recheck Cond: ("Payload" @> '{"Age": 25, "Name": "Joe"}'::jsonb)
  Heap Blocks: exact=1
  ->  Bitmap Index Scan on ix_log1_payload_gin  (cost=0.00..92.60 rows=500 width=0) (actual time=0.144..0.144 rows=4 loops=1)
        Index Cond: ("Payload" @> '{"Age": 25, "Name": "Joe"}'::jsonb)
Planning Time: 0.114 ms
Execution Time: 0.168 ms
```

Здесь ищутся записи, где JSONB содержит указанный фрагмент.

### 4. Обратная проверка вхождения

```sql
SELECT *
FROM "Log1"
WHERE '{"Name":"Joe","Age":25}'::jsonb <@ "Payload";
```

Это обратная проверка: левый JSON должен входить в правый.

### 5. Получение строкового поля

```sql
SELECT *
FROM "Log1"
WHERE "Payload"->>'Name' = 'Joe';
```

```sql
Bitmap Heap Scan on "Log1"  (cost=582.18..65247.01 rows=25000 width=198) (actual time=0.028..0.029 rows=6 loops=1)
  Recheck Cond: (("Payload" ->> 'Name'::text) = 'Joe'::text)
  Heap Blocks: exact=1
  ->  Bitmap Index Scan on ix_log1_payload_name  (cost=0.00..575.93 rows=25000 width=0) (actual time=0.023..0.023 rows=6 loops=1)
        Index Cond: (("Payload" ->> 'Name'::text) = 'Joe'::text)
Planning Time: 0.067 ms
Execution Time: 0.047 ms
```

`->>` извлекает значение как текст, поэтому сравнение идёт со строкой

### 6. Получение вложенного объекта

```sql
SELECT "Payload"->'Orders'
FROM "Log1";
```

Этот запрос возвращает массив `Orders` целиком как JSONB.

Если нужен первый элемент массива:

```sql
SELECT "Payload"->'Orders'->0
FROM "Log1";
```

***

## Упражнения по массивам

### 7. Длина массива

```sql
SELECT *
FROM "Log1"
WHERE jsonb_array_length("Payload"->'Orders') = 2;
```

Функция `jsonb_array_length` работает только с JSONB-массивом, поэтому здесь обязательно использовать `->`, а не `->>`.
### 8. Доступ к элементу массива по индексу

```sql
SELECT *
FROM "Log1"
WHERE jsonb_typeof("Payload" #> '{Orders,0,Price}') = 'number'
  AND ("Payload" #>> '{Orders,0,Price}')::int = 1;
```

Здесь используется путь к первому элементу массива `Orders` и далее к полю `Price`.

### 9. Фильтрация по любому элементу массива

```sql
SELECT *
FROM "Log1"
WHERE EXISTS (
    SELECT 1
    FROM jsonb_array_elements("Payload"->'Orders') AS order_item
    WHERE jsonb_typeof(order_item->'Price') = 'number'
      AND (order_item->>'Price')::int >= 100
);
```

Этот запрос находит строки, где хотя бы один заказ подходит под условие.

Через JSONPath это можно написать короче:

```sql
SELECT *
FROM "Log1"
WHERE "Payload" @? '$.Orders[*] ? (@.Price >= 100)';
```

### 10. Фильтрация по всем элементам

```sql
SELECT *
FROM "Log1"
WHERE NOT EXISTS (
    SELECT 1
    FROM jsonb_array_elements("Payload"->'Orders') AS order_item
    WHERE jsonb_typeof(order_item->'Price') = 'number'
      AND (order_item->>'Price')::int < 0
);
```

Этот вариант означает: “нет ни одного заказа с отрицательной ценой”.

Через JSONPath это можно выразить так:

```sql
SELECT *
FROM "Log1"
WHERE NOT ("Payload" @? '$.Orders[*] ? (@.Price < 0)');
```

***

## Упражнения по типам и проверкам

### 11. Проверка типа значения

```sql
SELECT *
FROM "Log1"
WHERE jsonb_typeof("Payload"->'Age') = 'number'
  AND jsonb_typeof("Payload"->'Name') = 'string';
```

Такой запрос полезен, если типы данных в JSON могут “плавать”.

### 12. Проверка на отсутствие поля

```sql
SELECT *
FROM "Log1"
WHERE NOT ("Payload" ? 'Orders');
```

Этот запрос ищет записи, где ключ `Orders` отсутствует.

### 13. Проверка пустого массива

```sql
SELECT *
FROM "Log1"
WHERE "Payload" ? 'Orders'
  AND jsonb_array_length("Payload"->'Orders') = 0;
```

Важно отличать пустой массив от отсутствующего поля

***

## Упражнения по путям

### 14. Доступ к вложенному пути

```sql
SELECT "Payload" #>> '{Orders,0,Price}'
FROM "Log1";
```

Этот вариант возвращает значение по пути в виде текста.

### 15. Фильтрация по вложенному пути

```sql
SELECT *
FROM "Log1"
WHERE "Payload" #>> '{Address,City}' = 'Berlin';
```

Добавили индекс, получаем запрос:

```sql
Bitmap Heap Scan on "Log1"  (cost=282.18..64947.01 rows=25000 width=198) (actual time=0.189..0.189 rows=0 loops=1)
  Recheck Cond: (("Payload" #>> '{Age}'::text[]) = 'Berlin'::text)
  ->  Bitmap Index Scan on ix_log1_payload_age_jsonb  (cost=0.00..275.93 rows=25000 width=0) (actual time=0.187..0.187 rows=0 loops=1)
        Index Cond: (("Payload" #>> '{Age}'::text[]) = 'Berlin'::text)
Planning Time: 0.070 ms
Execution Time: 0.206 ms
```

Здесь поиск идёт по глубоко вложенному полю.

### 16. Сравнение числового значения по пути

```sql
SELECT *
FROM "Log1"
WHERE ("Payload" #>> '{Orders,1,Price}')::int >= 100;
```

Если значение хранится как текст или число в JSON, его можно извлечь через `#>>` и привести к `int`.

***

## Упражнения на модификацию JSON

### 17. Обновление одного поля

```sql
UPDATE "Log1"
SET "Payload" = jsonb_set("Payload", '{Age}', '30'::jsonb, true)
WHERE "Id" = '7c55e1e4-81dd-41b1-abe7-0ee3986a4a28';
```

`jsonb_set` заменяет значение по указанному пути. Если `create_missing = true`, отсутствующее поле будет создано.

### 18. Добавление нового поля

```sql
UPDATE "Log1"
SET "Payload" = jsonb_set("Payload", '{Status}', '"Active"'::jsonb, true)
WHERE "Id" = '7c55e1e4-81dd-41b1-abe7-0ee3986a4a28';
```

Здесь добавляется новое поле `Status` со строковым значением `Active`.

### 19. Изменение элемента массива

```sql
UPDATE "Log1"
SET "Payload" = jsonb_set("Payload", '{Orders,1,Price}', '31'::jsonb, true)
WHERE "Id" = '7c55e1e4-81dd-41b1-abe7-0ee3986a4a28';
```

Этот запрос изменяет цену второго элемента массива `Orders`.

***

## Упражнения по JSONPath

### 20. Проверка хотя бы одного совпадения

```sql
SELECT *
FROM "Log1"
WHERE "Payload" @? '$.Orders[*] ? (@.Price >= 100)';
```

Оператор `@?` возвращает `true`, если JSONPath находит хотя бы один подходящий элемент.

### 21. Проверка булевого предиката

```sql
SELECT *
FROM "Log1"
WHERE "Payload" @@ '$.Name == "Joe" && $.Age > 20';
```

Оператор `@@` проверяет истинность JSONPath-предиката

### 22. Проверка вложенного массива через JSONPath

```sql
SELECT *
FROM "Log1"
WHERE "Payload" @? '$.Orders[*] ? (@.Price >= 10 && @.Price <= 50)';
```

Это поиск по диапазону значений внутри массива.

***

## Упражнения на производительность

### 23. Индекс по часто используемому полю

```sql
CREATE INDEX ix_log1_payload_name
ON "Log1" USING btree (("Payload"->>'Name'));
```

```sql
CREATE INDEX concurrently ix_log1_payload_age
ON "Log1" USING btree (("Payload"->>'Age'));
```

Такой индекс ускоряет поиск по конкретному значению в поле `Name`.

### 24. GIN-индекс по `jsonb`

```sql
CREATE INDEX ix_log1_payload_gin
ON "Log1" USING gin ("Payload");
```

GIN-индекс полезен для операторов `@>`, `?`, `?|`, `?&`, а также JSONPath-поиска `@?` и `@@`

### 25. Сравнение подходов

Для практики полезно сравнить:
- `@>`
- `#>>`
- `jsonb_array_elements(...) + EXISTS`
- `@?`

Разные операторы и функции по-разному работают с индексами и по-разному ведут себя на “грязных” данных.


1. @>

```sql
EXPLAIN ANALYZE
SELECT *
FROM "Log1"
WHERE "Payload" @> '{"Name":"Joe","Age":25}'::jsonb;
```

```sql
Bitmap Heap Scan on "Log1"  (cost=96.97..2014.48 rows=500 width=198) (actual time=3.929..3.936 rows=4 loops=1)
  Recheck Cond: ("Payload" @> '{"Age": 25, "Name": "Joe"}'::jsonb)
  Heap Blocks: exact=1
  ->  Bitmap Index Scan on ix_log1_payload_gin  (cost=0.00..96.85 rows=500 width=0) (actual time=3.902..3.902 rows=4 loops=1)
        Index Cond: ("Payload" @> '{"Age": 25, "Name": "Joe"}'::jsonb)
Planning Time: 0.754 ms
Execution Time: 4.031 ms
```

2. #>>

```sql
EXPLAIN ANALYZE
SELECT *
FROM "Log1"
WHERE "Payload"->>'Name' = 'Joe';
```

```sql
Bitmap Heap Scan on "Log1"  (cost=582.18..65247.01 rows=25000 width=198) (actual time=0.098..0.101 rows=6 loops=1)
  Recheck Cond: (("Payload" ->> 'Name'::text) = 'Joe'::text)
  Heap Blocks: exact=1
  ->  Bitmap Index Scan on ix_log1_payload_name  (cost=0.00..575.93 rows=25000 width=0) (actual time=0.085..0.086 rows=8 loops=1)
        Index Cond: (("Payload" ->> 'Name'::text) = 'Joe'::text)
Planning Time: 0.225 ms
Execution Time: 0.137 ms
```

3. jsonb_array_elements(...) + EXISTS

```sql
EXPLAIN ANALYZE
SELECT *
FROM "Log1"
WHERE EXISTS (
    SELECT 1
    FROM jsonb_array_elements("Payload"->'Orders') AS order_item
    WHERE (order_item->>'Price')::int >= 100
    and jsonb_typeof(order_item->'Price') = 'number'
);
```

```sql
Seq Scan on "Log1"  (cost=0.00..13967877.36 rows=2500004 width=198) (actual time=144.141..17961.856 rows=3325000 loops=1)
  Filter: (SubPlan 1)
  Rows Removed by Filter: 1675007
  SubPlan 1
    ->  Function Scan on jsonb_array_elements order_item  (cost=0.01..2.76 rows=1 width=0) (actual time=0.003..0.003 rows=1 loops=5000007)
          Filter: ((jsonb_typeof((value -> 'Price'::text)) = 'number'::text) AND (((value ->> 'Price'::text))::integer >= 100))
          Rows Removed by Filter: 1
Planning Time: 0.226 ms
JIT:
  Functions: 8
  Options: Inlining true, Optimization true, Expressions true, Deforming true
  Timing: Generation 1.090 ms, Inlining 32.312 ms, Optimization 80.113 ms, Emission 31.417 ms, Total 144.933 ms
Execution Time: 18181.891 ms
```

4. @?

```sql
EXPLAIN ANALYZE
SELECT *
FROM "Log1"
WHERE "Payload" @? '$.Orders[*] ? (@.Price >= 100)';
```

```sql
Seq Scan on "Log1"  (cost=0.00..205358.09 rows=3131318 width=198) (actual time=3.365..5482.345 rows=3325000 loops=1)
  Filter: ("Payload" @? '$."Orders"[*]?(@."Price" >= 100)'::jsonpath)
  Rows Removed by Filter: 1675007
Planning Time: 0.272 ms
JIT:
  Functions: 2
  Options: Inlining false, Optimization false, Expressions true, Deforming true
  Timing: Generation 0.317 ms, Inlining 0.000 ms, Optimization 0.298 ms, Emission 2.879 ms, Total 3.494 ms
Execution Time: 5694.734 ms
```

5. 