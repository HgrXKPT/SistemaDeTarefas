using SistemaDeTarefas.Models;
using System;
using BCrypt.Net;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

class Program 
{ 
    public static void Main()
    {
        string resposta,nome, email, senha;

        Console.WriteLine("Bem vindo ao Agendador/Otimizador de tarefas da HG Store." +
            " Já possui conta? \n 1)Login \n 2)Criar");

        resposta = Console.ReadLine();

        switch (resposta)
        {
            case "1":
                Console.WriteLine("Digite seu email para login");
                email = Console.ReadLine();
                Console.WriteLine("Digite sua senha");
                senha = Console.ReadLine();

                bool login = LoginUsuario(email, senha).Result;

                if (login)
                {
                    Console.WriteLine("Login efetuado com sucesso");
                }
                else
                {
                    Console.WriteLine("senha ou email incorreto!");
                }

                    break;
            case "2":
                
                Console.WriteLine("Digite seu nome de usuario para cadastro");
                nome = Console.ReadLine();
                Console.WriteLine("Digite seu email para cadastro");
                email = Console.ReadLine();
                Console.WriteLine("Agora digite sua senha para cadastro");
                senha = Console.ReadLine();

                CadastroUsuario(nome, email, senha);
                break;
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
            return SenhaValida;
        }
        

    }

    public static void CadastroUsuario(string nome, string email, string senha)
    {
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
