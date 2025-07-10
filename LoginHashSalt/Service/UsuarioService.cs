using LoginHashSalt.Data;
using LoginHashSalt.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Web.Services.Description;

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
        public Usuario GerarTokenRecuperacao(string email) 
        { 
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if(usuario == null)
                return null;

            usuario.TokenRecuperacao = Guid.NewGuid().ToString();   
            _context.SaveChanges();
            return usuario;
        }
        public void EnviarEmail(string para, string assunto, string corpo)
        {
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // 1) Configura o SMTP do Mailtrap
            var smtp = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("email", "senha"),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000    // opcional, 10s
            };

            // 2) Cria a mensagem passo a passo
            var mail = new MailMessage()
            {
                From = new MailAddress("no-reply@seuprojeto.com", "Seu Projeto"),
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = true
            };

            // Define o remetente como MailAddress
            
            // Adiciona o destinatário
            mail.To.Add(para);

            // 3) Envia
            smtp.Send(mail);
        }
        public Usuario BuscarPorToken(string token)
        {
            return _context.Usuarios.FirstOrDefault(u => u.TokenRecuperacao == token);
        }

        public void AtualizarUsuario(Usuario usuario)
        {
            _context.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
        }

    }
}
