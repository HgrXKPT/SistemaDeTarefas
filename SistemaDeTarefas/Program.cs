using SistemaDeTarefas.Models;
using System;
using BCrypt.Net;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;

class Program 
{
    public static int PessoLoginId;
    public static void Main()
    {
        string resposta,nome, email, senha; 
         
            Console.WriteLine("Bem vindo ao Agendador/Otimizador de tarefas da HG Store." +
           "Já possui conta? \n 1)Login \n 2)Criar");
            resposta = Console.ReadLine();

            Console.Clear();

        while(resposta != "1" && resposta != "2")
        {
            Console.WriteLine("Escolha uma opção válida \n 1)Login \n 2)Criar Conta");
            resposta = Console.ReadLine();
            Console.Clear();

        }
        
            switch (resposta)
            {
                case "1":
                    RealizarLogin();
                    break;
                case "2":
                    CadastroUsuario();
                    break;            
            }

       
    }
    //metodo para ver as tarefas cadastradas
    public static async Task VerTarefasCadastradas()
    {
        using (var context = new AppDbContext())
        {
            var tarefas = context.Tarefas
                 .Where(t => t.PessoaId == PessoLoginId)
                 .Select(t => new
                 {
                     t.NomeTarefa,
                     t.TempoEstimado,
                     t.DiaCriacao,
                     pessoaNome = t.Pessoa.Nome
                 });

            Console.WriteLine($"Número de tarefas encontradas: {tarefas.Count()}");

            if (tarefas.Any())
            {
                foreach(var item in tarefas)
                {
                    Console.WriteLine($"Nome da pessoa: {item.pessoaNome} Tarefa: {item.NomeTarefa} Tempo estimado {item.TempoEstimado} Data Criação {item.DiaCriacao}");
                }
            }

            

        }
    }
    //metodo que cadastra a tarefa no banco de dados
    public static void CadastrarTarefa()
    {
        string nomeTarefa;
        TimeSpan tempoTarefa;
        Console.WriteLine("Otimo!!! Qual o nome para a tarefa?");
        nomeTarefa = Console.ReadLine();
        Console.Clear();
        Console.WriteLine("Qual o tempo estimado para a tarefa?");
        tempoTarefa = TimeSpan.Parse(Console.ReadLine());
        Console.Clear();

        try
        {
            using (var context = new AppDbContext())
            {
                var tarefa = new Tarefas
                {
                    NomeTarefa = nomeTarefa,
                    TempoEstimado = tempoTarefa,
                    DiaCriacao = DateTime.Now,
                    PessoaId = PessoLoginId
                };
                context.Add(tarefa);
                context.SaveChanges();


            }
        }catch(Exception ex)
        {
            throw new Exception("Ocorreu uma excessão: " + ex);
        }

        Console.WriteLine("Cadastro feito com sucesso.");
        

    }

    //metodo para efetuar login
    public static void RealizarLogin()
    {

        string email, senha, resposta;
        Console.WriteLine("Digite seu email para login");
        email = Console.ReadLine().ToLower();
        Console.WriteLine("Digite sua senha");
        senha = Console.ReadLine();
        Console.WriteLine("Processando, por favor, aguarde...");


        bool login = LoginUsuario(email, senha).Result;

        if (login)
        {
            Console.Clear();
            Console.WriteLine("Login efetuado com sucesso");
            Thread.Sleep(2000);
            Console.Clear();

            Console.WriteLine("Deseja ver tarefas cadastradas ou efetuar o cadastro? \n " +
                "1)Ver Tarefas \n 2)Efetuar Cadastro");
            resposta = Console.ReadLine();

            while (resposta != "1" && resposta != "2")
            {
                Console.WriteLine("Escolha uma opção válida \n 1)Ver Tarefas \n 2)Cadastrar Tarefas");
                resposta = Console.ReadLine();
                Console.Clear();

            }
            switch (resposta)
            {
                case "1":
                    VerTarefasCadastradas();
                    Console.WriteLine("Deseja cadastrar uma nova tarefa? \n 1) Sim \n 2) Não");
                    resposta = Console.ReadLine();

                    if(resposta == "1")
                    {
                        CadastrarTarefa();
                    }
                    break;
                case "2":
                    CadastrarTarefa();
                    Console.WriteLine("Deseja visualizar as tarefas? \n 1)Sim \n Não");
                    resposta = Console.ReadLine();

                    if (resposta == "1")
                    {
                        VerTarefasCadastradas();
                    }
                    break;
            }

        }
        else
        {
            Console.WriteLine("senha ou email incorreto!");
        }
    }


    //metodo que confere se o usuario realmente está cadastrado no banco de dados e retorna um valor bool
    public static async Task<bool> LoginUsuario(string email, string senha)
    {
        using(var context = new AppDbContext())
        {
            var credenciais = await context.Credenciais
                                            .Include(c => c.Pessoa)
                                            .FirstOrDefaultAsync(c => c.Email == email);
            if(credenciais == null)
            {
                return false;
            }

            //verifica o hash da senha
            bool SenhaValida = BCrypt.Net.BCrypt.Verify(senha, credenciais.Senha);
            if (SenhaValida)
            {
                PessoLoginId = credenciais.PessoaId;
            }
            return SenhaValida;
        }
        

    }

    public static void CadastroUsuario()
    {
        string nome, email, senha;
        Console.WriteLine("Digite seu nome de usuario para cadastro");
        nome = Console.ReadLine();
        Console.Clear();
        Console.WriteLine("Digite seu email para cadastro");
        email = Console.ReadLine().ToLower();
        Console.Clear();
        Console.WriteLine("Agora digite sua senha para cadastro");
        senha = Console.ReadLine();
        Console.Clear();

        try
        {
            using (var context = new AppDbContext())
            {
                var pessoa = new Pessoa
                {
                    Nome = nome,
                    Credenciais = new Credenciais
                    {
                        Email = email,
                        //criptografa a senha em hash
                        Senha = BCrypt.Net.BCrypt.HashPassword(senha)
                    },
                };
                context.Add(pessoa);
                context.SaveChanges();

            }
        }catch(Exception ex)
        {
            throw new Exception($"Ocorreu o seguinte erro: {ex}");
        }
        
    }
}
