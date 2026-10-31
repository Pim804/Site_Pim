using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OptiStock.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TraduzirNomesBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP VIEW IF EXISTS categorias;
                DROP VIEW IF EXISTS fornecedores;
                DROP VIEW IF EXISTS pecas;
                DROP VIEW IF EXISTS movimentacoes_estoque;
                DROP VIEW IF EXISTS comunicados;
                DROP VIEW IF EXISTS visao_geral_estoque;
                DROP VIEW IF EXISTS movimentacoes_diarias;
                DROP VIEW IF EXISTS vw_stock_overview;
                DROP VIEW IF EXISTS vw_stock_movements_daily;
                DROP TRIGGER IF EXISTS trg_parts_set_updated_at ON "Parts";
                DROP TRIGGER IF EXISTS trg_stockmovements_immutable ON "StockMovements";
                DROP FUNCTION IF EXISTS fn_optistock_movement_totals(timestamptz, timestamptz);
                DROP FUNCTION IF EXISTS fn_optistock_stock_summary();
                DROP FUNCTION IF EXISTS fn_optistock_prevent_movement_change();
                DROP FUNCTION IF EXISTS fn_optistock_set_updated_at();
                """);
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_AspNetUsers_AuthorId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Categories_CategoryId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_Suppliers_SupplierId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_AspNetUsers_UserId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Parts_PartId",
                table: "StockMovements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_TaxId",
                table: "Suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockMovements",
                table: "StockMovements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_StockMovements_Balances_NonNegative",
                table: "StockMovements");

            migrationBuilder.DropCheckConstraint(
                name: "CK_StockMovements_Quantity_Positive",
                table: "StockMovements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parts",
                table: "Parts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Parts_MinimumStock_NonNegative",
                table: "Parts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Parts_StockQuantity_NonNegative",
                table: "Parts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Parts_UnitPrice_NonNegative",
                table: "Parts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements");

            migrationBuilder.RenameTable(
                name: "Suppliers",
                newName: "fornecedores");

            migrationBuilder.RenameTable(
                name: "StockMovements",
                newName: "movimentacoes_estoque");

            migrationBuilder.RenameTable(
                name: "Parts",
                newName: "pecas");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "categorias");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "tokens_usuarios");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "usuarios");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "usuarios_perfis");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "logins_usuarios");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "permissoes_usuarios");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "perfis");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "permissoes_perfis");

            migrationBuilder.RenameTable(
                name: "Announcements",
                newName: "comunicados");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "fornecedores",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "fornecedores",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TaxId",
                table: "fornecedores",
                newName: "documento_fiscal");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "fornecedores",
                newName: "telefone");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "fornecedores",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "fornecedores",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "fornecedores",
                newName: "endereco");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "movimentacoes_estoque",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "movimentacoes_estoque",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "movimentacoes_estoque",
                newName: "tipo");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "movimentacoes_estoque",
                newName: "quantidade");

            migrationBuilder.RenameColumn(
                name: "PreviousBalance",
                table: "movimentacoes_estoque",
                newName: "saldo_anterior");

            migrationBuilder.RenameColumn(
                name: "PartId",
                table: "movimentacoes_estoque",
                newName: "peca_id");

            migrationBuilder.RenameColumn(
                name: "OccurredAtUtc",
                table: "movimentacoes_estoque",
                newName: "ocorrido_em_utc");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "movimentacoes_estoque",
                newName: "observacao");

            migrationBuilder.RenameColumn(
                name: "NewBalance",
                table: "movimentacoes_estoque",
                newName: "saldo_posterior");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_UserId",
                table: "movimentacoes_estoque",
                newName: "ix_movimentacoes_estoque_usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_PartId_OccurredAtUtc",
                table: "movimentacoes_estoque",
                newName: "ix_movimentacoes_estoque_peca_id_ocorrido_em_utc");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "pecas",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WarehouseLocation",
                table: "pecas",
                newName: "localizacao_deposito");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "pecas",
                newName: "versao");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "pecas",
                newName: "atualizado_em_utc");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "pecas",
                newName: "preco_unitario");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "pecas",
                newName: "fornecedor_id");

            migrationBuilder.RenameColumn(
                name: "StockQuantity",
                table: "pecas",
                newName: "quantidade_estoque");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "pecas",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "MinimumStock",
                table: "pecas",
                newName: "estoque_minimo");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "pecas",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "pecas",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "pecas",
                newName: "criado_em_utc");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "pecas",
                newName: "codigo");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "pecas",
                newName: "categoria_id");

            migrationBuilder.RenameIndex(
                name: "IX_Parts_SupplierId",
                table: "pecas",
                newName: "ix_pecas_fornecedor_id");

            migrationBuilder.RenameIndex(
                name: "IX_Parts_Name",
                table: "pecas",
                newName: "ix_pecas_nome");

            migrationBuilder.RenameIndex(
                name: "IX_Parts_Code",
                table: "pecas",
                newName: "ix_pecas_codigo");

            migrationBuilder.RenameIndex(
                name: "IX_Parts_CategoryId_IsActive",
                table: "pecas",
                newName: "ix_pecas_categoria_id_ativo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "categorias",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "categorias",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "categorias",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "categorias",
                newName: "descricao");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "categorias",
                newName: "criado_em_utc");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_Name",
                table: "categorias",
                newName: "ix_categorias_nome");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "tokens_usuarios",
                newName: "valor");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "tokens_usuarios",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "tokens_usuarios",
                newName: "provedor_login");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "tokens_usuarios",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "usuarios",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "usuarios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "usuarios",
                newName: "nome_usuario");

            migrationBuilder.RenameColumn(
                name: "TwoFactorEnabled",
                table: "usuarios",
                newName: "dois_fatores_habilitado");

            migrationBuilder.RenameColumn(
                name: "SecurityStamp",
                table: "usuarios",
                newName: "marca_seguranca");

            migrationBuilder.RenameColumn(
                name: "PhoneNumberConfirmed",
                table: "usuarios",
                newName: "telefone_confirmado");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "usuarios",
                newName: "numero_telefone");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "usuarios",
                newName: "hash_senha");

            migrationBuilder.RenameColumn(
                name: "NormalizedUserName",
                table: "usuarios",
                newName: "nome_usuario_normalizado");

            migrationBuilder.RenameColumn(
                name: "NormalizedEmail",
                table: "usuarios",
                newName: "email_normalizado");

            migrationBuilder.RenameColumn(
                name: "LockoutEnd",
                table: "usuarios",
                newName: "fim_bloqueio");

            migrationBuilder.RenameColumn(
                name: "LockoutEnabled",
                table: "usuarios",
                newName: "bloqueio_habilitado");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "usuarios",
                newName: "ativo");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmed",
                table: "usuarios",
                newName: "email_confirmado");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "usuarios",
                newName: "nome_exibicao");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "usuarios",
                newName: "criado_em_utc");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                table: "usuarios",
                newName: "marca_concorrencia");

            migrationBuilder.RenameColumn(
                name: "AccessFailedCount",
                table: "usuarios",
                newName: "tentativas_falhas");

            migrationBuilder.RenameIndex(
                name: "UserNameIndex",
                table: "usuarios",
                newName: "ix_usuarios_nome_usuario_normalizado");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_IsActive",
                table: "usuarios",
                newName: "ix_usuarios_ativo");

            migrationBuilder.RenameIndex(
                name: "EmailIndex",
                table: "usuarios",
                newName: "ix_usuarios_email_normalizado");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "usuarios_perfis",
                newName: "perfil_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "usuarios_perfis",
                newName: "usuario_id");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "usuarios_perfis",
                newName: "ix_usuarios_perfis_perfil_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "logins_usuarios",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "ProviderDisplayName",
                table: "logins_usuarios",
                newName: "nome_exibicao_provedor");

            migrationBuilder.RenameColumn(
                name: "ProviderKey",
                table: "logins_usuarios",
                newName: "chave_provedor");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "logins_usuarios",
                newName: "provedor_login");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "logins_usuarios",
                newName: "ix_logins_usuarios_usuario_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "permissoes_usuarios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "permissoes_usuarios",
                newName: "usuario_id");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "permissoes_usuarios",
                newName: "valor_permissao");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                table: "permissoes_usuarios",
                newName: "tipo_permissao");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "permissoes_usuarios",
                newName: "ix_permissoes_usuarios_usuario_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "perfis",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                table: "perfis",
                newName: "nome_normalizado");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "perfis",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                table: "perfis",
                newName: "marca_concorrencia");

            migrationBuilder.RenameIndex(
                name: "RoleNameIndex",
                table: "perfis",
                newName: "ix_perfis_nome_normalizado");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "permissoes_perfis",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "permissoes_perfis",
                newName: "perfil_id");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "permissoes_perfis",
                newName: "valor_permissao");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                table: "permissoes_perfis",
                newName: "tipo_permissao");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "permissoes_perfis",
                newName: "ix_permissoes_perfis_perfil_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "comunicados",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "comunicados",
                newName: "titulo");

            migrationBuilder.RenameColumn(
                name: "PublishedAtUtc",
                table: "comunicados",
                newName: "publicado_em_utc");

            migrationBuilder.RenameColumn(
                name: "IsPublished",
                table: "comunicados",
                newName: "publicado");

            migrationBuilder.RenameColumn(
                name: "IsArchived",
                table: "comunicados",
                newName: "arquivado");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "comunicados",
                newName: "criado_em_utc");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "comunicados",
                newName: "conteudo");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "comunicados",
                newName: "autor_id");

            migrationBuilder.RenameIndex(
                name: "IX_Announcements_IsPublished_IsArchived_PublishedAtUtc",
                table: "comunicados",
                newName: "ix_comunicados_publicado_arquivado_publicado_em_utc");

            migrationBuilder.RenameIndex(
                name: "IX_Announcements_AuthorId",
                table: "comunicados",
                newName: "ix_comunicados_autor_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_fornecedores",
                table: "fornecedores",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_movimentacoes_estoque",
                table: "movimentacoes_estoque",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_pecas",
                table: "pecas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_categorias",
                table: "categorias",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_tokens_usuarios",
                table: "tokens_usuarios",
                columns: new[] { "usuario_id", "provedor_login", "nome" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_usuarios",
                table: "usuarios",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_usuarios_perfis",
                table: "usuarios_perfis",
                columns: new[] { "usuario_id", "perfil_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_logins_usuarios",
                table: "logins_usuarios",
                columns: new[] { "provedor_login", "chave_provedor" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_permissoes_usuarios",
                table: "permissoes_usuarios",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_perfis",
                table: "perfis",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_permissoes_perfis",
                table: "permissoes_perfis",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_comunicados",
                table: "comunicados",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_fornecedores_documento_fiscal",
                table: "fornecedores",
                column: "documento_fiscal",
                unique: true,
                filter: "\"documento_fiscal\" IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_movimentacoes_quantidade_positiva",
                table: "movimentacoes_estoque",
                sql: "\"quantidade\" > 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_movimentacoes_saldos_nao_negativos",
                table: "movimentacoes_estoque",
                sql: "\"saldo_anterior\" >= 0 AND \"saldo_posterior\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_pecas_estoque_minimo_nao_negativo",
                table: "pecas",
                sql: "\"estoque_minimo\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_pecas_preco_nao_negativo",
                table: "pecas",
                sql: "\"preco_unitario\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_pecas_quantidade_nao_negativa",
                table: "pecas",
                sql: "\"quantidade_estoque\" >= 0");

            migrationBuilder.AddForeignKey(
                name: "fk_comunicados_usuarios_autor_id",
                table: "comunicados",
                column: "autor_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_logins_usuarios_usuarios_usuario_id",
                table: "logins_usuarios",
                column: "usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_estoque_pecas_peca_id",
                table: "movimentacoes_estoque",
                column: "peca_id",
                principalTable: "pecas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_movimentacoes_estoque_usuarios_usuario_id",
                table: "movimentacoes_estoque",
                column: "usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pecas_categorias_categoria_id",
                table: "pecas",
                column: "categoria_id",
                principalTable: "categorias",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_pecas_fornecedores_fornecedor_id",
                table: "pecas",
                column: "fornecedor_id",
                principalTable: "fornecedores",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_permissoes_perfis_perfis_perfil_id",
                table: "permissoes_perfis",
                column: "perfil_id",
                principalTable: "perfis",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_permissoes_usuarios_usuarios_usuario_id",
                table: "permissoes_usuarios",
                column: "usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tokens_usuarios_usuarios_usuario_id",
                table: "tokens_usuarios",
                column: "usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_usuarios_perfis_perfis_perfil_id",
                table: "usuarios_perfis",
                column: "perfil_id",
                principalTable: "perfis",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_usuarios_perfis_usuarios_usuario_id",
                table: "usuarios_perfis",
                column: "usuario_id",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.Sql("""
                ALTER TABLE "movimentacoes_estoque" RENAME CONSTRAINT "CK_StockMovements_Type_Valid" TO "ck_movimentacoes_tipo_valido";
                ALTER TABLE "movimentacoes_estoque" RENAME CONSTRAINT "CK_StockMovements_Balance_Matches_Type" TO "ck_movimentacoes_saldo_coerente";
                DO $renomear_sequencias$
                DECLARE nome_atual text;
                BEGIN
                    nome_atual := pg_get_serial_sequence('"categorias"', 'id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'categorias_id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"fornecedores"', 'id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'fornecedores_id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"permissoes_perfis"', 'id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'permissoes_perfis_id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"permissoes_usuarios"', 'id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'permissoes_usuarios_id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"pecas"', 'id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'pecas_id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"movimentacoes_estoque"', 'id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'movimentacoes_estoque_id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"comunicados"', 'id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'comunicados_id_seq'); END IF;
                END;
                $renomear_sequencias$;
                CREATE OR REPLACE FUNCTION fn_atualizar_data_peca()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    NEW."atualizado_em_utc" := CURRENT_TIMESTAMP;
                    RETURN NEW;
                END;
                $$;
                
                CREATE TRIGGER trg_pecas_atualizar_data
                BEFORE UPDATE ON "pecas"
                FOR EACH ROW
                EXECUTE FUNCTION fn_atualizar_data_peca();
                
                CREATE OR REPLACE FUNCTION fn_proteger_historico_movimentacoes()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    RAISE EXCEPTION 'Movimentacoes de estoque sao imutaveis; registre uma movimentacao de correcao.'
                        USING ERRCODE = '55000';
                END;
                $$;
                
                CREATE TRIGGER trg_movimentacoes_proteger_historico
                BEFORE UPDATE OR DELETE ON "movimentacoes_estoque"
                FOR EACH ROW
                EXECUTE FUNCTION fn_proteger_historico_movimentacoes();
                
                CREATE OR REPLACE VIEW visao_geral_estoque AS
                SELECT
                    p."id" AS peca_id,
                    p."codigo" AS codigo,
                    p."nome" AS nome,
                    c."nome" AS categoria,
                    s."nome" AS fornecedor,
                    p."quantidade_estoque" AS quantidade_estoque,
                    p."estoque_minimo" AS estoque_minimo,
                    p."preco_unitario" AS preco_unitario,
                    (p."quantidade_estoque" * p."preco_unitario")::numeric(14, 2) AS valor_estoque,
                    p."localizacao_deposito" AS localizacao_deposito,
                    p."ativo" AS ativo,
                    CASE
                        WHEN NOT p."ativo" THEN 'Inativo'
                        WHEN p."quantidade_estoque" = 0 THEN 'Sem estoque'
                        WHEN p."quantidade_estoque" <= p."estoque_minimo" THEN 'Estoque baixo'
                        ELSE 'Disponivel'
                    END AS situacao_estoque,
                    p."criado_em_utc" AS criado_em_utc,
                    p."atualizado_em_utc" AS atualizado_em_utc
                FROM "pecas" AS p
                JOIN "categorias" AS c ON c."id" = p."categoria_id"
                LEFT JOIN "fornecedores" AS s ON s."id" = p."fornecedor_id";
                
                CREATE OR REPLACE VIEW movimentacoes_diarias AS
                SELECT
                    (m."ocorrido_em_utc" AT TIME ZONE 'UTC')::date AS data_movimentacao_utc,
                    COUNT(*) FILTER (WHERE m."tipo" = 'Entry')::bigint AS numero_entradas,
                    COUNT(*) FILTER (WHERE m."tipo" = 'Exit')::bigint AS numero_saidas,
                    COALESCE(SUM(m."quantidade") FILTER (WHERE m."tipo" = 'Entry'), 0)::bigint AS quantidade_entradas,
                    COALESCE(SUM(m."quantidade") FILTER (WHERE m."tipo" = 'Exit'), 0)::bigint AS quantidade_saidas
                FROM "movimentacoes_estoque" AS m
                GROUP BY (m."ocorrido_em_utc" AT TIME ZONE 'UTC')::date;
                
                CREATE OR REPLACE FUNCTION fn_resumo_estoque()
                RETURNS TABLE (
                    pecas_ativas bigint,
                    unidades_disponiveis bigint,
                    pecas_estoque_baixo bigint,
                    pecas_sem_estoque bigint,
                    valor_estoque numeric
                )
                LANGUAGE sql
                STABLE
                AS $$
                    SELECT
                        COUNT(*) FILTER (WHERE p."ativo")::bigint,
                        COALESCE(SUM(p."quantidade_estoque") FILTER (WHERE p."ativo"), 0)::bigint,
                        COUNT(*) FILTER (WHERE p."ativo" AND p."quantidade_estoque" > 0 AND p."quantidade_estoque" <= p."estoque_minimo")::bigint,
                        COUNT(*) FILTER (WHERE p."ativo" AND p."quantidade_estoque" = 0)::bigint,
                        COALESCE(SUM(p."quantidade_estoque" * p."preco_unitario") FILTER (WHERE p."ativo"), 0)::numeric
                    FROM "pecas" AS p;
                $$;
                
                CREATE OR REPLACE FUNCTION fn_totais_movimentacoes(
                    p_inicio timestamptz,
                    p_fim timestamptz
                )
                RETURNS TABLE (
                    numero_entradas bigint,
                    numero_saidas bigint,
                    quantidade_entradas bigint,
                    quantidade_saidas bigint
                )
                LANGUAGE plpgsql
                STABLE
                AS $$
                BEGIN
                    IF p_inicio IS NULL OR p_fim IS NULL OR p_fim < p_inicio THEN
                        RAISE EXCEPTION 'O periodo deve ter inicio e fim, com fim maior ou igual ao inicio.'
                            USING ERRCODE = '22023';
                    END IF;
                
                    RETURN QUERY
                    SELECT
                        COUNT(*) FILTER (WHERE m."tipo" = 'Entry')::bigint,
                        COUNT(*) FILTER (WHERE m."tipo" = 'Exit')::bigint,
                        COALESCE(SUM(m."quantidade") FILTER (WHERE m."tipo" = 'Entry'), 0)::bigint,
                        COALESCE(SUM(m."quantidade") FILTER (WHERE m."tipo" = 'Exit'), 0)::bigint
                    FROM "movimentacoes_estoque" AS m
                    WHERE m."ocorrido_em_utc" >= p_inicio
                      AND m."ocorrido_em_utc" < p_fim;
                END;
                $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP VIEW IF EXISTS visao_geral_estoque;
                DROP VIEW IF EXISTS movimentacoes_diarias;
                DROP TRIGGER IF EXISTS trg_pecas_atualizar_data ON pecas;
                DROP TRIGGER IF EXISTS trg_movimentacoes_proteger_historico ON movimentacoes_estoque;
                DROP FUNCTION IF EXISTS fn_totais_movimentacoes(timestamptz, timestamptz);
                DROP FUNCTION IF EXISTS fn_resumo_estoque();
                DROP FUNCTION IF EXISTS fn_proteger_historico_movimentacoes();
                DROP FUNCTION IF EXISTS fn_atualizar_data_peca();
                """);
            migrationBuilder.DropForeignKey(
                name: "fk_comunicados_usuarios_autor_id",
                table: "comunicados");

            migrationBuilder.DropForeignKey(
                name: "fk_logins_usuarios_usuarios_usuario_id",
                table: "logins_usuarios");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_estoque_pecas_peca_id",
                table: "movimentacoes_estoque");

            migrationBuilder.DropForeignKey(
                name: "fk_movimentacoes_estoque_usuarios_usuario_id",
                table: "movimentacoes_estoque");

            migrationBuilder.DropForeignKey(
                name: "fk_pecas_categorias_categoria_id",
                table: "pecas");

            migrationBuilder.DropForeignKey(
                name: "fk_pecas_fornecedores_fornecedor_id",
                table: "pecas");

            migrationBuilder.DropForeignKey(
                name: "fk_permissoes_perfis_perfis_perfil_id",
                table: "permissoes_perfis");

            migrationBuilder.DropForeignKey(
                name: "fk_permissoes_usuarios_usuarios_usuario_id",
                table: "permissoes_usuarios");

            migrationBuilder.DropForeignKey(
                name: "fk_tokens_usuarios_usuarios_usuario_id",
                table: "tokens_usuarios");

            migrationBuilder.DropForeignKey(
                name: "fk_usuarios_perfis_perfis_perfil_id",
                table: "usuarios_perfis");

            migrationBuilder.DropForeignKey(
                name: "fk_usuarios_perfis_usuarios_usuario_id",
                table: "usuarios_perfis");

            migrationBuilder.DropPrimaryKey(
                name: "pk_usuarios_perfis",
                table: "usuarios_perfis");

            migrationBuilder.DropPrimaryKey(
                name: "pk_usuarios",
                table: "usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "pk_tokens_usuarios",
                table: "tokens_usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "pk_permissoes_usuarios",
                table: "permissoes_usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "pk_permissoes_perfis",
                table: "permissoes_perfis");

            migrationBuilder.DropPrimaryKey(
                name: "pk_perfis",
                table: "perfis");

            migrationBuilder.DropPrimaryKey(
                name: "pk_pecas",
                table: "pecas");

            migrationBuilder.DropCheckConstraint(
                name: "ck_pecas_estoque_minimo_nao_negativo",
                table: "pecas");

            migrationBuilder.DropCheckConstraint(
                name: "ck_pecas_preco_nao_negativo",
                table: "pecas");

            migrationBuilder.DropCheckConstraint(
                name: "ck_pecas_quantidade_nao_negativa",
                table: "pecas");

            migrationBuilder.DropPrimaryKey(
                name: "pk_movimentacoes_estoque",
                table: "movimentacoes_estoque");

            migrationBuilder.DropCheckConstraint(
                name: "ck_movimentacoes_quantidade_positiva",
                table: "movimentacoes_estoque");

            migrationBuilder.DropCheckConstraint(
                name: "ck_movimentacoes_saldos_nao_negativos",
                table: "movimentacoes_estoque");

            migrationBuilder.DropPrimaryKey(
                name: "pk_logins_usuarios",
                table: "logins_usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "pk_fornecedores",
                table: "fornecedores");

            migrationBuilder.DropIndex(
                name: "ix_fornecedores_documento_fiscal",
                table: "fornecedores");

            migrationBuilder.DropPrimaryKey(
                name: "pk_comunicados",
                table: "comunicados");

            migrationBuilder.DropPrimaryKey(
                name: "pk_categorias",
                table: "categorias");

            migrationBuilder.RenameTable(
                name: "usuarios_perfis",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "usuarios",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "tokens_usuarios",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "permissoes_usuarios",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "permissoes_perfis",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "perfis",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "pecas",
                newName: "Parts");

            migrationBuilder.RenameTable(
                name: "movimentacoes_estoque",
                newName: "StockMovements");

            migrationBuilder.RenameTable(
                name: "logins_usuarios",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "fornecedores",
                newName: "Suppliers");

            migrationBuilder.RenameTable(
                name: "comunicados",
                newName: "Announcements");

            migrationBuilder.RenameTable(
                name: "categorias",
                newName: "Categories");

            migrationBuilder.RenameColumn(
                name: "perfil_id",
                table: "AspNetUserRoles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "AspNetUserRoles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_usuarios_perfis_perfil_id",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "AspNetUsers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetUsers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tentativas_falhas",
                table: "AspNetUsers",
                newName: "AccessFailedCount");

            migrationBuilder.RenameColumn(
                name: "telefone_confirmado",
                table: "AspNetUsers",
                newName: "PhoneNumberConfirmed");

            migrationBuilder.RenameColumn(
                name: "numero_telefone",
                table: "AspNetUsers",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "nome_usuario_normalizado",
                table: "AspNetUsers",
                newName: "NormalizedUserName");

            migrationBuilder.RenameColumn(
                name: "nome_usuario",
                table: "AspNetUsers",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "nome_exibicao",
                table: "AspNetUsers",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "marca_seguranca",
                table: "AspNetUsers",
                newName: "SecurityStamp");

            migrationBuilder.RenameColumn(
                name: "marca_concorrencia",
                table: "AspNetUsers",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "hash_senha",
                table: "AspNetUsers",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "fim_bloqueio",
                table: "AspNetUsers",
                newName: "LockoutEnd");

            migrationBuilder.RenameColumn(
                name: "email_normalizado",
                table: "AspNetUsers",
                newName: "NormalizedEmail");

            migrationBuilder.RenameColumn(
                name: "email_confirmado",
                table: "AspNetUsers",
                newName: "EmailConfirmed");

            migrationBuilder.RenameColumn(
                name: "dois_fatores_habilitado",
                table: "AspNetUsers",
                newName: "TwoFactorEnabled");

            migrationBuilder.RenameColumn(
                name: "criado_em_utc",
                table: "AspNetUsers",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "bloqueio_habilitado",
                table: "AspNetUsers",
                newName: "LockoutEnabled");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "AspNetUsers",
                newName: "IsActive");

            migrationBuilder.RenameIndex(
                name: "ix_usuarios_nome_usuario_normalizado",
                table: "AspNetUsers",
                newName: "UserNameIndex");

            migrationBuilder.RenameIndex(
                name: "ix_usuarios_email_normalizado",
                table: "AspNetUsers",
                newName: "EmailIndex");

            migrationBuilder.RenameIndex(
                name: "ix_usuarios_ativo",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_IsActive");

            migrationBuilder.RenameColumn(
                name: "valor",
                table: "AspNetUserTokens",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "AspNetUserTokens",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "provedor_login",
                table: "AspNetUserTokens",
                newName: "LoginProvider");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "AspNetUserTokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetUserClaims",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "valor_permissao",
                table: "AspNetUserClaims",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "AspNetUserClaims",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "tipo_permissao",
                table: "AspNetUserClaims",
                newName: "ClaimType");

            migrationBuilder.RenameIndex(
                name: "ix_permissoes_usuarios_usuario_id",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetRoleClaims",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "valor_permissao",
                table: "AspNetRoleClaims",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "tipo_permissao",
                table: "AspNetRoleClaims",
                newName: "ClaimType");

            migrationBuilder.RenameColumn(
                name: "perfil_id",
                table: "AspNetRoleClaims",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "ix_permissoes_perfis_perfil_id",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "AspNetRoles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nome_normalizado",
                table: "AspNetRoles",
                newName: "NormalizedName");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "AspNetRoles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "marca_concorrencia",
                table: "AspNetRoles",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameIndex(
                name: "ix_perfis_nome_normalizado",
                table: "AspNetRoles",
                newName: "RoleNameIndex");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Parts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "versao",
                table: "Parts",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "quantidade_estoque",
                table: "Parts",
                newName: "StockQuantity");

            migrationBuilder.RenameColumn(
                name: "preco_unitario",
                table: "Parts",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Parts",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "localizacao_deposito",
                table: "Parts",
                newName: "WarehouseLocation");

            migrationBuilder.RenameColumn(
                name: "fornecedor_id",
                table: "Parts",
                newName: "SupplierId");

            migrationBuilder.RenameColumn(
                name: "estoque_minimo",
                table: "Parts",
                newName: "MinimumStock");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "Parts",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "criado_em_utc",
                table: "Parts",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "codigo",
                table: "Parts",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "categoria_id",
                table: "Parts",
                newName: "CategoryId");

            migrationBuilder.RenameColumn(
                name: "atualizado_em_utc",
                table: "Parts",
                newName: "UpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "Parts",
                newName: "IsActive");

            migrationBuilder.RenameIndex(
                name: "ix_pecas_nome",
                table: "Parts",
                newName: "IX_Parts_Name");

            migrationBuilder.RenameIndex(
                name: "ix_pecas_fornecedor_id",
                table: "Parts",
                newName: "IX_Parts_SupplierId");

            migrationBuilder.RenameIndex(
                name: "ix_pecas_codigo",
                table: "Parts",
                newName: "IX_Parts_Code");

            migrationBuilder.RenameIndex(
                name: "ix_pecas_categoria_id_ativo",
                table: "Parts",
                newName: "IX_Parts_CategoryId_IsActive");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "StockMovements",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "StockMovements",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "tipo",
                table: "StockMovements",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "saldo_posterior",
                table: "StockMovements",
                newName: "NewBalance");

            migrationBuilder.RenameColumn(
                name: "saldo_anterior",
                table: "StockMovements",
                newName: "PreviousBalance");

            migrationBuilder.RenameColumn(
                name: "quantidade",
                table: "StockMovements",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "peca_id",
                table: "StockMovements",
                newName: "PartId");

            migrationBuilder.RenameColumn(
                name: "ocorrido_em_utc",
                table: "StockMovements",
                newName: "OccurredAtUtc");

            migrationBuilder.RenameColumn(
                name: "observacao",
                table: "StockMovements",
                newName: "Notes");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_estoque_usuario_id",
                table: "StockMovements",
                newName: "IX_StockMovements_UserId");

            migrationBuilder.RenameIndex(
                name: "ix_movimentacoes_estoque_peca_id_ocorrido_em_utc",
                table: "StockMovements",
                newName: "IX_StockMovements_PartId_OccurredAtUtc");

            migrationBuilder.RenameColumn(
                name: "usuario_id",
                table: "AspNetUserLogins",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "nome_exibicao_provedor",
                table: "AspNetUserLogins",
                newName: "ProviderDisplayName");

            migrationBuilder.RenameColumn(
                name: "chave_provedor",
                table: "AspNetUserLogins",
                newName: "ProviderKey");

            migrationBuilder.RenameColumn(
                name: "provedor_login",
                table: "AspNetUserLogins",
                newName: "LoginProvider");

            migrationBuilder.RenameIndex(
                name: "ix_logins_usuarios_usuario_id",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Suppliers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Suppliers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "telefone",
                table: "Suppliers",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Suppliers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "endereco",
                table: "Suppliers",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "documento_fiscal",
                table: "Suppliers",
                newName: "TaxId");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "Suppliers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Announcements",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "titulo",
                table: "Announcements",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "publicado_em_utc",
                table: "Announcements",
                newName: "PublishedAtUtc");

            migrationBuilder.RenameColumn(
                name: "publicado",
                table: "Announcements",
                newName: "IsPublished");

            migrationBuilder.RenameColumn(
                name: "criado_em_utc",
                table: "Announcements",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "conteudo",
                table: "Announcements",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "autor_id",
                table: "Announcements",
                newName: "AuthorId");

            migrationBuilder.RenameColumn(
                name: "arquivado",
                table: "Announcements",
                newName: "IsArchived");

            migrationBuilder.RenameIndex(
                name: "ix_comunicados_publicado_arquivado_publicado_em_utc",
                table: "Announcements",
                newName: "IX_Announcements_IsPublished_IsArchived_PublishedAtUtc");

            migrationBuilder.RenameIndex(
                name: "ix_comunicados_autor_id",
                table: "Announcements",
                newName: "IX_Announcements_AuthorId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Categories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Categories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "descricao",
                table: "Categories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "criado_em_utc",
                table: "Categories",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "ativo",
                table: "Categories",
                newName: "IsActive");

            migrationBuilder.RenameIndex(
                name: "ix_categorias_nome",
                table: "Categories",
                newName: "IX_Categories_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parts",
                table: "Parts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockMovements",
                table: "StockMovements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Parts_MinimumStock_NonNegative",
                table: "Parts",
                sql: "\"MinimumStock\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Parts_StockQuantity_NonNegative",
                table: "Parts",
                sql: "\"StockQuantity\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Parts_UnitPrice_NonNegative",
                table: "Parts",
                sql: "\"UnitPrice\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_StockMovements_Balances_NonNegative",
                table: "StockMovements",
                sql: "\"PreviousBalance\" >= 0 AND \"NewBalance\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_StockMovements_Quantity_Positive",
                table: "StockMovements",
                sql: "\"Quantity\" > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TaxId",
                table: "Suppliers",
                column: "TaxId",
                unique: true,
                filter: "\"TaxId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_AspNetUsers_AuthorId",
                table: "Announcements",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Categories_CategoryId",
                table: "Parts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_Suppliers_SupplierId",
                table: "Parts",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_AspNetUsers_UserId",
                table: "StockMovements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Parts_PartId",
                table: "StockMovements",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.Sql("""
                ALTER TABLE "StockMovements" RENAME CONSTRAINT "ck_movimentacoes_tipo_valido" TO "CK_StockMovements_Type_Valid";
                ALTER TABLE "StockMovements" RENAME CONSTRAINT "ck_movimentacoes_saldo_coerente" TO "CK_StockMovements_Balance_Matches_Type";
                DO $renomear_sequencias$
                DECLARE nome_atual text;
                BEGIN
                    nome_atual := pg_get_serial_sequence('"Categories"', 'Id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'Categories_Id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"Suppliers"', 'Id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'Suppliers_Id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"AspNetRoleClaims"', 'Id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'AspNetRoleClaims_Id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"AspNetUserClaims"', 'Id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'AspNetUserClaims_Id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"Parts"', 'Id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'Parts_Id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"StockMovements"', 'Id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'StockMovements_Id_seq'); END IF;
                    nome_atual := pg_get_serial_sequence('"Announcements"', 'Id');
                    IF nome_atual IS NOT NULL THEN EXECUTE format('ALTER SEQUENCE %s RENAME TO %I', nome_atual, 'Announcements_Id_seq'); END IF;
                END;
                $renomear_sequencias$;
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
    }
}
