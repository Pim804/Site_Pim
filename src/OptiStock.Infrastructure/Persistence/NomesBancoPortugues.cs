using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace OptiStock.Infrastructure.Persistence;

// Nomes fisicos do PostgreSQL. As classes C# e os contratos da API permanecem estaveis.
internal static class NomesBancoPortugues
{
    public static void Aplicar(ModelBuilder builder)
    {
        var tabelas = new Dictionary<string, string>
        {
            ["AspNetRoles"] = "perfis",
            ["AspNetRoleClaims"] = "permissoes_perfis",
            ["AspNetUsers"] = "usuarios",
            ["AspNetUserClaims"] = "permissoes_usuarios",
            ["AspNetUserLogins"] = "logins_usuarios",
            ["AspNetUserRoles"] = "usuarios_perfis",
            ["AspNetUserTokens"] = "tokens_usuarios",
            ["Categories"] = "categorias",
            ["Suppliers"] = "fornecedores",
            ["Parts"] = "pecas",
            ["StockMovements"] = "movimentacoes_estoque",
            ["Announcements"] = "comunicados",
        };
        var colunas = new Dictionary<string, string>
        {
            ["Id"] = "id",
            ["Name"] = "nome",
            ["Description"] = "descricao",
            ["IsActive"] = "ativo",
            ["CreatedAtUtc"] = "criado_em_utc",
            ["TaxId"] = "documento_fiscal",
            ["Phone"] = "telefone",
            ["Email"] = "email",
            ["Address"] = "endereco",
            ["Code"] = "codigo",
            ["CategoryId"] = "categoria_id",
            ["SupplierId"] = "fornecedor_id",
            ["StockQuantity"] = "quantidade_estoque",
            ["MinimumStock"] = "estoque_minimo",
            ["UnitPrice"] = "preco_unitario",
            ["WarehouseLocation"] = "localizacao_deposito",
            ["UpdatedAtUtc"] = "atualizado_em_utc",
            ["Version"] = "versao",
            ["PartId"] = "peca_id",
            ["Type"] = "tipo",
            ["Quantity"] = "quantidade",
            ["PreviousBalance"] = "saldo_anterior",
            ["NewBalance"] = "saldo_posterior",
            ["Notes"] = "observacao",
            ["OccurredAtUtc"] = "ocorrido_em_utc",
            ["UserId"] = "usuario_id",
            ["Title"] = "titulo",
            ["Content"] = "conteudo",
            ["PublishedAtUtc"] = "publicado_em_utc",
            ["IsPublished"] = "publicado",
            ["IsArchived"] = "arquivado",
            ["AuthorId"] = "autor_id",
            ["DisplayName"] = "nome_exibicao",
            ["UserName"] = "nome_usuario",
            ["NormalizedUserName"] = "nome_usuario_normalizado",
            ["NormalizedEmail"] = "email_normalizado",
            ["EmailConfirmed"] = "email_confirmado",
            ["PasswordHash"] = "hash_senha",
            ["SecurityStamp"] = "marca_seguranca",
            ["ConcurrencyStamp"] = "marca_concorrencia",
            ["PhoneNumber"] = "numero_telefone",
            ["PhoneNumberConfirmed"] = "telefone_confirmado",
            ["TwoFactorEnabled"] = "dois_fatores_habilitado",
            ["LockoutEnd"] = "fim_bloqueio",
            ["LockoutEnabled"] = "bloqueio_habilitado",
            ["AccessFailedCount"] = "tentativas_falhas",
            ["NormalizedName"] = "nome_normalizado",
            ["RoleId"] = "perfil_id",
            ["ClaimType"] = "tipo_permissao",
            ["ClaimValue"] = "valor_permissao",
            ["LoginProvider"] = "provedor_login",
            ["ProviderKey"] = "chave_provedor",
            ["ProviderDisplayName"] = "nome_exibicao_provedor",
            ["Value"] = "valor",
        };
        var entidades = builder.Model.GetEntityTypes()
            .Where(e => tabelas.ContainsKey(e.GetTableName() ?? "")).ToArray();
        foreach (var entidade in entidades)
        {
            var tabela = tabelas[entidade.GetTableName()!];
            entidade.SetTableName(tabela);
            foreach (var propriedade in entidade.GetProperties())
                propriedade.SetColumnName(colunas[propriedade.Name]);

            foreach (var chave in entidade.GetKeys())
                chave.SetName(NomeCurto($"pk_{tabela}"));
            foreach (var indice in entidade.GetIndexes())
                indice.SetDatabaseName(NomeCurto($"ix_{tabela}_{string.Join("_", indice.Properties.Select(p => colunas[p.Name]))}"));
        }
        foreach (var entidade in entidades)
            foreach (var chave in entidade.GetForeignKeys())
                chave.SetConstraintName(NomeCurto($"fk_{entidade.GetTableName()}_{chave.PrincipalEntityType.GetTableName()}_{string.Join("_", chave.Properties.Select(p => colunas[p.Name]))}"));
    }

    private static string NomeCurto(string nome)
    {
        if (nome.Length <= 63) return nome;
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(nome)))[..8].ToLowerInvariant();
        return nome[..54] + "_" + hash;
    }
}
