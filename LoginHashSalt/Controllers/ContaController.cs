using LoginHashSalt.Data;
using LoginHashSalt.Service;
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
                ViewBag.Mensagem = "Login realizado com sucesso!";
            }
            else
            {
                ViewBag.Mensagem = "Email ou senha inválidos!";
            }
            return View();
        }
    }
}