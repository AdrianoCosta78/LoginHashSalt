using LoginHashSalt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LoginHashSalt.Data
{
    public class LoginDbContext : DbContext
    {
        public LoginDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}