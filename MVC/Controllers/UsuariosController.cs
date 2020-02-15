using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class UsuariosController : Controller
    {
        private Context db = new Context();

        // GET: Usuarios
        public ActionResult Index()
        {
            if (Session["mailUsuarioLogueado"] == null ) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            return View(db.DbUsuarios.Where(u => u.Activo == true).ToList());
        }


        // GET: Usuarios/VerIndisponibilidades/5
        public ActionResult VerIndisponibilidades(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("IndisponibilidadesUsuario", "Indisponibilidades", new { id = id });
        }
        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
            {
                int idUsuario = int.Parse(Session["idUsuarioLogueado"].ToString());
                if(idUsuario != id) return RedirectToAction("Index", "Home");
            }

            Usuario usuario = db.DbUsuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            List<string> listaTiposUsuario = new List<string>();
            listaTiposUsuario.Add("Analista");
            string tipoUsuarioLogueado = Session["tipoUsuarioLogueado"].ToString();
            if(tipoUsuarioLogueado == "Manager")
            {
                listaTiposUsuario.Add("Manager");
                listaTiposUsuario.Add("TeamLeader");
                listaTiposUsuario.Add("Supervisor");
            }else if (tipoUsuarioLogueado == "TeamLeader")
            {
                listaTiposUsuario.Add("TeamLeader");
                listaTiposUsuario.Add("Supervisor");
            }else if(tipoUsuarioLogueado == "Supervisor")
            {
                listaTiposUsuario.Add("Supervisor");
            }
            ViewBag.ListaTiposUsuario = listaTiposUsuario;
            return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUsuario,NombreUsuario,Password,TipoUsuario,Mail")] Usuario usuario, string passwordValidacion)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");

            if (usuario.Password.Equals(passwordValidacion))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.DbUsuarios.Add(usuario);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("Create", "No se pudo crear el usuario. El nombre de usuario y el mail deben ser únicos");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("Password", "Las contraseñas deben coincidir");
            }

            List<string> listaTiposUsuario = new List<string>();
            listaTiposUsuario.Add("Analista");
            string tipoUsuarioLogueado = Session["tipoUsuarioLogueado"].ToString();
            if (tipoUsuarioLogueado == "Manager")
            {
                listaTiposUsuario.Add("Manager");
                listaTiposUsuario.Add("TeamLeader");
                listaTiposUsuario.Add("Supervisor");
            }
            else if (tipoUsuarioLogueado == "TeamLeader")
            {
                listaTiposUsuario.Add("TeamLeader");
                listaTiposUsuario.Add("Supervisor");
            }
            else if (tipoUsuarioLogueado == "Supervisor")
            {
                listaTiposUsuario.Add("Supervisor");
            }
            ViewBag.ListaTiposUsuario = listaTiposUsuario;



            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
            {
                int idUsuario = int.Parse(Session["idUsuarioLogueado"].ToString());
                if (idUsuario != id) return RedirectToAction("Index", "Home");
            }
            Usuario usuario = db.DbUsuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            List<string> listaTiposUsuario = new List<string>();
            listaTiposUsuario.Add("Analista");
            string tipoUsuarioLogueado = Session["tipoUsuarioLogueado"].ToString();
            if (tipoUsuarioLogueado == "Manager")
            {
                listaTiposUsuario.Add("Manager");
                listaTiposUsuario.Add("TeamLeader");
                listaTiposUsuario.Add("Supervisor");
            }
            else if (tipoUsuarioLogueado == "TeamLeader")
            {
                listaTiposUsuario.Add("TeamLeader");
                listaTiposUsuario.Add("Supervisor");
            }
            else if (tipoUsuarioLogueado == "Supervisor")
            {
                listaTiposUsuario.Add("Supervisor");
            }
            ViewBag.ListaTiposUsuario = listaTiposUsuario;
            return View(usuario);
        }

        // GET: Ranking
        public ActionResult Ranking()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            return View(db.DbUsuarios.Where(u => u.TipoUsuario == TipoUsuario.Analista).Where(u => u.Activo == true).OrderBy(u => u.NombreUsuario).OrderBy(u => u.Puntaje).ToList());
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUsuario,NombreUsuario,Password,TipoUsuario,Mail,Puntaje")] Usuario usuario)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
            try { 
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }catch (Exception)
            {
                ModelState.AddModelError("Edit", "No se pudo editar el usuario. El nombre de usuario y el mail deben ser únicos");
            }
        }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.DbUsuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult DeshabilitarUsuario(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.DbUsuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            usuario.Activo = false;
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult HabilitarUsuario(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.DbUsuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            usuario.Activo = true;
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UsuariosInhabilitados()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            return View(db.DbUsuarios.Where(u => u.Activo == false).ToList());
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            Usuario usuario = db.DbUsuarios.Find(id);
            db.DbUsuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
