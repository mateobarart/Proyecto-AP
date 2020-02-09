using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private Context db = new Context();
        public ActionResult Index()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            DateTime hoy = DateTime.Today;
            DateTime mañana = hoy.AddDays(1);
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
            {
                int idUsuario = int.Parse(Session["idUsuarioLogueado"].ToString());
                List<Partido> partidosLocal = db.DbPartidos.Where(p => p.FechaPartido >= hoy).Where(p => p.AnalistaLocal.IdUsuario ==  idUsuario).ToList();
                List<Partido> partidosVisitante = db.DbPartidos.Where(p => p.FechaPartido >= hoy).Where(p => p.AnalistaVisitante.IdUsuario == idUsuario).ToList();
                List<Partido> partidos = partidosLocal.Concat(partidosVisitante).ToList();
                return View(partidosLocal.Concat(partidosVisitante).OrderBy(p => p.FechaPartido).ToList());
            }
            return View(db.DbPartidos.Where(p => p.FechaPartido >= hoy).Where(p => p.FechaPartido < mañana).OrderBy(p => p.FechaPartido).ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}