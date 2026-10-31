# OptiStock — Site e API

## 1. Identificação do projeto

**Projeto:** OptiStock — Sistema de gerenciamento de estoque  
**Componente:** aplicação web administrativa e API REST  
**Tecnologias principais:** C#, ASP.NET Core MVC, ASP.NET Core Web API, Entity Framework Core e PostgreSQL.

## 2. Resumo

O OptiStock foi desenvolvido para apoiar o controle de peças e materiais de uma empresa que antes dependia de anotações manuais. O site oferece uma interface administrativa para consultar e operar os dados de estoque. A API, hospedada pela mesma aplicação ASP.NET Core, fornece os serviços consumidos pelo aplicativo Flutter. Os dois clientes compartilham as mesmas regras de negócio e os mesmos registros armazenados no PostgreSQL.

## 3. Contextualização e problema

O registro manual de peças pode dificultar a localização de produtos, a atualização de saldos e a consulta do histórico de entradas e saídas. O projeto digitaliza essas rotinas e concentra as informações em um banco de dados relacional, com validações e rastreabilidade das movimentações.

## 4. Objetivos

### 4.1 Objetivo geral

Desenvolver uma aplicação integrada para facilitar o controle de estoque e o acesso às informações por meio de uma interface web e de um aplicativo mobile.

### 4.2 Objetivos específicos

- Organizar o cadastro e a consulta de peças.
- Registrar entradas e saídas com identificação do responsável.
- Disponibilizar informações de estoque e indicadores gerenciais.
- Centralizar serviços web e mobile em uma API REST.
- Persistir os dados em PostgreSQL com Entity Framework Core.

## 5. Arquitetura e organização

O projeto está dividido em camadas para separar interface, regras de aplicação, domínio e persistência:

- `src/OptiStock.Web`: site ASP.NET Core MVC, Razor Views e endpoints da API.
- `src/OptiStock.Application`: serviços e casos de uso da aplicação.
- `src/OptiStock.Domain`: entidades e regras centrais do domínio.
- `src/OptiStock.Infrastructure`: acesso a dados, Entity Framework Core, Identity e migrations.
- `tests/OptiStock.Tests`: projeto de testes automatizados.
- `OptiStock.slnx`: solução .NET.

O site e o aplicativo acessam os serviços no mesmo servidor. A API utiliza o prefixo `/api`. O acesso ao banco é feito pelo back-end; o aplicativo Flutter não se conecta diretamente ao PostgreSQL.

## 6. Requisitos e tecnologias

- Windows, PostgreSQL instalado e ativo, ferramenta `psql` e .NET SDK 10.
- O projeto Web utiliza o alvo `net10.0`.
- Os pacotes ASP.NET Core de autenticação JWT e OpenAPI estão na versão `10.0.12`.
- O acesso ao PostgreSQL é realizado com Entity Framework Core e o provedor Npgsql configurado no projeto de infraestrutura.

## 7. Funcionalidades

O sistema reúne autenticação e autorização, consulta e cadastro de peças, categorias, movimentações de estoque, comunicados e indicadores gerenciais. Os endpoints da API atendem às operações correspondentes do aplicativo. As permissões e validações são aplicadas no servidor.

## 8. Configuração e execução local

### 8.1 Preparação

Instale o .NET SDK 10 e o PostgreSQL. Durante a instalação do PostgreSQL, inclua as ferramentas de linha de comando para disponibilizar `psql`.

Na raiz do repositório, crie o arquivo de configuração local:

```powershell
Copy-Item .env.example .env
notepad .env
```

Substitua os valores de exemplo. Configure `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `Jwt__SigningKey`, `BootstrapAdmin__Email` e `BootstrapAdmin__Password`. A chave JWT deve conter pelo menos 32 bytes; a senha inicial deve atender aos requisitos do sistema. Não compartilhe nem envie o arquivo `.env` ao Git.

### 8.2 Inicialização

Com o serviço PostgreSQL ativo, execute na raiz do repositório:

```powershell
.\iniciar-local.ps1
```

Na primeira execução, o script poderá solicitar as credenciais administrativas do PostgreSQL para preparar o banco local. Em seguida, aplica as migrations e inicia o site e a API. Para encerrar, pressione `Ctrl+C` no terminal.

Endereços locais:

- Site: `http://localhost:5044`
- API: `http://localhost:5044/api`
- Documento OpenAPI: `http://localhost:5044/openapi/v1.json`

A conta administrativa inicial é criada com os valores configurados em `.env`. Não há credenciais de produção fixas no código.

## 9. Testes

Para executar os testes automatizados a partir da raiz do repositório:

```powershell
dotnet test Site_Pim\OptiStock.slnx
```

Os testes ficam em `tests/OptiStock.Tests`. Este README documenta o comando, mas não afirma que os testes foram executados.

## 10. Integração com o aplicativo

No emulador Android, o app deve acessar o computador anfitrião pelo endereço `http://10.0.2.2:5044/api`. Para executar em um aparelho físico, configure o endereço IP local do computador e permita o acesso à porta `5044` na rede local. Consulte o [README do aplicativo](../App_Pim/README.md).

## 11. Publicação

Para disponibilizar o sistema externamente, publique a aplicação ASP.NET Core em um serviço compatível com .NET 10 e configure as variáveis de ambiente e uma conexão PostgreSQL protegida. A URL usada pelo aplicativo deve ser HTTPS e terminar em `/api`. A configuração de publicação deve manter segredos fora do repositório.

## 12. Considerações finais e limitações

A aplicação depende de uma instância PostgreSQL acessível e de uma configuração válida para autenticação. O script `iniciar-local.ps1` foi preparado para desenvolvimento local em Windows. Para publicar em outro sistema operacional ou provedor, configure o banco, os segredos, o HTTPS e a inicialização das migrations conforme o ambiente escolhido.


## Nomes f?sicos do banco em portugu?s

A migration `TraduzirNomesBanco` renomeia as tabelas e colunas existentes, preservando os dados. `Persistence/NomesBancoPortugues.cs` mapeia as entidades e o Identity para os nomes f?sicos em portugu?s. Atualize o banco e o site juntos; vers?es antigas do site usam o esquema anterior. A tabela t?cnica `__EFMigrationsHistory` permanece no padr?o do EF Core.
