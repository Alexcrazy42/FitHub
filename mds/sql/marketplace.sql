Table productCategories {
  id uuid [pk]
  name string
  parentId uuid [null]
  isActive bool
}

Table attributeDefinitions {
  id uuid [primary key]
  code string
  name string
}

Table attributeOptions {
  id uuid [primary key]
  attributeDefinitionId uuid [not null]
  value string
}

Table products {
  id uuid [primary key]
  name varchar
  isActive bool
  productCategoryId uuid
}

Table productVariants {
  id uuid [primary key]
  productId uuid [not null]
  name string
  price decimal
  currency string
  isActive bool
}

Table productVariantInventory {
  id uuid [primary key]
  productVariantId uuid [not null]
  countOnHand integer
  countReserved integer
}

Table productVariantAttribute {
  id uuid [primary key]
  productVariantId uuid [not null]
  definitionId uuid [not null]
  optionId uuid [not null]
}


Ref: attributeDefinitions.id < attributeOptions.attributeDefinitionId

Ref: productCategories.id < products.productCategoryId

Ref: products.id < productVariants.productId

Ref: productVariants.id < productVariantInventory.productVariantId

Ref: productVariants.id > productVariantAttribute.productVariantId

Ref: attributeDefinitions.id > productVariantAttribute.definitionId

Ref: attributeOptions.id > productVariantAttribute.optionId

//Ref user_posts: posts.user_id > users.id // many-to-one

//Ref: users.id < follows.following_user_id

//Ref: users.id < follows.followed_user_id



SELECT count(*)::int
FROM products AS p
WHERE p.is_active AND p.category_id = @__command_CategoryId_0 AND EXISTS (
    SELECT 1
    FROM product_variants AS p0
    WHERE p.id = p0.product_id AND p0.is_active AND p0.price_amount <= @__command_MaxPrice_Value_1)


SELECT p.category_id AS "CategoryId", count(*)::int AS "Count"
FROM products AS p
WHERE p.is_active AND p.category_id = @__command_CategoryId_0 AND EXISTS (
    SELECT 1
    FROM product_variants AS p0
    WHERE p.id = p0.product_id AND p0.is_active AND p0.price_amount <= @__command_MaxPrice_Value_1)
GROUP BY p.category_id


SELECT p.category_id AS "CategoryId", count(*)::int AS "Count"
FROM products AS p
WHERE p.is_active AND p.category_id = @__command_CategoryId_0 AND EXISTS (
    SELECT 1
    FROM product_variants AS p0
    WHERE p.id = p0.product_id AND p0.is_active AND p0.price_amount <= @__command_MaxPrice_Value_1)
GROUP BY p.category_id


SELECT p.id, p.is_active, p.name, p.parent_id, p.slug
FROM product_categories AS p
WHERE p.is_active
ORDER BY p.name

SELECT p.id, p.is_active, p.name, p.parent_id, p.slug
FROM product_categories AS p
WHERE p.is_active
ORDER BY p.name


SELECT p.attribute_definition_id AS "AttributeDefinitionId", p.attribute_option_id AS "AttributeOptionId", (
    SELECT count(*)::int
    FROM (
        SELECT DISTINCT p7.product_id
        FROM product_variant_attributes AS p3
        INNER JOIN product_variants AS p4 ON p3.product_variant_id = p4.id
        INNER JOIN product_variants AS p7 ON p3.product_variant_id = p7.id
        WHERE p4.is_active AND p4.product_id IN (
            SELECT p5.id
            FROM products AS p5
            WHERE p5.is_active AND p5.category_id = @__command_CategoryId_0 AND EXISTS (
                SELECT 1
                FROM product_variants AS p6
                WHERE p5.id = p6.product_id AND p6.is_active AND p6.price_amount <= @__command_MaxPrice_Value_1)
        ) AND p.attribute_definition_id = p3.attribute_definition_id AND p.attribute_option_id = p3.attribute_option_id
    ) AS s) AS "Count"
FROM product_variant_attributes AS p
INNER JOIN product_variants AS p0 ON p.product_variant_id = p0.id
WHERE p0.is_active AND p0.product_id IN (
    SELECT p1.id
    FROM products AS p1
    WHERE p1.is_active AND p1.category_id = @__command_CategoryId_0 AND EXISTS (
        SELECT 1
        FROM product_variants AS p2
        WHERE p1.id = p2.product_id AND p2.is_active AND p2.price_amount <= @__command_MaxPrice_Value_1)
)
GROUP BY p.attribute_definition_id, p.attribute_option_id


SELECT a.id, a.code, a.is_filterable, a.is_purchase_option, a.name, a.sort_order, a0.id, a0.attribute_definition_id, a0.sort_order, a0.value
FROM attribute_definitions AS a
LEFT JOIN attribute_options AS a0 ON a.id = a0.attribute_definition_id
WHERE a.is_filterable
ORDER BY a.sort_order, a.name, a.id


SELECT p1.id, p1.brand_id, p1.category_id, p1.created_at, p1.description, p1.is_active, p1.name, p1.slug, p1.updated_at, p1.version, m.id, m.name, m.slug, p2.id, p2.alt_text, p2.file_id, p2.is_main, p2.product_id, p2.sort_order, s.id, s.compare_at_price_amount, s.currency, s.is_active, s.name, s.price_amount, s.product_id, s.sku, s.version, s.id0, s.product_variant_id, s.quantity_on_hand, s.quantity_reserved, s.version0
FROM (
    SELECT p.id, p.brand_id, p.category_id, p.created_at, p.description, p.is_active, p.name, p.slug, p.updated_at, p.version
    FROM products AS p
    WHERE p.is_active
      AND p.category_id = '019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01'
      AND EXISTS (
          SELECT 1
          FROM product_variants AS p0
          WHERE p.id = p0.product_id
            AND p0.is_active
            AND p0.price_amount <= 3000
      )
    ORDER BY p.created_at DESC, p.name
    LIMIT 12 OFFSET 0
) AS p1
LEFT JOIN marketplace_brands AS m ON p1.brand_id = m.id
LEFT JOIN product_images AS p2 ON p1.id = p2.product_id
LEFT JOIN (
    SELECT p3.id, p3.compare_at_price_amount, p3.currency, p3.is_active, p3.name, p3.price_amount, p3.product_id, p3.sku, p3.version, p4.id AS id0, p4.product_variant_id, p4.quantity_on_hand, p4.quantity_reserved, p4.version AS version0
    FROM product_variants AS p3
    LEFT JOIN product_variant_inventories AS p4 ON p3.id = p4.product_variant_id
) AS s ON p1.id = s.product_id
ORDER BY p1.created_at DESC, p1.name, p1.id, m.id, p2.id, s.id;