# Test

## Материалы

1. https://habr.com/ru/companies/ppr/articles/978100/ - sql planner

2. https://habr.com/ru/companies/ozontech/articles/667600/ как делают поиск озон

3. https://habr.com/ru/companies/oleg-bunin/articles/767066/ как сделать свой интернет магазин

## Таблицы

product_category (id, name)

product (id, price, payload)

attribute_definition (id, productCategoryId, code, name, unit, filterable, facetable)

attribute_options (id, attributeDefinitionId, optionText)

product_attribute_index (productId, attributeDefinitionId, valueText?, valueNum?, valueBool?, attributeOptionId?)

## Важные замечания

1. attribute_definition.unit - num, text, bool, select, radioButton (если select или radioButton, то нужно смотреть в attribute_options)

2. между одним attribute_definition - ИЛИ, между разными attribute_definition - И

## Запросы

```sql
-- DDL
create table product_category (
    id bigserial primary key,
    name text not null
);

create table product (
    id bigserial primary key,
    productCategoryId bigint not null references product_category(id),
    price numeric(12,2) not null,
    payload jsonb not null default '{}'::jsonb
);

create table attribute_definition (
    id bigserial primary key,
    productCategoryId bigint not null references product_category(id),
    code text not null,
    name text not null,
    unit text not null, -- num, text, bool, select, radioButton
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
    valueText text null,
    valueNum numeric null,
    valueBool boolean null,
    attributeOptionId bigint null references attribute_options(id),
    primary key (productId, attributeDefinitionId)
);

-- indexes
create index ix_product_category_price on product(productCategoryId, price);
create index ix_product_payload_gin on product using gin (payload);

create index ix_attr_def_category_filterable on attribute_definition(productCategoryId, filterable, facetable);
create index ix_attr_opt_def on attribute_options(attributeDefinitionId);

create index ix_pai_attr_opt on product_attribute_index(attributeDefinitionId, attributeOptionId, productId);
create index ix_pai_attr_text on product_attribute_index(attributeDefinitionId, valueText, productId);
create index ix_pai_attr_num on product_attribute_index(attributeDefinitionId, valueNum, productId);
create index ix_pai_product on product_attribute_index(productId);

-- seed data
insert into product_category(name) values
('Clothing'),
('Shoes');

insert into attribute_definition(productCategoryId, code, name, unit, filterable, facetable) values
(1, 'color', 'Color', 'select', true, true),
(1, 'size', 'Size', 'select', true, true),
(1, 'sleeve_length', 'Sleeve length', 'select', true, true),
(1, 'material', 'Material', 'select', true, true),
(2, 'color', 'Color', 'select', true, true),
(2, 'size', 'Size', 'num', true, true),
(2, 'season', 'Season', 'select', true, true),
(2, 'heel_height', 'Heel height', 'num', true, true);

insert into attribute_options(attributeDefinitionId, optionText) values
(1, 'black'), (1, 'white'), (1, 'blue'),
(2, 'S'), (2, 'M'), (2, 'L'),
(3, 'short'), (3, 'long'),
(4, 'cotton'), (4, 'polyester'),
(5, 'black'), (5, 'white'), (5, 'brown'),
(7, 'summer'), (7, 'winter'),
(7, 'all-season');

insert into product(productCategoryId, price, payload) values
(1, 19.99, '{"title":"T-shirt basic"}'),
(1, 29.99, '{"title":"Hoodie"}'),
(1, 24.50, '{"title":"Shirt slim"}'),
(1, 49.90, '{"title":"Jacket"}'),
(1, 15.00, '{"title":"T-shirt premium"}'),
(2, 79.99, '{"title":"Sneakers"}'),
(2, 129.99, '{"title":"Boots"}'),
(2, 59.99, '{"title":"Loafers"}'),
(2, 89.50, '{"title":"Running shoes"}'),
(2, 149.00, '{"title":"Heels"}');

insert into product_attribute_index(productId, attributeDefinitionId, attributeOptionId, valueText, valueNum, valueBool) values
-- clothing products
(1, 1, 1, 'black', null, null), (1, 2, 4, 'S', null, null), (1, 3, 7, 'short', null, null), (1, 4, 9, 'cotton', null, null),
(2, 1, 3, 'blue', null, null), (2, 2, 5, 'M', null, null), (2, 3, 8, 'long', null, null), (2, 4, 10, 'polyester', null, null),
(3, 1, 2, 'white', null, null), (3, 2, 5, 'M', null, null), (3, 3, 8, 'long', null, null), (3, 4, 9, 'cotton', null, null),
(4, 1, 1, 'black', null, null), (4, 2, 6, 'L', null, null), (4, 3, 8, 'long', null, null), (4, 4, 10, 'polyester', null, null),
(5, 1, 2, 'white', null, null), (5, 2, 4, 'S', null, null), (5, 3, 7, 'short', null, null), (5, 4, 9, 'cotton', null, null),
-- shoes products
(6, 5, 11, 'black', null, null), (6, 6, null, null, 42, null), (6, 7, 14, 'summer', null, null), (6, 8, null, null, 2.5, null),
(7, 5, 13, 'brown', null, null), (7, 6, null, null, 44, null), (7, 7, 15, 'winter', null, null), (7, 8, null, null, 4.0, null),
(8, 5, 12, 'white', null, null), (8, 6, null, null, 41, null), (8, 7, 16, 'all-season', null, null), (8, 8, null, null, 1.5, null),
(9, 5, 11, 'black', null, null), (9, 6, null, null, 43, null), (9, 7, 14, 'summer', null, null), (9, 8, null, null, 3.0, null),
(10, 5, 12, 'white', null, null), (10, 6, null, null, 39, null), (10, 7, 15, 'winter', null, null), (10, 8, null, null, 7.5, null);

-- sample queries

-- Q1: matching products for filters
--explain analyze
with selected_filters as (
    select 3::bigint as attributeDefinitionId, 32::bigint as attributeOptionId
    union all
    select 203::bigint, 632::bigint
),
matched_products as (
    select pai.productId
    from  selected_filters sf
    join product_attribute_index pai
      on sf.attributeDefinitionId = pai.attributeDefinitionId
 		and sf.attributeOptionId = pai.attributeOptionId
    group by pai.productId
    having count(*) = (select count(*) from selected_filters)
)
select *
from product p
join matched_products mp on mp.productId = p.id
where p.productCategoryId = 3
  and p.price between 0 and 10000
order by p.price asc;


-- Q2: facets for clothing filters color=black (and no show not available facets)
with selected_filters as (
    select 3::bigint as attributeDefinitionId, 32::bigint as attributeOptionId
    union all
    select 203::bigint, 632::bigint
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
    select pai.productId
    from selected_filters sf
    join product_attribute_index pai
      on sf.attributeDefinitionId = pai.attributeDefinitionId
     and sf.attributeOptionId = pai.attributeOptionId
    group by pai.productId
    having count(*) = (select count(*) from selected_filters)
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


-- Q4: Q3 but show all facets even with 0 count (for 0 count - show 0 and not join to matching_products)
with selected_filters as (
    select 3::bigint as attributeDefinitionId, 32::bigint as attributeOptionId
    union all
    select 203::bigint, 632::bigint
),
matching_products as (
    select p.id
    from product p
    where p.productCategoryId = 3
      and p.price between 0 and 10000
      and not exists (
          select 1
          from selected_filters sf
          where not exists (
              select 1
              from product_attribute_index pai
              where pai.productId = p.id
                and pai.attributeDefinitionId = sf.attributeDefinitionId
                and pai.attributeOptionId = sf.attributeOptionId
          )
      )
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
    count(distinct mp.id) as cnt
from all_facet_values af
left join product_attribute_index pai
  on pai.attributeDefinitionId = af.attributeDefinitionId
 and pai.attributeOptionId = af.attributeOptionId
left join matching_products mp
  on mp.id = pai.productId
group by
    af.attributeDefinitionId,
    af.attributeName,
    af.attributeOptionId,
    af.optionText
order by
    af.attributeDefinitionId,
    af.attributeOptionId;


explain analyze
with selected_filters as (
    select 1::bigint as attributeDefinitionId, 1::bigint as attributeOptionId
    union all
    select 2::bigint, 6::bigint
),
matching_products as (
    select p.id
    from product p
    where p.productCategoryId = 1
      and p.price between 0 and 1000
      and not exists (
          select 1
          from selected_filters sf
          where not exists (
              select 1
              from product_attribute_index pai
              where pai.productId = p.id
                and pai.attributeDefinitionId = sf.attributeDefinitionId
                and pai.attributeOptionId = sf.attributeOptionId
          )
      )
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
    where ad.productCategoryId = 1
      and ad.filterable = true
      and ad.facetable = true
)
select
    af.attributeDefinitionId,
    af.attributeName,
    af.attributeOptionId,
    af.optionText,
    count(distinct mp.id) as cnt
from all_facet_values af
left join product_attribute_index pai
  on pai.attributeDefinitionId = af.attributeDefinitionId
 and pai.attributeOptionId = af.attributeOptionId
left join matching_products mp
  on mp.id = pai.productId
group by
    af.attributeDefinitionId,
    af.attributeName,
    af.attributeOptionId,
    af.optionText
order by
    af.attributeDefinitionId,
    af.attributeOptionId;



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
```


```
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