# Task Scheduler/Optimizer - HG Store

Este é um sistema simples de agendador e otimizador de tarefas desenvolvido em C#. 

## Funcionalidades

- **Login e Cadastro de Usuário**: Permite ao usuário criar uma conta ou fazer login se já possuir uma.
- **Cadastro de Tarefas**: Após o login, o usuário pode cadastrar novas tarefas.
- **Visualização de Tarefas**: Permite ao usuário visualizar as tarefas cadastradas.

## Tecnologias Utilizadas

- **C#**
- **.NET**
- **BCrypt.Net** para criptografia de senha
- **Entity Framework Core** para acesso ao banco de dados
- **Microsoft SQL Server** para armazenamento de dados

## Como usar

1. **Inicialização**: Ao iniciar o programa, você será saudado com uma mensagem de boas-vindas e terá a opção de criar uma conta ou fazer login.
2. **Login**: Se você já tiver uma conta, insira seu e-mail e senha. Se as credenciais estiverem corretas, você fará login com sucesso.
3. **Cadastro de Usuário**: Se você não tiver uma conta, insira seu nome, e-mail e senha para criar uma nova conta.
4. **Visualização de Tarefas**: Após o login, você pode optar por visualizar as tarefas cadastradas.
5. **Cadastro de Tarefas**: Você pode cadastrar novas tarefas inserindo o nome da tarefa e o tempo estimado para completá-la.

## Criando o Banco de Dados

Para configurar o banco de dados Microsoft SQL Server para este projeto, siga os passos abaixo:

1. **Instalar o SQL Server**: Se ainda não o tiver, baixe e instale o Microsoft SQL Server e o SQL Server Management Studio (SSMS).
2. **Criar um novo banco de dados**:
    - Abra o SSMS e conecte-se ao seu servidor.
    - Clique com o botão direito em "Databases" e selecione "New Database...".
    - Dê um nome ao seu banco de dados (por exemplo, `SistemaDeTarefas`) e clique em "OK".
3. **Criar as tabelas necessárias**:
    - Abra uma nova janela de consulta e insira os seguintes comandos SQL para criar as tabelas necessárias:

    ```sql
    CREATE TABLE Pessoas (
        PessoaId INT PRIMARY KEY IDENTITY(1,1),
        Nome NVARCHAR(100)
    );

    CREATE TABLE Credenciais (
        CredencialId INT PRIMARY KEY IDENTITY(1,1),
        Email NVARCHAR(100) NOT NULL,
        Senha NVARCHAR(200) NOT NULL,
        PessoaId INT,
        FOREIGN KEY (PessoaId) REFERENCES Pessoas(PessoaId)
    );

    CREATE TABLE Tarefas (
        TarefaId INT PRIMARY KEY IDENTITY(1,1),
        NomeTarefa NVARCHAR(100),
        TempoEstimado TIME,
        DiaCriacao DATETIME,
        PessoaId INT,
        FOREIGN KEY (PessoaId) REFERENCES Pessoas(PessoaId)
    );
    ```

4. **Configurar a conexão no Entity Framework**:
    - Certifique-se de que a classe `AppDbContext` está configurada conforme o código fornecido:

    ```csharp
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.SqlServer;

    namespace SistemaDeTarefas.Models
    {
        class AppDbContext : DbContext
        {
            public DbSet<Pessoa> Pessoas { get; set; }
            public DbSet<Credenciais> Credenciais { get; set; }
            public DbSet<Tarefas> Tarefas { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(
                "Server=Seu-Server;Database=SistemaDeTarefas;Trusted_Connection=True;TrustServerCertificate=True;");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Pessoa>()
                    .HasOne(p => p.Credenciais)
                    .WithOne(c => c.Pessoa)
                    .HasForeignKey<Credenciais>(c => c.PessoaId);

                modelBuilder.Entity<Tarefas>()
                    .HasOne(t => t.Pessoa)
                    .WithMany(p => p.Tarefas)
                    .HasForeignKey(p => p.PessoaId);
            }
        }
    }
    ```

