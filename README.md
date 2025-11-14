# InventarioAPI - Backend .NET 8

Esta é uma API RESTful de backend construída com **.NET 8** e **C#** como parte de um projeto de portfólio. A API gerencia um inventário simples de ativos de TI e é protegida por autenticação baseada em **Token JWT**.

O projeto foi desenvolvido com foco em boas práticas de .NET, arquitetura de API moderna (Minimal APIs) e segurança.

---

## 🚀 Tecnologias Utilizadas

* **.NET 8:** Versão mais recente e LTS (Long-Term Support) da plataforma.
* **C# 12:** Linguagem principal do projeto.
* **Minimal APIs:** Abordagem moderna para criar APIs leves e rápidas no .NET.
* **Entity Framework Core (EF Core):** Para o acesso a dados.
    * **In-Memory Database:** Usado para desenvolvimento e testes rápidos.
* **Autenticação JWT Bearer:** Para proteger os endpoints da API.
* **BCrypt.Net:** Para fazer o hash e verificação segura de senhas.
* **Swagger/OpenAPI:** Para documentação e teste interativo dos endpoints.

---

## 📋 Funcionalidades

A API expõe dois conjuntos principais de endpoints:

### Autenticação (`/auth`)
* `POST /auth/register`: Registra um novo usuário com senha criptografada.
* `POST /auth/login`: Autentica um usuário e retorna um token JWT válido por 8 horas.

### Inventário (`/api/ativos`)
*Todos os endpoints deste grupo são **protegidos** e exigem um token JWT válido.*

* `GET /api/ativos`: Lista todos os ativos cadastrados.
* `GET /api/ativos/{id}`: Busca um ativo específico pelo seu ID.
* `POST /api/ativos`: Cadastra um novo ativo no sistema.
* `PUT /api/ativos/{id}`: Atualiza um ativo existente.
* `DELETE /api/ativos/{id}`: Deleta um ativo do sistema.

---

## 🔧 Como Executar Localmente

**Pré-requisitos:**
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

**Passos:**

1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/SEU-USUARIO/InventarioAPI-Backend.git](https://github.com/SEU-USUARIO/InventarioAPI-Backend.git)
    cd InventarioAPI-Backend
    ```

2.  **Crie o arquivo de configuração:**
    * Na raiz do projeto, renomeie `appsettings.Template.json` para `appsettings.Development.json`.
    * Abra o arquivo e preencha a `Jwt:Key` com uma chave secreta de sua escolha (deve ter **32 caracteres ou mais**).

3.  **Execute a aplicação:**
    ```bash
    dotnet run
    ```

4.  **Acesse a documentação:**
    * Abra seu navegador e vá para a URL indicada no terminal (ex: `http://localhost:5062/swagger`) para testar os endpoints.