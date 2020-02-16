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
    public class CompeticionesController : Controller
    {
        private Context db = new Context();

        // GET: Competiciones
        public ActionResult Index()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            //var x = db.DbCompeticiones.Select(c => c.NombreCompeticion).Distinct().AsEnumerable().ToList();
            List<Competicion> competiciones = db.DbCompeticiones.Include(c => c.Equipos).ToList();
            List<Competicion> nuevaLista = new List<Competicion>();
                for (int i = 0; i < competiciones.Count; i++)
                {
                    // Assume not duplicate.
                    bool duplicate = false;
                    for (int z = 0; z < i; z++)
                    {
                        if (competiciones[z].NombreCompeticion.Equals(competiciones[i].NombreCompeticion))
                        {
                            // This is a duplicate.
                            duplicate = true;
                            break;
                        }
                    }
                    // If not duplicate, add to result.
                    if (!duplicate)
                    {
                    nuevaLista.Add(competiciones[i]);
                    }
                }
                return View(nuevaLista);
            
            //return View(db.DbCompeticiones.ToList());
        }

        // GET: Competiciones/VerEquipos/5
        public ActionResult VerEquipos(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competicion competicion = db.DbCompeticiones.Where(c => c.IdCompeticion == id).Include(c => c.Equipos).FirstOrDefault();
            if (competicion == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdCompeticion = competicion.IdCompeticion;
            return View(competicion.Equipos.ToList());
        }

        // GET: Competiciones/QuitarEquipo/5
        public ActionResult QuitarEquipo(int? id, string idCompeticion)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int idC = Int32.Parse(idCompeticion);
            Competicion competicion = db.DbCompeticiones.Where(c => c.IdCompeticion == idC).Include(c => c.Equipos).FirstOrDefault();
            Equipo equipo = db.DbEquipos.Find(id);
            if (competicion == null)
            {
                return HttpNotFound();
            }
            competicion.Equipos.Remove(equipo);
            db.Entry(competicion).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("VerEquipos", "Competiciones", new { id = idC });
        }

        // GET: Competiciones/AgregarEquipos/5
        public ActionResult AgregarEquipo(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.ListaEquipos = db.DbEquipos.ToList();
            ViewBag.IdCompeticion = id;
            return View();
        }

        // GET: Competiciones/AgregarEquipos/5
        public ActionResult AgregarEquipoACompeticion(string IdCompeticion, string Equipo)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            Equipo equipo = db.DbEquipos.Find(Int32.Parse(Equipo));
            Competicion competicion = db.DbCompeticiones.Find(Int32.Parse(IdCompeticion));
            if(competicion != null && equipo != null && !competicion.Equipos.Contains(equipo))
            {
                competicion.Equipos.Add(equipo);
                db.Entry(competicion).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("VerEquipos", "Competiciones", new { id = IdCompeticion });
        }



        // GET: Competiciones/VerEdiciones/5
        public ActionResult VerEdiciones(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competicion competicion = db.DbCompeticiones.Find(id);
            if (competicion == null)
            {
                return HttpNotFound();
            }
            return View(db.DbCompeticiones.Where(x => x.NombreCompeticion == competicion.NombreCompeticion).Include(c => c.Equipos).ToList());
        }

        // GET: Competiciones/Create
        public ActionResult Create()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            return View();
        }

        // POST: Competiciones/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string NombreCompeticion)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                    try
                    {
                        Competicion competicion = new Competicion(NombreCompeticion);
                        db.DbCompeticiones.Add(competicion);
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("CreateIncorrecto", "No se pudo crear el equipo. El nombre debe ser único");
                        return View();
                    }
            }
            return RedirectToAction("Index");
        }

        // GET: Competiciones/CrearEdicion
        public ActionResult CrearEdicion(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            Competicion c = db.DbCompeticiones.Find(id);
            return View(c);
        }

        // POST: Competiciones/CrearEdicion
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearEdicion(string nombreCompeticion, string temporada, int prioridad)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                if (nombreCompeticion != "" && temporada != "" && prioridad > 0 && prioridad <= 5)
                {
                    List <Competicion> competiciones = db.DbCompeticiones.Where(x => x.NombreCompeticion == nombreCompeticion).Include(c => c.Equipos).ToList();
                    foreach (Competicion c in competiciones)
                    {
                        if(c.Temporada == temporada) return RedirectToAction("Index");
                    }
                    Competicion competicion = new Competicion(nombreCompeticion, temporada, prioridad);
                    db.DbCompeticiones.Add(competicion);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Competiciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home"); if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competicion competicion = db.DbCompeticiones.Find(id);
            if (competicion == null)
            {
                return HttpNotFound();
            }
            return View(competicion);
        }

        // POST: Competiciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string nombreCompeticion, int id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home"); 
            if (ModelState.IsValid)
            {
                Competicion competicion = db.DbCompeticiones.Find(id);
                List<Competicion> competiciones = db.DbCompeticiones.Where(c => c.NombreCompeticion == competicion.NombreCompeticion).Include(c => c.Equipos).ToList();
                foreach (Competicion c in competiciones)
                {
                    c.NombreCompeticion = nombreCompeticion;
                    db.Entry(c).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Competiciones/EditarEdicion/5
        public ActionResult EditarEdicion(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home"); if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competicion competicion = db.DbCompeticiones.Find(id);
            if (competicion == null)
            {
                return HttpNotFound();
            }
            return View(competicion);
        }

        // POST: Competiciones/EditarEdicion/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEdicion(string nombreCompeticion, string temporada, int prioridad, int id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                Competicion comp = db.DbCompeticiones.Where(c => c.NombreCompeticion == nombreCompeticion).Where(c => c.Temporada == temporada).Include(c => c.Equipos).FirstOrDefault();
                if(comp == null)
                {
                    comp = db.DbCompeticiones.Find(id);
                    comp.NombreCompeticion = nombreCompeticion;
                    comp.Temporada = temporada;
                    comp.Prioridad= prioridad;
                    db.Entry(comp).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("VerEdiciones", new { id = id});
        }

        // GET: Competiciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competicion competicion = db.DbCompeticiones.Find(id);
            if (competicion == null)
            {
                return HttpNotFound();
            }
            return View(competicion);
        }

        // POST: Competiciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            try
            {
                Competicion competicion = db.DbCompeticiones.Find(id);
                List<Competicion> competiciones = db.DbCompeticiones.Where(c => c.NombreCompeticion == competicion.NombreCompeticion).Include(c => c.Equipos).ToList();
                for (int i = competiciones.Count() - 1; i >= 0; i--)
                {
                    db.DbCompeticiones.Remove(competiciones[i]);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("DeleteIncorrecto", "No se puede eliminar la competición. Tiene equipos asignados.");
                return View();
            }
           
        }

        // GET: Competiciones/EliminarEdicion/5
        public ActionResult EliminarEdicion(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Competicion competicion = db.DbCompeticiones.Find(id);
            if (competicion == null)
            {
                return HttpNotFound();
            }
            return View(competicion);
        }

        // POST: Competiciones/EliminarEdicionConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarEdicionConfirmed(string IdCompeticion)
        {
            try
            {
                if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
                if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
                int idCompeticion = int.Parse(IdCompeticion);
                Competicion competicion = db.DbCompeticiones.Find(idCompeticion);
                db.DbCompeticiones.Remove(competicion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("DeleteIncorrecto", "No se puede eliminar la edición. Tiene equipos asignados.");
                return RedirectToAction("EliminarEdicion","Competiciones",new { id = IdCompeticion });
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
