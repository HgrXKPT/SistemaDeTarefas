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
            " Já possui conta? \n 1)Login \n 2)Criar");

        resposta = Console.ReadLine();

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

    public static async Task VerTarefasCadastradas()
    {
        using (var context = new AppDbContext())
        {
            Console.WriteLine($"PessoaLoginId: {PessoLoginId}");

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

    public static void CadastrarTarefa()
    {
        string nomeTarefa;
        TimeSpan tempoTarefa;
        Console.WriteLine("Otimo!!! Qual o nome para a tarefa?");
        nomeTarefa = Console.ReadLine();
        Console.WriteLine("Qual o tempo estimado para a tarefa?");
        tempoTarefa = TimeSpan.Parse(Console.ReadLine());
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

    public static void RealizarLogin()
    {

        string email, senha, resposta;
        Console.WriteLine("Digite seu email para login");
        email = Console.ReadLine();
        Console.WriteLine("Digite sua senha");
        senha = Console.ReadLine();

        bool login = LoginUsuario(email, senha).Result;

        if (login)
        {
            Console.WriteLine("Login efetuado com sucesso");
            Thread.Sleep(1500);
            Console.Clear();

            Console.WriteLine("Deseja ver tarefas cadastradas ou efetuar o cadastro? \n " +
                "1)Ver Tarefas \n 2)Efetuar Cadastro");
            resposta = Console.ReadLine();

            switch (resposta)
            {
                case "1":
                    VerTarefasCadastradas();
                    break;
                case "2":
                    CadastrarTarefa();
                    break;
            }

        }
        else
        {
            Console.WriteLine("senha ou email incorreto!");
        }
    }
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
        Console.WriteLine("Digite seu email para cadastro");
        email = Console.ReadLine();
        Console.WriteLine("Agora digite sua senha para cadastro");
        senha = Console.ReadLine();
        try {
            using (var context = new AppDbContext())
            {
                var pessoa = new Pessoa
                {
                    Nome = nome,
                    Credenciais = new Credenciais
                    {
                        Email = email,
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
