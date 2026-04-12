# Test

## Материалы

1. https://habr.com/ru/companies/ppr/articles/978100/ - sql planner
    - Еженедельный мониторинг pg_stat_statements спасает часы отладки
    - параметры: запуск gather, mem на хранение в памяти при hash join

2. https://habr.com/ru/companies/ozontech/articles/667600/ как делают поиск озон

3. https://habr.com/ru/companies/oleg-bunin/articles/767066/ как сделать свой интернет магазин
    архитектура перекрестка
    эластик и другие технологии

## Таблицы

product_category (id, name)

product (id, price, payload)

attribute_definition (id, productCategoryId, code, name, filterable, facetable)

attribute_options (id, attributeDefinitionId, optionText)

product_attribute_index (productId, attributeDefinitionId, attributeOptionId?)

## DDL

```sql
-- DDL
create table product_category (
    id bigserial primary key,
    name text not null
);

create table product (
    id bigserial primary key,
    productCategoryId bigint not null references product_category(id),
    price numeric(12,2) not null
    -- other fields
);

create table attribute_definition (
    id bigserial primary key,
    productCategoryId bigint not null references product_category(id),
    name text not null,
    filterable boolean not null default true,
    facetable boolean not null default true,
    unique (productCategoryId, code)
);

create table attribute_options (
    id bigserial primary key,
    attributeDefinitionId bigint not null references attribute_definition(id) on delete cascade,
    optionText text not null,
    unique (attributeDefinitionId, optionText)
);

create table product_attribute_index (
    productId bigint not null references product(id) on delete cascade,
    attributeDefinitionId bigint not null references attribute_definition(id) on delete cascade,
    attributeOptionId bigint not null references attribute_options(id),
    primary key (productId, attributeDefinitionId)
);

-- indexes
create index ix_product_category_price on product(productCategoryId, price);
create index ix_product_payload_gin on product using gin (payload);

create index ix_attr_def_category_filterable on attribute_definition(productCategoryId, filterable, facetable);
create index ix_attr_opt_def on attribute_options(attributeDefinitionId);

create index ix_pai_attr_opt on product_attribute_index(attributeDefinitionId, attributeOptionId, productId);
create index ix_pai_product on product_attribute_index(productId);

## Большое количество данных

```sql
-- Generate 3,000,000 products with categories, attributes, options, and index rows.
-- Assumes schema from marketplace_facet_demo.sql or equivalent tables already exist.

-- Recommended before running:
-- set maintenance_work_mem = '1GB';
-- set work_mem = '128MB';
-- set synchronous_commit = off;

truncate table product_attribute_index, attribute_options, attribute_definition, product, product_category restart identity cascade;

insert into product_category(name)
select 'Category ' || gs
from generate_series(1, 50) gs;

insert into attribute_definition(productCategoryId, code, name, unit, filterable, facetable)
select c.id, 'attr_' || a, 'Attr ' || a,
       case when a % 4 = 0 then 'num' when a % 4 = 1 then 'select' when a % 4 = 2 then 'text' else 'bool' end,
       true, true
from product_category c
cross join generate_series(1, 30) a;

insert into attribute_options(attributeDefinitionId, optionText)
select ad.id, 'opt_' || gs
from attribute_definition ad
join lateral generate_series(1, case when ad.unit in ('select','radioButton') then 12 else 0 end) gs on true;

with cat as (
    select id, row_number() over(order by id) as rn from product_category
), p as (
    select gs as product_id,
           ((gs - 1) % 50) + 1 as category_id,
           (random() * 9999 + 1)::numeric(12,2) as price,
           jsonb_build_object(
               'title', 'Product ' || gs,
               'brand', 'Brand ' || ((gs - 1) % 200 + 1),
               'rating', round((random() * 4 + 1)::numeric, 2)
           ) as payload
    from generate_series(1, 3000000) gs
)
insert into product(id, productCategoryId, price, payload)
select product_id, category_id, price, payload
from p;

-- product_attribute_index: average 8 attributes per product.
-- We use 8 deterministic attribute slots per category.
with product_base as (
    select p.id as product_id, p.productCategoryId as category_id
    from product p
), cat_attr as (
    select ad.id as attribute_definition_id, ad.productCategoryId as category_id, ad.unit,
           row_number() over(partition by ad.productCategoryId order by ad.id) as attr_rank
    from attribute_definition ad
), prod_slots as (
    select pb.product_id, pb.category_id, gs as slot
    from product_base pb
    cross join generate_series(1, 8) gs
), chosen_attr as (
    select ps.product_id, ps.category_id, ca.attribute_definition_id, ca.unit, ps.slot
    from prod_slots ps
    join cat_attr ca
      on ca.category_id = ps.category_id
     and ca.attr_rank = ps.slot
), prepared as (
    select
        product_id,
        attribute_definition_id,
        case when unit in ('select','radioButton') then null::text
             when unit = 'text' then 'v_' || (product_id % 1000)::text
             else null::text end as value_text,
        case when unit = 'num' then ((product_id % 2000) * 0.1)::numeric(12,2) else null::numeric end as value_num,
        case when unit = 'bool' then (product_id % 2 = 0) else null::boolean end as value_bool,
        unit
    from chosen_attr
)
insert into product_attribute_index(productId, attributeDefinitionId, valueText, valueNum, valueBool, attributeOptionId)
select
    p.product_id,
    p.attribute_definition_id,
    case when p.unit in ('select','radioButton') then ao.optionText else p.value_text end as valueText,
    p.value_num,
    p.value_bool,
    case when p.unit in ('select','radioButton') then ao.id else null end as attributeOptionId
from prepared p
left join lateral (
    select ao.id, ao.optionText
    from attribute_options ao
    where ao.attributeDefinitionId = p.attribute_definition_id
    order by ao.id
    offset (p.product_id % 12)
    limit 1
) ao on p.unit in ('select','radioButton');

analyze product_category;
analyze attribute_definition;
analyze attribute_options;
analyze product;
analyze product_attribute_index;


SELECT pg_size_pretty(pg_total_relation_size('public."product_attribute_index"'));

SELECT pg_size_pretty(pg_table_size('public."product_attribute_index"'));

SELECT 
    i.relname AS index_name,
    a.attname AS column_name,
    ix.indisunique AS is_unique,
    ix.indisprimary AS is_primary,
    t.relname
FROM pg_class t
JOIN pg_index ix ON t.oid = ix.indrelid
JOIN pg_class i ON i.oid = ix.indexrelid
JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = ANY(ix.indkey)
WHERE t.relname = 'product_attribute_index';
```


-- sample queries

-- Q1: matching products for filters
--explain analyze
explain analyze
WITH selected_filters AS (
    SELECT 3::bigint AS attributeDefinitionId, 32::bigint AS attributeOptionId
    UNION ALL
    SELECT 3::bigint, 33::bigint
    UNION ALL
    SELECT 203::bigint, 632::bigint
),
selected_groups AS (
    SELECT
        attributeDefinitionId,
        array_agg(attributeOptionId) AS option_ids,
        count(*) AS options_cnt
    FROM selected_filters
    GROUP BY attributeDefinitionId
),
matched_products AS (
    SELECT pai.productId
    FROM product_attribute_index pai
    JOIN selected_groups sg
      ON sg.attributeDefinitionId = pai.attributeDefinitionId
     AND pai.attributeOptionId = ANY(sg.option_ids)
    GROUP BY pai.productId
    HAVING count(DISTINCT pai.attributeDefinitionId) = (SELECT count(*) FROM selected_groups)
)
SELECT *
FROM product p
JOIN matched_products mp ON mp.productId = p.id
where 1 = 1
and p.productCategoryId = 3
  AND p.price BETWEEN 0 AND 10000
ORDER BY p.price ASC;


-- Q2: facets for clothing filters color=black (and no show not available facets)
explain analyze
WITH selected_filters AS (
    SELECT 3::bigint AS attributeDefinitionId, 32::bigint AS attributeOptionId
    UNION ALL
    SELECT 3::bigint, 33::bigint
    UNION ALL
    SELECT 203::bigint, 632::bigint
),
selected_groups AS (
    SELECT
        attributeDefinitionId,
        array_agg(attributeOptionId) AS option_ids,
        count(*) AS options_cnt
    FROM selected_filters
    GROUP BY attributeDefinitionId
),
allowed_attrs as (
    select id, name
    from attribute_definition
    where 1 = 1
    and productCategoryId = 3
      and filterable = true
      and facetable = true
),
matched_products as (
    SELECT pai.productId
    FROM product_attribute_index pai
    JOIN selected_groups sg
      ON sg.attributeDefinitionId = pai.attributeDefinitionId
     AND pai.attributeOptionId = ANY(sg.option_ids)
    GROUP BY pai.productId
    HAVING count(DISTINCT pai.attributeDefinitionId) = (SELECT count(*) FROM selected_groups)
)
select
    ad.id as attributeDefinitionId,
    ad.name as attributeName,
    ao.id as attributeOptionId,
    ao.optionText,
    count(distinct mp.productId) as cnt
from matched_products mp
join product_attribute_index pai
  on pai.productId = mp.productId
join allowed_attrs ad
  on ad.id = pai.attributeDefinitionId
join attribute_options ao
  on ao.id = pai.attributeOptionId
group by ad.id, ad.name, ao.id, ao.optionText
order by ad.id, cnt desc;


-- Q3: Q2 but show all facets even with 0 count (for 0 count - show 0 and not join to matching_products)
WITH selected_filters AS (
    SELECT 3::bigint AS attributeDefinitionId, 32::bigint AS attributeOptionId
    UNION ALL
    SELECT 3::bigint, 33::bigint
    UNION ALL
    SELECT 203::bigint, 632::bigint
),
selected_groups AS (
    SELECT
        attributeDefinitionId,
        array_agg(attributeOptionId) AS option_ids,
        count(*) AS options_cnt
    FROM selected_filters
    GROUP BY attributeDefinitionId
),
matched_products as (
    SELECT pai.productId
    FROM product_attribute_index pai
    JOIN selected_groups sg
      ON sg.attributeDefinitionId = pai.attributeDefinitionId
     AND pai.attributeOptionId = ANY(sg.option_ids)
    GROUP BY pai.productId
    HAVING count(DISTINCT pai.attributeDefinitionId) = (SELECT count(*) FROM selected_groups)
),
all_facet_values as (
    select
        ad.id as attributeDefinitionId,
        ad.name as attributeName,
        ao.id as attributeOptionId,
        ao.optionText
    from attribute_definition ad
    join attribute_options ao
      on ao.attributeDefinitionId = ad.id
    where ad.productCategoryId = 3
      and ad.filterable = true
      and ad.facetable = true
)
select
    af.attributeDefinitionId,
    af.attributeName,
    af.attributeOptionId,
    af.optionText,
    count(distinct mp.productId) as cnt
from all_facet_values af
left join product_attribute_index pai
  on pai.attributeDefinitionId = af.attributeDefinitionId
 and pai.attributeOptionId = af.attributeOptionId
left join matched_products mp
  on mp.productId = pai.productId
group by
    af.attributeDefinitionId,
    af.attributeName,
    af.attributeOptionId,
    af.optionText
order by
    af.attributeDefinitionId,
    af.attributeOptionId;
