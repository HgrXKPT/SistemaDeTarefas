using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaDeTarefas.Models
{
    class Tarefas
    {
        public int Id { get; set; }
        public string NomeTarefa { get; set; }
        public DateTime DiaCriacao { get; set; }

        public TimeSpan TempoEstimado { get; set; }

        public int PessoaId { get; set; }

        public Pessoa Pessoa { get; set; }
    }
}
