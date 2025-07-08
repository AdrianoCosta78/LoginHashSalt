using LoginHashSalt.Data;
using LoginHashSalt.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LoginHashSalt.Service
{
    public class UsuarioService
    {
        private readonly LoginDbContext _context;

        public UsuarioService(LoginDbContext context)
        {
            _context = context;
        }

        public string GerarSalt()
        {
            var saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public string GerarHash(string senha, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combined = Encoding.UTF8.GetBytes(senha + salt);
                var hash = sha256.ComputeHash(combined);
                return Convert.ToBase64String(hash);
            }
        }

        public Usuario Cadastrar(string nome, string email, string senha)
        {
            var salt = GerarSalt();
            var hash = GerarHash(senha, salt);

            var usuario = new Usuario
            {
                Nome = nome,
                Email = email,
                Salt = salt,
                SenhaHash = hash
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
            return usuario;
        }

        public bool VerificarSenha(string email, string senhaDigitada)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario == null)
                return false;

            var hashDigitado = GerarHash(senhaDigitada, usuario.Salt);
            return hashDigitado == usuario.SenhaHash;
        }
    }
}
