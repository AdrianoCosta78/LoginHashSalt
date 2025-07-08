using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginHashSalt.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }   // PK
        public string Nome { get; set; }
        public string Email { get; set; }
        public string SenhaHash { get; set; }
        public string Salt { get; set; }
    }
}