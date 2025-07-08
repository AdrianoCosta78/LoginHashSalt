using LoginHashSalt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoginHashSalt.Service
{
    public class UsuarioService
    {
        private List<Usuario> Usuarios = new List<Usuario>();

        public byte[] GerarSalt()
        {
            var salt = new byte[16];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public byte[] GerarHash(string senha, byte[] salt)
        {
            using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(senha, salt, 10000))
            {
                return pbkdf2.GetBytes(2);
            }
        }

        public Usuario Cadastrar(string email, string senha)
        {
            var salt = GerarSalt();
            var hash = GerarHash(senha, salt);

            var usuario = new Usuario
            {
                Email = email,
                Salt = salt,
                HashSenha = hash
            };

            Usuarios.Add(usuario);
            return usuario;
        }

        public bool VerificarSenha(string email, string senhaDigitada)
        {
            var usuario = Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario == null)
                return false;

            var hasDigitado = GerarHash(senhaDigitada, usuario.Salt);
            return hasDigitado.SequenceEqual(usuario.HashSenha);
        }
    }
}