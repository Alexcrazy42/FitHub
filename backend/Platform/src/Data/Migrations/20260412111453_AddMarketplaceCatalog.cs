using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attribute_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_purchase_option = table.Column<bool>(type: "boolean", nullable: false),
                    is_filterable = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attribute_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "marketplace_brands",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_marketplace_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "product_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_categories_product_categories_parent_id",
                        column: x => x.parent_id,
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "attribute_options",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    attribute_definition_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attribute_options", x => x.id);
                    table.ForeignKey(
                        name: "fk_attribute_options_attribute_definitions_attribute_definitio",
                        column: x => x.attribute_definition_id,
                        principalTable: "attribute_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    brand_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_marketplace_brands_brand_id",
                        column: x => x.brand_id,
                        principalTable: "marketplace_brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_products_product_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "product_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    alt_text = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    is_main = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_images_file_entity_file_id",
                        column: x => x.file_id,
                        principalTable: "file_entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_images_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_variants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    sku = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    compare_at_price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_variants", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_variants_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_variant_attributes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    attribute_definition_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    attribute_option_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_variant_attributes", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_variant_attributes_attribute_definitions_attribute_",
                        column: x => x.attribute_definition_id,
                        principalTable: "attribute_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_variant_attributes_attribute_options_attribute_opti",
                        column: x => x.attribute_option_id,
                        principalTable: "attribute_options",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_variant_attributes_product_variants_product_variant",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_variant_inventories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    quantity_on_hand = table.Column<int>(type: "integer", nullable: false),
                    quantity_reserved = table.Column<int>(type: "integer", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_variant_inventories", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_variant_inventories_product_variants_product_varian",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_reservations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    product_variant_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    idempotency_key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stock_reservations", x => x.id);
                    table.ForeignKey(
                        name: "fk_stock_reservations_product_variants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "product_variants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_stock_reservations_user_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "attribute_definitions",
                columns: new[] { "id", "code", "is_filterable", "is_purchase_option", "name", "sort_order" },
                values: new object[] { new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b06"), "size", true, true, "Size", 0 });

            migrationBuilder.InsertData(
                table: "marketplace_brands",
                columns: new[] { "id", "name", "slug" },
                values: new object[] { new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02"), "FitHub", "fithub" });

            migrationBuilder.InsertData(
                table: "product_categories",
                columns: new[] { "id", "is_active", "name", "parent_id", "slug" },
                values: new object[] { new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01"), true, "Accessories", null, "accessories" });

            migrationBuilder.InsertData(
                table: "attribute_options",
                columns: new[] { "id", "attribute_definition_id", "sort_order", "value" },
                values: new object[] { new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b07"), new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b06"), 0, "M" });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "brand_id", "category_id", "created_at", "description", "is_active", "name", "slug", "updated_at", "version" },
                values: new object[] { new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03"), new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b02"), new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b01"), new DateTimeOffset(new DateTime(2026, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Demo marketplace product for local development.", true, "FitHub Training Mat", "fithub-training-mat", new DateTimeOffset(new DateTime(2026, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 0L });

            migrationBuilder.InsertData(
                table: "product_variants",
                columns: new[] { "id", "compare_at_price_amount", "currency", "is_active", "name", "price_amount", "product_id", "sku", "version" },
                values: new object[] { new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04"), null, "RUB", true, "Medium", 2490m, new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b03"), "FITHUB-MAT-M", 0L });

            migrationBuilder.InsertData(
                table: "product_variant_attributes",
                columns: new[] { "id", "attribute_definition_id", "attribute_option_id", "product_variant_id" },
                values: new object[] { new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b08"), new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b06"), new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b07"), new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04") });

            migrationBuilder.InsertData(
                table: "product_variant_inventories",
                columns: new[] { "id", "product_variant_id", "quantity_on_hand", "quantity_reserved", "version" },
                values: new object[] { new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b05"), new Guid("019ad0f7-1d2b-78c0-bf3b-3f7c88bd5b04"), 25, 0, 0L });

            migrationBuilder.CreateIndex(
                name: "ix_attribute_definitions_code",
                table: "attribute_definitions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_attribute_options_attribute_definition_id_value",
                table: "attribute_options",
                columns: new[] { "attribute_definition_id", "value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_marketplace_brands_slug",
                table: "marketplace_brands",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_categories_parent_id",
                table: "product_categories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_categories_slug",
                table: "product_categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_images_file_id",
                table: "product_images",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_images_product_id_sort_order",
                table: "product_images",
                columns: new[] { "product_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_product_variant_attributes_attribute_definition_id",
                table: "product_variant_attributes",
                column: "attribute_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_variant_attributes_attribute_option_id",
                table: "product_variant_attributes",
                column: "attribute_option_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_variant_attributes_product_variant_id_attribute_def",
                table: "product_variant_attributes",
                columns: new[] { "product_variant_id", "attribute_definition_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_variant_inventories_product_variant_id",
                table: "product_variant_inventories",
                column: "product_variant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_product_variants_product_id_is_active",
                table: "product_variants",
                columns: new[] { "product_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_product_variants_product_id_sku",
                table: "product_variants",
                columns: new[] { "product_id", "sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_brand_id",
                table: "products",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_category_id_is_active",
                table: "products",
                columns: new[] { "category_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_products_slug",
                table: "products",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stock_reservations_created_by_user_id",
                table: "stock_reservations",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_reservations_idempotency_key",
                table: "stock_reservations",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stock_reservations_product_variant_id_status_expires_at",
                table: "stock_reservations",
                columns: new[] { "product_variant_id", "status", "expires_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "product_variant_attributes");

            migrationBuilder.DropTable(
                name: "product_variant_inventories");

            migrationBuilder.DropTable(
                name: "stock_reservations");

            migrationBuilder.DropTable(
                name: "attribute_options");

            migrationBuilder.DropTable(
                name: "product_variants");

            migrationBuilder.DropTable(
                name: "attribute_definitions");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "marketplace_brands");

            migrationBuilder.DropTable(
                name: "product_categories");
        }
    }
}
