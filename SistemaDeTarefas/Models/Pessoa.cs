using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeTarefas.Models
{
    class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public Credenciais Credenciais { get; set; }

        public ICollection<Tarefas> Tarefas { get; set; }
    }
}
