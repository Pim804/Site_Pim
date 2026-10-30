using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OptiStock.Infrastructure.Persistence.Migrations
{
    public partial class AddStockDatabaseObjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_StockMovements_Type_Valid",
                table: "StockMovements",
                sql: "\"Type\" IN ('Entry', 'Exit')");
            migrationBuilder.AddCheckConstraint(
                name: "CK_StockMovements_Balance_Matches_Type",
                table: "StockMovements",
                sql: "(\"Type\" = 'Entry' AND \"NewBalance\" = \"PreviousBalance\" + \"Quantity\") OR (\"Type\" = 'Exit' AND \"NewBalance\" = \"PreviousBalance\" - \"Quantity\")");

            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION fn_optistock_set_updated_at()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    NEW."UpdatedAtUtc" := CURRENT_TIMESTAMP;
                    RETURN NEW;
                END;
                $$;

                CREATE TRIGGER trg_parts_set_updated_at
                BEFORE UPDATE ON "Parts"
                FOR EACH ROW
                EXECUTE FUNCTION fn_optistock_set_updated_at();

                CREATE OR REPLACE FUNCTION fn_optistock_prevent_movement_change()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    RAISE EXCEPTION 'Stock movements are immutable; register a corrective movement instead.'
                        USING ERRCODE = '55000';
                END;
                $$;

                CREATE TRIGGER trg_stockmovements_immutable
                BEFORE UPDATE OR DELETE ON "StockMovements"
                FOR EACH ROW
                EXECUTE FUNCTION fn_optistock_prevent_movement_change();

                CREATE OR REPLACE VIEW vw_stock_overview AS
                SELECT
                    p."Id" AS part_id,
                    p."Code" AS code,
                    p."Name" AS name,
                    c."Name" AS category,
                    s."Name" AS supplier,
                    p."StockQuantity" AS quantity,
                    p."MinimumStock" AS minimum_stock,
                    p."UnitPrice" AS unit_price,
                    (p."StockQuantity" * p."UnitPrice")::numeric(14, 2) AS inventory_value,
                    p."WarehouseLocation" AS warehouse_location,
                    p."IsActive" AS is_active,
                    CASE
                        WHEN NOT p."IsActive" THEN 'Inactive'
                        WHEN p."StockQuantity" = 0 THEN 'OutOfStock'
                        WHEN p."StockQuantity" <= p."MinimumStock" THEN 'LowStock'
                        ELSE 'Available'
                    END AS stock_status,
                    p."CreatedAtUtc" AS created_at_utc,
                    p."UpdatedAtUtc" AS updated_at_utc
                FROM "Parts" AS p
                JOIN "Categories" AS c ON c."Id" = p."CategoryId"
                LEFT JOIN "Suppliers" AS s ON s."Id" = p."SupplierId";

                CREATE OR REPLACE VIEW vw_stock_movements_daily AS
                SELECT
                    (m."OccurredAtUtc" AT TIME ZONE 'UTC')::date AS movement_date_utc,
                    COUNT(*) FILTER (WHERE m."Type" = 'Entry')::bigint AS entry_count,
                    COUNT(*) FILTER (WHERE m."Type" = 'Exit')::bigint AS exit_count,
                    COALESCE(SUM(m."Quantity") FILTER (WHERE m."Type" = 'Entry'), 0)::bigint AS entry_quantity,
                    COALESCE(SUM(m."Quantity") FILTER (WHERE m."Type" = 'Exit'), 0)::bigint AS exit_quantity
                FROM "StockMovements" AS m
                GROUP BY (m."OccurredAtUtc" AT TIME ZONE 'UTC')::date;

                CREATE OR REPLACE FUNCTION fn_optistock_stock_summary()
                RETURNS TABLE (
                    active_parts bigint,
                    available_units bigint,
                    low_stock_parts bigint,
                    out_of_stock_parts bigint,
                    inventory_value numeric
                )
                LANGUAGE sql
                STABLE
                AS $$
                    SELECT
                        COUNT(*) FILTER (WHERE p."IsActive")::bigint,
                        COALESCE(SUM(p."StockQuantity") FILTER (WHERE p."IsActive"), 0)::bigint,
                        COUNT(*) FILTER (WHERE p."IsActive" AND p."StockQuantity" > 0 AND p."StockQuantity" <= p."MinimumStock")::bigint,
                        COUNT(*) FILTER (WHERE p."IsActive" AND p."StockQuantity" = 0)::bigint,
                        COALESCE(SUM(p."StockQuantity" * p."UnitPrice") FILTER (WHERE p."IsActive"), 0)::numeric
                    FROM "Parts" AS p;
                $$;

                CREATE OR REPLACE FUNCTION fn_optistock_movement_totals(
                    p_from timestamptz,
                    p_until timestamptz
                )
                RETURNS TABLE (
                    entry_count bigint,
                    exit_count bigint,
                    entry_quantity bigint,
                    exit_quantity bigint
                )
                LANGUAGE plpgsql
                STABLE
                AS $$
                BEGIN
                    IF p_from IS NULL OR p_until IS NULL OR p_until < p_from THEN
                        RAISE EXCEPTION 'The report period must have a start and an end, with end >= start.'
                            USING ERRCODE = '22023';
                    END IF;

                    RETURN QUERY
                    SELECT
                        COUNT(*) FILTER (WHERE m."Type" = 'Entry')::bigint,
                        COUNT(*) FILTER (WHERE m."Type" = 'Exit')::bigint,
                        COALESCE(SUM(m."Quantity") FILTER (WHERE m."Type" = 'Entry'), 0)::bigint,
                        COALESCE(SUM(m."Quantity") FILTER (WHERE m."Type" = 'Exit'), 0)::bigint
                    FROM "StockMovements" AS m
                    WHERE m."OccurredAtUtc" >= p_from
                      AND m."OccurredAtUtc" < p_until;
                END;
                $$;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP VIEW IF EXISTS vw_stock_movements_daily;
                DROP VIEW IF EXISTS vw_stock_overview;
                DROP TRIGGER IF EXISTS trg_stockmovements_immutable ON "StockMovements";
                DROP TRIGGER IF EXISTS trg_parts_set_updated_at ON "Parts";
                DROP FUNCTION IF EXISTS fn_optistock_movement_totals(timestamptz, timestamptz);
                DROP FUNCTION IF EXISTS fn_optistock_stock_summary();
                DROP FUNCTION IF EXISTS fn_optistock_prevent_movement_change();
                DROP FUNCTION IF EXISTS fn_optistock_set_updated_at();
                """);

            migrationBuilder.DropCheckConstraint(
                name: "CK_StockMovements_Balance_Matches_Type",
                table: "StockMovements");
            migrationBuilder.DropCheckConstraint(
                name: "CK_StockMovements_Type_Valid",
                table: "StockMovements");
        }
    }
}
