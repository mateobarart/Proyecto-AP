using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class EquiposController : Controller
    {
        private Context db = new Context();

        // GET: Equipos  
        public ActionResult Index()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).ToList();
            return View(db.DbEquipos.ToList());
        }

        // GET: Equipos/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipo equipo = db.DbEquipos.Find(id);
            if (equipo == null)
            {
                return HttpNotFound();
            }
            return View(equipo);
        }

        // GET: Equipos/Create
        public ActionResult Create()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
            return View();
        }

        // POST: Equipos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string NombreEquipo, string Titular, string Suplente, string Reserva)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid) { 
                if (!((Titular.Equals(Suplente) && Titular != "") || (Titular.Equals(Reserva) && Titular != "") || (Suplente.Equals(Reserva) && Suplente != "")))
                {
                    Usuario titular = null;
                    Usuario suplente = null;
                    Usuario reserva = null;
                    if (Titular != "") titular = db.DbUsuarios.Find(Int32.Parse(Titular));
                    if (Suplente != "") suplente = db.DbUsuarios.Find(Int32.Parse(Suplente));
                    if (Reserva != "")  reserva = db.DbUsuarios.Find(Int32.Parse(Reserva));
                    try { 
                        Equipo equipo = new Equipo(NombreEquipo, titular, suplente, reserva);
                        db.DbEquipos.Add(equipo);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }catch (Exception)
                    {
                        ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
                        ModelState.AddModelError("CreateIncorrecto", "No se pudo crear el equipo. El nombre deben ser único");
                    }
                }
                else
                {
                    ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
                    ModelState.AddModelError("CreateIncorrecto", "Los analistas no pueden ser iguales.");
                }
                
            }
            return View();
        }

        // GET: Equipos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipo equipo = db.DbEquipos.Find(id);
            if (equipo == null)
            {
                return HttpNotFound();
            }
            ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
            return View(equipo);
        }

        // POST: Equipos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string NombreEquipo, string Titular, string Suplente, string Reserva, string idEquipo)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                if (!((Titular.Equals(Suplente) && Titular != "") || (Titular.Equals(Reserva) && Titular != "") || (Suplente.Equals(Reserva) && Suplente != "")))
                {
                try { 
                    Equipo equipo = db.DbEquipos.Find(Int32.Parse(idEquipo));
                    equipo.NombreEquipo = NombreEquipo;

                    if (Titular == "")  equipo.Titular = null;
                    else   equipo.Titular = db.DbUsuarios.Find(Int32.Parse(Titular));

                    if (Suplente == "")  equipo.Suplente = null;
                    else   equipo.Suplente = db.DbUsuarios.Find(Int32.Parse(Suplente));
                    
                    if (Reserva == "")  equipo.Reserva = null;
                    else   equipo.Reserva = db.DbUsuarios.Find(Int32.Parse(Reserva));
                    db.Entry(equipo).State = EntityState.Modified;
                    //Thread.Sleep(5000);
                    db.SaveChanges();
                }catch (Exception)
                {
                    ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
                    ModelState.AddModelError("EditIncorrecto", "No se pudo editar el equipo. El nombre deben ser único");
                }
            }
            else
                {
                    ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
                    ModelState.AddModelError("EditIncorrecto", "Los analistas no pueden ser iguales.");
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Equipos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipo equipo = db.DbEquipos.Find(id);
            if (equipo == null)
            {
                return HttpNotFound();
            }
            return View(equipo);
        }

        // POST: Equipos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            try
            {
                Equipo equipo = db.DbEquipos.Find(id);
                db.DbEquipos.Remove(equipo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("DeleteIncorrecto", "No se puede eliminar el equipo. Tiene partidos asignados.");
                return View();
            }
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
