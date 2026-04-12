

  insert into product_categories (id, name, slug, parent_id, is_active)
  values
    ('119ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01', 'Equipment', 'equipment', null, true),
    ('119ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02', 'Apparel', 'apparel', null, true),
    ('119ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03', 'Nutrition', 'nutrition', null, true)
  on conflict (slug) do update
  set name = excluded.name,
      is_active = excluded.is_active;

  insert into marketplace_brands (id, name, slug)
  values
    ('219ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01', 'Nike', 'nike'),
    ('219ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02', 'Adidas', 'adidas'),
    ('219ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03', 'Rogue', 'rogue'),
    ('219ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04', 'Garmin', 'garmin'),
    ('219ad0f7-1d2b-78c0-bf3b-3f7c88bd5b05', 'MyProtein', 'myprotein')
  on conflict (slug) do update
  set name = excluded.name;

  insert into attribute_definitions (id, code, name, is_purchase_option, is_filterable, sort_order)
  values
    ('319ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01', 'color', 'Color', true, true, 10),
    ('319ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02', 'material', 'Material', false, true, 20),
    ('319ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03', 'goal', 'Goal', false, true, 30),
    ('319ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04', 'weight', 'Weight', true, true, 40)
  on conflict (code) do update
  set name = excluded.name,
      is_purchase_option = excluded.is_purchase_option,
      is_filterable = excluded.is_filterable,
      sort_order = excluded.sort_order;

  insert into attribute_options (id, attribute_definition_id, value, sort_order)
  values
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01', (select id from attribute_definitions where code = 'size'), 'S', 1),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02', (select id from attribute_definitions where code = 'size'), 'L', 3),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03', (select id from attribute_definitions where code = 'color'), 'Black', 1),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04', (select id from attribute_definitions where code = 'color'), 'Blue', 2),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b05', (select id from attribute_definitions where code = 'color'), 'Red', 3),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b06', (select id from attribute_definitions where code = 'material'), 'Rubber',
  1),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b07', (select id from attribute_definitions where code = 'material'), 'Steel',
  2),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b08', (select id from attribute_definitions where code = 'material'), 'Cotton',
  3),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b09', (select id from attribute_definitions where code = 'goal'), 'Strength', 1),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b10', (select id from attribute_definitions where code = 'goal'), 'Cardio', 2),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11', (select id from attribute_definitions where code = 'goal'), 'Recovery', 3),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b12', (select id from attribute_definitions where code = 'weight'), '5 kg', 1),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b13', (select id from attribute_definitions where code = 'weight'), '10 kg', 2),
    ('419ad0f7-1d2b-78c0-bf3b-3f7c88bd5b14', (select id from attribute_definitions where code = 'weight'), '20 kg', 3)
  on conflict (attribute_definition_id, value) do update
  set sort_order = excluded.sort_order;

  with product_seed(id, category_slug, brand_slug, name, slug, description, created_at) as (
    values
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01'::uuid, 'equipment', 'rogue', 'Rogue Steel Kettlebell 10 kg', 'rogue-steel-
  kettlebell-10kg', 'Steel kettlebell for strength training.', now() - interval '12 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02'::uuid, 'equipment', 'rogue', 'Rogue Steel Kettlebell 20 kg', 'rogue-steel-
  kettlebell-20kg', 'Heavy steel kettlebell for squats and swings.', now() - interval '11 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03'::uuid, 'equipment', 'fithub', 'FitHub Resistance Band Blue', 'fithub-
  resistance-band-blue', 'Rubber resistance band for mobility and recovery.', now() - interval '10 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04'::uuid, 'equipment', 'fithub', 'FitHub Jump Rope Red', 'fithub-jump-rope-
  red', 'Fast jump rope for cardio intervals.', now() - interval '9 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b05'::uuid, 'apparel', 'nike', 'Nike Training Tee Black S', 'nike-training-tee-
  black-s', 'Cotton tee for gym training.', now() - interval '8 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b06'::uuid, 'apparel', 'nike', 'Nike Training Tee Blue L', 'nike-training-tee-
  blue-l', 'Blue cotton tee for everyday training.', now() - interval '7 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b07'::uuid, 'apparel', 'adidas', 'Adidas Recovery Hoodie Black L', 'adidas-
  recovery-hoodie-black-l', 'Soft hoodie for recovery days.', now() - interval '6 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b08'::uuid, 'equipment', 'garmin', 'Garmin Cardio Sensor Black', 'garmin-
  cardio-sensor-black', 'Sensor for cardio sessions and zone tracking.', now() - interval '5 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b09'::uuid, 'nutrition', 'myprotein', 'MyProtein Recovery Shake Chocolate',
  'myprotein-recovery-shake-chocolate', 'Recovery shake after strength sessions.', now() - interval '4 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b10'::uuid, 'nutrition', 'myprotein', 'MyProtein Cardio Electrolytes',
  'myprotein-cardio-electrolytes', 'Electrolytes for long cardio workouts.', now() - interval '3 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11'::uuid, 'equipment', 'fithub', 'FitHub Yoga Block Blue', 'fithub-yoga-
  block-blue', 'Blue block for mobility and recovery.', now() - interval '2 days'),
      ('519ad0f7-1d2b-78c0-bf3b-3f7c88bd5b12'::uuid, 'equipment', 'rogue', 'Rogue Rubber Plate 5 kg', 'rogue-rubber-
  plate-5kg', 'Rubber plate for strength training.', now() - interval '1 day')
  )
  insert into products (id, category_id, brand_id, name, slug, description, is_active, created_at, updated_at, version)
  select
    p.id,
    c.id,
    b.id,
    p.name,
    p.slug,
    p.description,
    true,
    p.created_at,
    p.created_at,
    0
  from product_seed p
  join product_categories c on c.slug = p.category_slug
  join marketplace_brands b on b.slug = p.brand_slug
  on conflict (slug) do update
  set name = excluded.name,
      description = excluded.description,
      category_id = excluded.category_id,
      brand_id = excluded.brand_id,
      is_active = true,
      updated_at = now();

  with variant_seed(id, product_slug, sku, variant_name, price, compare_at_price, qty) as (
    values
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01'::uuid, 'rogue-steel-kettlebell-10kg', 'ROGUE-KB-10-BLK', '10 kg / Black',
  5900::numeric, 6900::numeric, 8),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02'::uuid, 'rogue-steel-kettlebell-20kg', 'ROGUE-KB-20-BLK', '20 kg / Black',
  9900::numeric, null::numeric, 0),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03'::uuid, 'fithub-resistance-band-blue', 'FITHUB-BAND-BLU', 'Blue',
  1290::numeric, null::numeric, 24),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04'::uuid, 'fithub-jump-rope-red', 'FITHUB-ROPE-RED', 'Red', 1490::numeric,
  1990::numeric, 15),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b05'::uuid, 'nike-training-tee-black-s', 'NIKE-TEE-BLK-S', 'S / Black',
  2990::numeric, null::numeric, 11),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b06'::uuid, 'nike-training-tee-blue-l', 'NIKE-TEE-BLU-L', 'L / Blue',
  3190::numeric, 3790::numeric, 4),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b07'::uuid, 'adidas-recovery-hoodie-black-l', 'ADIDAS-HOOD-BLK-L', 'L / Black',
  6990::numeric, 7990::numeric, 2),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b08'::uuid, 'garmin-cardio-sensor-black', 'GARMIN-SENSOR-BLK', 'Black',
  12990::numeric, null::numeric, 6),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b09'::uuid, 'myprotein-recovery-shake-chocolate', 'MYPROT-SHAKE-REC',
  'Recovery', 2190::numeric, null::numeric, 18),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b10'::uuid, 'myprotein-cardio-electrolytes', 'MYPROT-ELECTRO-CARDIO', 'Cardio',
  1590::numeric, null::numeric, 30),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11'::uuid, 'fithub-yoga-block-blue', 'FITHUB-BLOCK-BLU', 'Blue', 890::numeric,
  null::numeric, 20),
      ('619ad0f7-1d2b-78c0-bf3b-3f7c88bd5b12'::uuid, 'rogue-rubber-plate-5kg', 'ROGUE-PLATE-5-RUB', '5 kg',
  3490::numeric, 3990::numeric, 13)
  )
  insert into product_variants (id, product_id, sku, name, price_amount, currency, compare_at_price_amount, is_active,
  version)
  select
    v.id,
    p.id,
    v.sku,
    v.variant_name,
    v.price,
    'RUB',
    v.compare_at_price,
    true,
    0
  from variant_seed v
  join products p on p.slug = v.product_slug
  on conflict (product_id, sku) do update
  set name = excluded.name,
      price_amount = excluded.price_amount,
      compare_at_price_amount = excluded.compare_at_price_amount,
      is_active = true;

  with inventory_seed(id, sku, qty) as (
    values
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01'::uuid, 'ROGUE-KB-10-BLK', 8),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02'::uuid, 'ROGUE-KB-20-BLK', 0),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03'::uuid, 'FITHUB-BAND-BLU', 24),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04'::uuid, 'FITHUB-ROPE-RED', 15),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b05'::uuid, 'NIKE-TEE-BLK-S', 11),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b06'::uuid, 'NIKE-TEE-BLU-L', 4),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b07'::uuid, 'ADIDAS-HOOD-BLK-L', 2),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b08'::uuid, 'GARMIN-SENSOR-BLK', 6),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b09'::uuid, 'MYPROT-SHAKE-REC', 18),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b10'::uuid, 'MYPROT-ELECTRO-CARDIO', 30),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b11'::uuid, 'FITHUB-BLOCK-BLU', 20),
      ('719ad0f7-1d2b-78c0-bf3b-3f7c88bd5b12'::uuid, 'ROGUE-PLATE-5-RUB', 13)
  )
  insert into product_variant_inventories (id, product_variant_id, quantity_on_hand, quantity_reserved, version)
  select
    i.id,
    v.id,
    i.qty,
    0,
    0
  from inventory_seed i
  join product_variants v on v.sku = i.sku
  on conflict (product_variant_id) do update
  set quantity_on_hand = excluded.quantity_on_hand,
      quantity_reserved = 0,
      version = product_variant_inventories.version + 1;

  with attr_seed(sku, code, option_value) as (
    values
      ('ROGUE-KB-10-BLK', 'color', 'Black'), ('ROGUE-KB-10-BLK', 'material', 'Steel'), ('ROGUE-KB-10-BLK', 'goal',
  'Strength'), ('ROGUE-KB-10-BLK', 'weight', '10 kg'),
      ('ROGUE-KB-20-BLK', 'color', 'Black'), ('ROGUE-KB-20-BLK', 'material', 'Steel'), ('ROGUE-KB-20-BLK', 'goal',
  'Strength'), ('ROGUE-KB-20-BLK', 'weight', '20 kg'),
      ('FITHUB-BAND-BLU', 'color', 'Blue'), ('FITHUB-BAND-BLU', 'material', 'Rubber'), ('FITHUB-BAND-BLU', 'goal',
  'Recovery'),
      ('FITHUB-ROPE-RED', 'color', 'Red'), ('FITHUB-ROPE-RED', 'material', 'Rubber'), ('FITHUB-ROPE-RED', 'goal',
  'Cardio'),
      ('NIKE-TEE-BLK-S', 'size', 'S'), ('NIKE-TEE-BLK-S', 'color', 'Black'), ('NIKE-TEE-BLK-S', 'material', 'Cotton'),
  ('NIKE-TEE-BLK-S', 'goal', 'Strength'),
      ('NIKE-TEE-BLU-L', 'size', 'L'), ('NIKE-TEE-BLU-L', 'color', 'Blue'), ('NIKE-TEE-BLU-L', 'material', 'Cotton'),
  ('NIKE-TEE-BLU-L', 'goal', 'Cardio'),
      ('ADIDAS-HOOD-BLK-L', 'size', 'L'), ('ADIDAS-HOOD-BLK-L', 'color', 'Black'), ('ADIDAS-HOOD-BLK-L', 'material',
  'Cotton'), ('ADIDAS-HOOD-BLK-L', 'goal', 'Recovery'),
      ('GARMIN-SENSOR-BLK', 'color', 'Black'), ('GARMIN-SENSOR-BLK', 'goal', 'Cardio'),
      ('MYPROT-SHAKE-REC', 'goal', 'Recovery'),
      ('MYPROT-ELECTRO-CARDIO', 'goal', 'Cardio'),
      ('FITHUB-BLOCK-BLU', 'color', 'Blue'), ('FITHUB-BLOCK-BLU', 'material', 'Rubber'), ('FITHUB-BLOCK-BLU', 'goal',
  'Recovery'),
      ('ROGUE-PLATE-5-RUB', 'material', 'Rubber'), ('ROGUE-PLATE-5-RUB', 'goal', 'Strength'), ('ROGUE-PLATE-5-RUB',
  'weight', '5 kg')
  )
  insert into product_variant_attributes (id, product_variant_id, attribute_definition_id, attribute_option_id)
  select
    md5(a.sku || ':' || a.code || ':' || a.option_value)::uuid,
    v.id,
    ad.id,
    ao.id
  from attr_seed a
  join product_variants v on v.sku = a.sku
  join attribute_definitions ad on ad.code = a.code
  join attribute_options ao on ao.attribute_definition_id = ad.id and ao.value = a.option_value
  on conflict (product_variant_id, attribute_definition_id) do update
  set attribute_option_id = excluded.attribute_option_id;