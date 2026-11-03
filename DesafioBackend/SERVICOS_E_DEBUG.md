# 🚀 Guia de Serviços, Credenciais e Depuração no Visual Studio

Este documento reúne todas as informações de conectividade, portas, usuários, senhas e procedimentos passo a passo para depurar e executar a solução **DesafioBackend** utilizando o **Visual Studio 2022** e o **Docker**.

---

## 📌 1. Tabela Geral de Serviços e Portas

| Serviço | Host (Máquina Local / Windows) | Host (Rede Interna Docker) | Porta Host | Porta Container | Usuário | Senha | URL / Acesso |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **PostgreSQL** | `localhost` ou `127.0.0.1` | `db` | `5433` | `5432` | `postgres` | `postgrespassword` | `Host=localhost;Port=5433;Database=desafiodb;Username=postgres;Password=postgrespassword` |
| **RabbitMQ (Broker AMQP)** | `localhost` | `rabbitmq` | `5672` | `5672` | `guest` | `guest` | `amqp://guest:guest@localhost:5672` |
| **RabbitMQ (Painel Web)** | `localhost` | `rabbitmq` | `15672` | `15672` | `guest` | `guest` | [http://localhost:15672](http://localhost:15672) |
| **API Backend (.NET 8)** | `localhost` | `api` | `5000` | `8080` | `admin` | `admin` | [http://localhost:5000](http://localhost:5000) (Swagger UI) |
| **Painel Web MVC (.NET 8)** | `localhost` | `web` | `5001` | `8080` | `admin` | `admin` | [http://localhost:5001](http://localhost:5001) (Painel Admin) |

---

## 🔐 2. Credenciais Detalhadas de Acesso

### 🐘 PostgreSQL 16
* **Host Local**: `localhost` (Porta: `5433`)
* **Banco de Dados**: `desafiodb`
* **Usuário**: `postgres`
* **Senha**: `postgrespassword`
* **Connection String Local**:
  ```text
  Host=localhost;Port=5433;Database=desafiodb;Username=postgres;Password=postgrespassword
  ```
* **Connection String no Docker**:
  ```text
  Host=db;Port=5432;Database=desafiodb;Username=postgres;Password=postgrespassword
  ```

---

### 🐰 RabbitMQ 3 Management
* **Host Local**: `localhost` (Porta AMQP: `5672`, Porta Painel Web: `15672`)
* **Painel de Gestão (Interface Web)**: [http://localhost:15672](http://localhost:15672)
* **Usuário Padrão**: `guest`
* **Senha Padrão**: `guest`
* **Fila do Desafio**: `DesafioBackend.Domain.Events:MotorcycleRegisteredEvent`

---

### 🔑 API REST & Painel Web (Autenticação JWT & Cookies)
* **Usuário de Acesso**: `admin`
* **Senha de Acesso**: `admin`
* **Endpoint de Autenticação da API**: `POST http://localhost:5000/auth/login`
  ```json
  {
    "username": "admin",
    "password": "admin"
  }
  ```
* **Tela de Login do Painel Web**: [http://localhost:5001/Auth/Login](http://localhost:5001/Auth/Login)

---

## 🛠️ 3. Como Depurar no Visual Studio 2022

Existem duas maneiras recomendadas para depurar os serviços com o Visual Studio:

---

### 🌟 Opção 1: Depuração Local com Dependências no Docker (Recomendada)
Esta é a abordagem mais rápida e prática no dia a dia. Você mantém apenas o **PostgreSQL** e o **RabbitMQ** rodando em containers leves no Docker e executa o código C# da **API** e/ou da **Web** nativamente no Visual Studio com suporte total a **Hot Reload**, **Edit and Continue** e breakpoints instantâneos.

#### Passo 1: Iniciar apenas o banco e a mensageria no Docker
Abra o terminal na raiz do projeto (`C:\GitHub\Mottu-ops\DesafioBackend`) e execute:
```bash
docker compose up -d db rabbitmq
```
> *(Isso garante que o PostgreSQL na porta `5433` e o RabbitMQ nas portas `5672`/`15672` estejam ativos).*

#### Passo 2: Abrir a Solução no Visual Studio
1. Abra o **Visual Studio 2022**.
2. Abra a solução `DesafioBackend.sln`.

#### Passo 3: Configurar Inicialização Múltipla (API + Web juntos)
1. No **Gerenciador de Soluções** (*Solution Explorer*), clique com o botão direito sobre a solução **DesafioBackend**.
2. Selecione **Configurar Projetos de Inicialização...** (*Configure Startup Projects...*).
3. Marque a opção **Vários projetos de inicialização** (*Multiple startup projects*):
   - `DesafioBackend.Api` &rarr; Ação: **Iniciar** (*Start*)
   - `DesafioBackend.Web` &rarr; Ação: **Iniciar** (*Start*)
4. Clique em **OK**.

#### Passo 4: Pressionar `F5`
1. Pressione **`F5`** (ou clique no botão **Iniciar** / *Start*).
2. O Visual Studio iniciará:
   - A API em: `http://localhost:5000` (abrindo o Swagger)
   - O Painel Web em: `http://localhost:5001` (abrindo a tela de Login)
3. Você pode colocar pontos de interrupção (*breakpoints*) nos Controllers, Services, Handlers e Consumers e depurar linha por linha.

---

### 🐳 Opção 2: Depurar Diretamente dentro dos Containers Docker (Anexar ao Processo)
Se você preferir executar a stack completa dentro dos containers Docker e anexar o depurador do Visual Studio ao container em execução:

#### Passo 1: Iniciar toda a stack no Docker
```bash
docker compose up -d
```

#### Passo 2: Anexar ao Processo no Visual Studio
1. Com o Visual Studio 2022 aberto, vá no menu superior:
   👉 **Depurar** (*Debug*) &rarr; **Anexar ao Processo...** (*Attach to Process...*) ou pressione `Ctrl + Alt + P`.
2. No campo **Tipo de Conexão** (*Connection type*), selecione:
   👉 **Docker (Contêiner do Linux)**.
3. No campo **Destino da Conexão** (*Connection target*), clique em **Localizar...** (*Find...*):
   - Uma lista com os containers em execução será exibida.
   - Escolha o container que deseja depurar:
     - `desafio_backend_api` (para a API) ou
     - `desafio_backend_web` (para a aplicação Web MVC).
4. Na lista de processos disponíveis abaixo, selecione o processo:
   👉 `dotnet` (processo .NET em execução no container).
5. Clique em **Anexar** (*Attach*).
6. Coloque breakpoints no código C#. Ao fazer uma requisição no navegador ou Swagger, o Visual Studio pausará a execução exatamente no breakpoint dentro do container.

---

### 🔍 Dica: Janela "Contêineres" do Visual Studio
O Visual Studio 2022 possui uma ferramenta integrada excelente para gerenciar containers:
1. Pressione `Ctrl + K, Ctrl + O` ou acesse:
   👉 **Exibir** (*View*) &rarr; **Outras Janelas** (*Other Windows*) &rarr; **Contêineres** (*Containers*).
2. Por essa janela você consegue:
   - Ver logs em tempo real de cada container.
   - Ver portas expostas e variáveis de ambiente.
   - Abrir um terminal bash interativo dentro de qualquer container com um único clique.
   - Inspecionar arquivos criados dentro do volume do container.

---

## 📋 4. Comandos Úteis do Terminal

```bash
# Subir toda a stack em segundo plano
docker compose up -d

# Subir apenas o banco e mensageria (para desenvolvimento local)
docker compose up -d db rabbitmq

# Reconstruir imagens após alterações de código
docker compose build

# Ver logs da API em tempo real
docker logs -f desafio_backend_api

# Parar todos os serviços
docker compose down

# Rodar todos os testes automatizados da solução
dotnet test DesafioBackend.sln
```

