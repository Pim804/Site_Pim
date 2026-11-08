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
