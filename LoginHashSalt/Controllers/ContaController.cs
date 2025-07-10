using LoginHashSalt.Data;
using LoginHashSalt.Service;
using System;
using System.Web.Mvc;

namespace LoginHashSalt.Controllers
{
    public class ContaController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public ContaController()
        {
            var context = new LoginDbContext();
            _usuarioService = new UsuarioService(context);
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Cadastro(string nome, string email, string senha)
        {
            _usuarioService.Cadastrar(nome, email, senha);
            ViewBag.Mensagem = "Usuário cadastrado com sucesso!";
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string senha)
        {
            var sucesso = _usuarioService.VerificarSenha(email, senha);
            if (sucesso)
            {
                 Session["UsuarioEmail"] = email;

                return RedirectToAction("AreaUsuario");
                //ViewBag.Mensagem = "Login realizado com sucesso!";
            }
            else
            {
                ViewBag.Mensagem = "Email ou senha inválidos!";
                return View();
            }
            
        }
        public ActionResult AreaUsuario()
        {
            // Só pra exemplo, exibe email que ficou salvo
            ViewBag.Email = Session["UsuarioEmail"];
            return View();
        }

        public ActionResult EsqueciSenha()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EsqueciSenha(string email)
        {
            var usuario = _usuarioService.GerarTokenRecuperacao(email);
            if (usuario != null)
            {
                var link = Url.Action(
                    "RedefinirSenha",
                    "Conta",
                    new { token = usuario.TokenRecuperacao },
                    protocol: Request.Url.Scheme);

                // Conteúdo HTML do email
                var corpoEmail = $@"
            <div style=""font-family: Arial, sans-serif; line-height:1.5; max-width:600px; margin:auto;"">
              <h2 style=""color:#333;"">Recuperação de Senha Solicitada</h2>
              <p>Olá,</p>
              <p>Recebemos uma solicitação para redefinir a senha da sua conta.</p>
              <p>Para criar uma nova senha, clique no link abaixo:</p>
              <p><a href=""{link}"" style=""background:#007bff; color:#fff; padding:10px 15px; text-decoration:none; border-radius:4px;"">Redefinir Minha Senha</a></p>
              <p>Se o botão não funcionar, copie e cole esta URL no seu navegador:</p>
              <p><code>{link}</code></p>
              <hr>
              <p style=""font-size:0.9em; color:#666;"">
                Se você não solicitou a recuperação de senha, simplesmente ignore este email.<br>
                Este link expira em 30 minutos por medida de segurança.
              </p>
              <p style=""font-size:0.8em; color:#999;"">
                &copy; {DateTime.Now.Year} Seu Projeto. Todos os direitos reservados.
              </p>
            </div>";

                // Envia para Mailtrap
                _usuarioService.EnviarEmail(usuario.Email, "Recuperação de Senha", corpoEmail);

                ViewBag.LinkTeste = link;  // só se quiser exibir na tela para testes
                ViewBag.Mensagem = "Email de recuperação enviado. Verifique sua caixa de entrada!";
            }
            else
            {
                ViewBag.Mensagem = "Email não encontrado!";
            }
            return View();
        }

        public ActionResult RedefinirSenha(string token)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public ActionResult RedefinirSenha(string token, string novaSenha, string confirmarSenha)
        {
            if (novaSenha != confirmarSenha)
            {
                ViewBag.Mensagem = "As senhas não conferem. Tente novamente.";
                ViewBag.Token = token;  // mantém o token para repost
                return View();
            }
            var usuario = _usuarioService.BuscarPorToken(token);
            if (usuario == null)
            {
                ViewBag.Mensagem = "Token inválido!";
                return View();
            }

            var salt = _usuarioService.GerarSalt();
            var hash = _usuarioService.GerarHash(novaSenha, salt);

            usuario.Salt = salt;
            usuario.SenhaHash = hash;
            usuario.TokenRecuperacao = null; // Limpa token

            _usuarioService.AtualizarUsuario(usuario);

            ViewBag.Mensagem = "Senha redefinida com sucesso!";
            return View();
        }
    }
}