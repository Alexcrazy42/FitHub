# PostgreSQL EXPLAIN ANALYZE

`EXPLAIN ANALYZE` показывает не только **какой план выбрал PostgreSQL**, но и **как он реально выполнился**. Это главное: по `EXPLAIN` ты видишь намерение планировщика, по `EXPLAIN ANALYZE` — реальное поведение запроса. [sql-ex](https://sql-ex.ru/blogs/?%2FCupersposobnosti_EXPLAIN_v_PostgreSQL.html)
Для backend-разработчика это особенно важно в search/catalog/faceted queries, где маленькая ошибка в индексе или join strategy может превратить миллисекунды в секунды. [crafthomelab](https://crafthomelab.ru/posts/2025/09/06-postgresql-explain-analyze/)

## Как читать план
План читается **снизу вверх**: нижние узлы поставляют данные, верхние их преобразуют, сортируют, агрегируют и возвращают наружу. [postgrespro](https://postgrespro.ru/docs/postgrespro/current/using-explain)
Первое, на что смотри: **какой узел реально съел время**, где большой разрыв между `rows` и фактом, и где запрос делает лишнюю работу. [habr](https://habr.com/ru/companies/ppr/articles/978100/)

## Базовые поля плана

### cost
`cost=...` — это не миллисекунды, а **внутренняя оценка планировщика**.  
Первое число — стоимость до первой строки, второе — стоимость до конца узла. [sql-ex](https://sql-ex.ru/blogs/?%2FCupersposobnosti_EXPLAIN_v_PostgreSQL.html)
То есть `cost=100..200` не значит “200 мс”; это просто числа в условных единицах, которые PostgreSQL использует для сравнения альтернативных планов. [postgrespro](https://postgrespro.ru/docs/postgresql/11/sql-explain)

### rows
`rows=...` — сколько строк планировщик **ожидал** получить из узла. [postgrespro](https://postgrespro.ru/docs/postgrespro/current/using-explain)
Если в реальности сильно меньше или сильно больше — это сигнал, что статистика плохая, селективность неверно оценена, или запрос устроен так, что planner не может нормально угадать. [proselyte](https://proselyte.net/postgres-for-devs/)

### width
`width=...` — оценка среднего размера выходной строки в байтах. [sql-ex](https://sql-ex.ru/blogs/?%2FCupersposobnosti_EXPLAIN_v_PostgreSQL.html)
Это важно для понимания стоимости сортировок, join’ов и передачи данных между узлами, особенно в широких таблицах и сложных marketplace-запросах.

### actual time
`actual time=a..b` — реальное время выполнения узла:  
- первое число — когда узел выдал **первую** строку,  
- второе — когда узел завершил выдачу **последней** строки. [stackoverflow](https://stackoverflow.com/questions/78082914/exact-meaning-of-actual-times-in-postgresql-explain-analyze)
Это уже реальные миллисекунды, и именно они важны для профилирования. [cybertec-postgresql](https://www.cybertec-postgresql.com/en/how-to-interpret-postgresql-explain-analyze-output/)

### loops
`loops` — сколько раз узел был выполнен. [postgresql](https://www.postgresql.org/message-id/CA+wPC0OXdN+_8HxhYMWyV9nfD8v+xChvLROiWCiRo=qSve2MjQ@mail.gmail.com)
Если `loops=1`, то `actual time` читается почти напрямую. Если `loops=1000`, то это уже средние значения на один проход, и реальная суммарная стоимость может быть очень большой. [stackoverflow](https://stackoverflow.com/questions/78082914/exact-meaning-of-actual-times-in-postgresql-explain-analyze)

### Planning Time
`Planning Time` — сколько времени ушло на построение плана. [postgrespro](https://postgrespro.ru/docs/postgresql/11/sql-explain)
Обычно это мало по сравнению с выполнением, но на очень сложных запросах и огромных схемах тоже бывает заметно.

### Execution Time
`Execution Time` — полное время выполнения запроса после старта исполнения. [postgrespro](https://postgrespro.ru/docs/postgrespro/current/using-explain)
Если для каталога или фасетов это секунды — значит, запрос уже не подходит для интерактивного UI.

## Основные узлы

### Seq Scan
`Seq Scan` — последовательное чтение всей таблицы. [stackoverflow](https://stackoverflow.com/questions/410586/what-is-the-difference-between-seq-scan-and-bitmap-heap-scan-in-postgres)
Он хорош, когда таблица маленькая или нужно прочитать значительную её часть.  
Плох он тогда, когда по таблице огромный объём, а совпадений мало: тогда ты читаешь слишком много лишнего. [cybertec-postgresql](https://www.cybertec-postgresql.com/en/join-strategies-and-performance-in-postgresql/)

Типичные проблемы:
- отсутствие подходящего индекса;
- низкая селективность;
- устаревшая статистика.

Оптимизация:
- индекс;
- более селективный фильтр;
- partitioning;
- partial index.

### Index Scan
`Index Scan` — PostgreSQL находит строки через индекс и потом читает таблицу по найденным ссылкам. [percona](https://www.percona.com/blog/one-index-three-different-postgresql-scan-types-bitmap-index-and-index-only/)
Хорош для точечных выборок и небольшого числа строк.  
Плохо становится, когда найдено слишком много строк и начинается куча случайных обращений к heap.

Оптимизация:
- правильный индекс;
- снижение числа возвращаемых строк;
- иногда лучше `Bitmap Heap Scan`.

### Index Only Scan
`Index Only Scan` — чтение только из индекса без обращения к таблице, если все нужные данные есть в индексе и visibility map позволяет не ходить в heap. [postgresql](https://www.postgresql.org/docs/current/using-explain.html)
Это часто очень быстрый вариант, особенно для count-like запросов и проверок существования.

Проблемы:
- не все нужные колонки покрыты индексом;
- visibility map не позволяет избежать heap fetches.

### Bitmap Index Scan
`Bitmap Index Scan` — индекс ищет подходящие TID/строки и складывает их в bitmap. [pganalyze](https://pganalyze.com/docs/explain/scan-nodes/bitmap-heap-scan)
Это подготовительный этап перед `Bitmap Heap Scan`.

### Bitmap Heap Scan
`Bitmap Heap Scan` — PostgreSQL забирает строки из таблицы пачкой по bitmap, а не по одной. [stackoverflow](https://stackoverflow.com/questions/410586/what-is-the-difference-between-seq-scan-and-bitmap-heap-scan-in-postgres)
Это полезно, когда строк не очень мало и не очень много: planner может сгруппировать чтение по страницам и уменьшить случайные обращения к диску. [eliasdorneles](https://eliasdorneles.com/til/posts/about-bitmap-heap-scan-on-potgresql-query-plan/)

Типичные проблемы:
- lossy bitmap;
- `Recheck Cond`;
- нехватка `work_mem`.

Оптимизация:
- более селективный индекс;
- увеличить `work_mem`;
- упростить predicate;
- часто это нормальный и даже хороший узел для фасетов.

### Nested Loop
`Nested Loop` — для каждой строки внешнего набора PostgreSQL пробует найти совпадения во внутреннем. [postgrespro](https://postgrespro.com/blog/pgsql/5969618)
Отлично работает, если внешний набор маленький и внутренний хорошо индексирован.  
Плохо — если внешний набор большой: тогда получается очень много повторных обращений.

Оптимизация:
- уменьшить внешний набор;
- добавить индекс на inner side;
- иногда лучше `Hash Join`.

### Hash Join
`Hash Join` — PostgreSQL строит hash по одной стороне и потом быстро проверяет совпадения по другой. [cybertec-postgresql](https://www.cybertec-postgresql.com/en/join-strategies-and-performance-in-postgresql/)
Хорош при больших наборах и равенствах.  
Проблема — память; если hash не помещается, начинается spill на диск.

Оптимизация:
- `work_mem`;
- индексы не всегда спасают;
- хороший выбор join order.

### Merge Join
`Merge Join` — обе стороны должны быть отсортированы по ключу, после чего PostgreSQL “сшивает” их в одном проходе. [postgrespro](https://postgrespro.com/blog/pgsql/5969618)
Хорош, когда данные уже идут в нужном порядке или сортировка дешева.  
Плохо, если сортировка дорогая.

### Nested Loop Anti Join
`Nested Loop Anti Join` — проверка “нет ли совпадения”. [postgrespro](https://postgrespro.ru/list/thread-id/2612148)
Часто используется в `NOT EXISTS`.  
Для фасетных фильтров и поиска по нескольким условиям это может быть узким местом, если внешний набор большой.

Оптимизация:
- переписать predicate;
- уменьшить внешний набор;
- проверить индекс по inner side;
- иногда заменить на другой паттерн.

### GroupAggregate
`GroupAggregate` — агрегация по группам, обычно при уже отсортированном входе. [sql-ex](https://sql-ex.ru/blogs/?%2FCupersposobnosti_EXPLAIN_v_PostgreSQL.html)
Хорош, если вход упорядочен по ключам группировки.  
Плохо, если перед ним надо долго сортировать большой поток данных.

### Incremental Sort
`Incremental Sort` — сортировка частями, когда вход уже частично отсортирован по части ключей. [pganalyze](https://pganalyze.com/blog/5mins-postgres-16-faster-query-plans)
Это часто хороший знак: PostgreSQL использует уже имеющийся порядок и сортирует только остаток.  
Но если групп много, он всё равно может стать дорогим.

### Sort
`Sort` — обычная сортировка. [postgrespro](https://postgrespro.ru/docs/postgrespro/current/using-explain)
Нормально на небольших наборах.  
Плохо на широких и больших данных, особенно если сортировка уходит на диск.

Оптимизация:
- индекс под `ORDER BY`;
- уменьшить ширину строк;
- ограничить результат;
- увеличить `work_mem`.

### Gather / Gather Merge
`Gather` и `Gather Merge` — узлы параллельного выполнения, которые собирают данные от worker’ов. [pganalyze](https://pganalyze.com/blog/5mins-postgres-16-faster-query-plans)
`Gather Merge` дополнительно сохраняет порядок.  
Это может сильно помочь на больших объёмах, но не всегда: параллелизм тоже стоит денег, и для маленьких запросов он лишний.

## Почему `cost` не равен миллисекундам
`cost` — это внутренняя модель стоимости, а не физическое время.  
В ней учитываются условные веса операций: I/O, CPU, случайные/последовательные чтения и т. п.. [postgrespro](https://postgrespro.ru/docs/postgresql/11/sql-explain)
Поэтому один план может иметь меньший `cost`, но реальное выполнение окажется медленнее из-за плохой статистики или неправильного распределения данных.

## Почему `rows` часто не совпадают с реальностью
Планировщик опирается на статистику, а статистика может:
- устареть;
- быть грубой;
- плохо описывать коррелированные условия;
- не понимать сложную логику JSONB/faceted filters. [habr](https://habr.com/ru/companies/ppr/articles/978100/)
В search/catalog запросах это особенно заметно: там много условий, много join’ов и часто неидеально предсказуемые распределения. [crafthomelab](https://crafthomelab.ru/posts/2025/09/06-postgresql-explain-analyze/)

## Что такое `Recheck Cond`
`Recheck Cond` означает, что после bitmap-поиска PostgreSQL ещё раз проверяет условие на heap-строке. [pganalyze](https://pganalyze.com/docs/explain/scan-nodes/bitmap-heap-scan)
Это бывает, когда bitmap частично неточный или lossy.  
Для фасетных запросов это нормально и не всегда проблема, но если recheck слишком дорогой — это сигнал к оптимизации. [pganalyze](https://pganalyze.com/docs/explain/scan-nodes/bitmap-heap-scan)

## Почему `Bitmap Heap Scan` часто в фасетах
Фасетные запросы обычно:
- фильтруют по нескольким признакам;
- возвращают не единичные строки, а набор;
- потом агрегируют counts. [docs.opensearch](https://docs.opensearch.org/latest/tutorials/faceted-search/)
В такой ситуации PostgreSQL часто выбирает bitmap-подход как компромисс между точечным `Index Scan` и полным `Seq Scan`. [stackoverflow](https://stackoverflow.com/questions/410586/what-is-the-difference-between-seq-scan-and-bitmap-heap-scan-in-postgres)

## Как применять на практике
Для backend-разработчика главный вопрос не “что означает узел”, а “что мне менять”.  
Если видишь:
- `Seq Scan` на большой таблице — ищи индекс или более селективный фильтр.
- `Nested Loop` на большом внешнем наборе — уменьши внешний набор или смени join strategy.
- `Bitmap Heap Scan` — это не всегда плохо; смотри на `Recheck Cond`, `loops`, количество строк и время.
- `Sort`/`GroupAggregate` на больших данных — проверяй индексы и `work_mem`.
- `rows` сильно расходится с реальностью — обновляй статистику и проверяй корреляции. [postgrespro](https://postgrespro.ru/docs/postgresql/current/using-explain)

Для marketplace/search-фич это особенно важно: фасеты, каталоги, фильтры по атрибутам и сортировки — именно те места, где план запросов решает, будет ли ответ за 100 ms или за 10 s. [potapov](https://potapov.me/ru/make/postgresql-patterns-guide)

## Cheat sheet

| Узел/поле | Что означает | Когда хорошо | Когда подозрительно |
|---|---|---|---|
| `cost` | Оценка планировщика | Для сравнения планов | Если думаешь, что это время |
| `rows` | Ожидаемое число строк | Совпадает с фактом | Сильный разрыв с реальностью |
| `width` | Оценка размера строки | Для понимания сортировок/join’ов | Если строки очень широкие |
| `actual time` | Фактическое время | Когда узел реально быстрый | Если миллисекунды превращаются в секунды |
| `loops` | Число запусков узла | 1 или немного | Сотни/тысячи на тяжёлом узле |
| `Seq Scan` | Полный проход таблицы | Маленькая таблица или много строк | На большой таблице при малом selectivity |
| `Index Scan` | Чтение через индекс | Небольшой точечный поиск | Очень много строк по индексу |
| `Index Only Scan` | Только индекс, без heap | Покрывающий индекс, мало heap fetches | Много heap fetches |
| `Bitmap Heap Scan` | Пачечное чтение строк по bitmap | Средний объём строк, фасеты | Большой `Recheck Cond`, lossy bitmap |
| `Nested Loop` | Вложенный цикл join | Маленькая outer side | Большой outer side |
| `Hash Join` | Join через hash table | Большие наборы, equality join | Spill на диск |
| `Merge Join` | Join через сортировку/порядок | Уже отсортированные данные | Дорогая сортировка |
| `GroupAggregate` | Группировка | Уже отсортированный вход | Большая предварительная сортировка |
| `Incremental Sort` | Частичная сортировка | Вход уже частично упорядочен | Много групп и большой объём |
| `Gather / Gather Merge` | Параллельное выполнение | Большие запросы | Маленькие запросы, где параллелизм лишний |

