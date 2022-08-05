using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Empregado
    {
        public int EmpregadoId { get; set; }

        public string NomeEmpregado { get; set; }

        public string Departamento { get; set; }

        public string DatadeEntrada { get; set; }

        public string FotoFileName { get; set; }
    }
}
