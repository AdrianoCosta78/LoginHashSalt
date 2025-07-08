using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginHashSalt.Models
{
    public class Usuario
    {
        public string Email { get; set; }
        public byte[] Salt { get; set; }
        public byte[] HashSenha { get; set; }
    }
}