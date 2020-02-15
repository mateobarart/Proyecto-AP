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
    public class IndisponibilidadesController : Controller
    {
        private Context db = new Context();

        // GET: Indisponibilidades
        public ActionResult Index()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            List<Indisponibilidad> ins = db.Indisponibilidads.Include("Usuario").ToList();
            List<IndisponibilidadRecurrente> ir = new List<IndisponibilidadRecurrente>();
            List<IndisponibilidadUnica> iu = new List<IndisponibilidadUnica>();
            foreach (Indisponibilidad i in ins)
            {
                if(i.Usuario != null && i.Usuario.Activo)
                {
                    if(i is IndisponibilidadRecurrente)
                    {
                        ir.Add((IndisponibilidadRecurrente)i);
                    }
                    else
                    {
                        iu.Add((IndisponibilidadUnica)i);
                    }
                }

            }
            ViewBag.ListaRecurrentes = ir;
            ViewBag.ListaUnicas = iu;
            return View();
        }

        // GET: Indisponibilidades
        public ActionResult IndisponibilidadesUsuario(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            
            Usuario usuario = db.DbUsuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
            {
                int idUsuario = int.Parse(Session["idUsuarioLogueado"].ToString());
                if (idUsuario != id) return RedirectToAction("Index", "Home");
            }
            List<IndisponibilidadRecurrente> ir = new List<IndisponibilidadRecurrente>();
            List<IndisponibilidadUnica> iu = new List<IndisponibilidadUnica>();
            foreach (Indisponibilidad i in usuario.Indisponibilidades)
            {
                if(i.Usuario != null && i.Usuario.Activo) { 
                    if (i is IndisponibilidadRecurrente)
                    {
                        ir.Add((IndisponibilidadRecurrente)i);
                    }
                    else
                    {
                        iu.Add((IndisponibilidadUnica)i);
                    }
                }
            }
            ViewBag.ListaRecurrentes = ir;
            ViewBag.ListaUnicas = iu;
            return View();
        }


        // GET: Indisponibilidades/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indisponibilidad indisponibilidad = db.Indisponibilidads.Find(id);
            if (indisponibilidad == null)
            {
                return HttpNotFound();
            }
            return View(indisponibilidad);
        }

        // GET: Indisponibilidades/Create
        public ActionResult Create()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
            return View();
        }

        // POST: Indisponibilidades/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DateTime HoraInicio, DateTime HoraFin, string Analista)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                //db.Indisponibilidads.Add();
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public ActionResult CrearRecurrente(string diaSemana, string HoraInicio, string HoraFin, string IdUsuario)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                if (IdUsuario == null) IdUsuario = Session["idUsuarioLogueado"].ToString();
                Usuario usuario = db.DbUsuarios.Find(Int32.Parse(IdUsuario));
                if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
                {
                    int idUsuario = int.Parse(IdUsuario);
                    if (idUsuario != usuario.IdUsuario) return RedirectToAction("Index", "Home");
                }
                IndisponibilidadRecurrente indisponibilidad = new IndisponibilidadRecurrente(diaSemana, usuario, HoraInicio, HoraFin);
                usuario.Indisponibilidades.Add(indisponibilidad);
                db.Indisponibilidads.Add(indisponibilidad);
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
                    return RedirectToAction("IndisponibilidadesUsuario", new { id = usuario.IdUsuario });
                else
                    return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Indisponibilidades/EditarRecurrente
        public ActionResult EditarRecurrente(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indisponibilidad indisponibilidad = db.Indisponibilidads.Where(i => i.IdIndisponibilidad == id).Include("Usuario").FirstOrDefault();
            if (indisponibilidad == null)
            {
                return HttpNotFound();
            }
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
            {
                int idUsuario = int.Parse(Session["idUsuarioLogueado"].ToString());
                if (idUsuario != indisponibilidad.Usuario.IdUsuario) return RedirectToAction("Index", "Home");
            }
            IndisponibilidadRecurrente iR = (IndisponibilidadRecurrente)indisponibilidad;
            ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
            return View(iR);
        }


        [HttpPost]
        public ActionResult EditarRecurrente(int id, string diaSemana, string HoraInicio, string HoraFin)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                Indisponibilidad indisponibilidad = db.Indisponibilidads.Where(i => i.IdIndisponibilidad == id).Include("Usuario").FirstOrDefault();
                if (indisponibilidad == null || indisponibilidad.Usuario == null) return RedirectToAction("Index");
                int idUsuario = 0;
                if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
                {
                    idUsuario = int.Parse(Session["idUsuarioLogueado"].ToString());
                    if (idUsuario != indisponibilidad.Usuario.IdUsuario) return RedirectToAction("Index", "Home");
                }
                Usuario usuario = db.DbUsuarios.Find(indisponibilidad.Usuario.IdUsuario);
                IndisponibilidadRecurrente iR = (IndisponibilidadRecurrente)indisponibilidad;
                iR.DiaSemana = diaSemana;
                iR.HoraInicio = HoraInicio;
                iR.HoraFin = HoraFin;
                ModificarIndisponibilidadUsuario(usuario, iR);
                db.Entry(iR).State = EntityState.Modified;
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
                    return RedirectToAction("IndisponibilidadesUsuario", new { id = idUsuario });
                else
                    return RedirectToAction("Index");
            }
            return View();
        }

        private void ModificarIndisponibilidadUsuario(Usuario usuario, Indisponibilidad ind)
        {
            for (int i = 0; i < usuario.Indisponibilidades.Count(); i++)
            {
                if(usuario.Indisponibilidades[i].IdIndisponibilidad == ind.IdIndisponibilidad)
                {
                    usuario.Indisponibilidades[i] = ind;
                }
            } 
        }

        // GET: Indisponibilidades/EditarUnica
        public ActionResult EditarUnica(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indisponibilidad indisponibilidad = db.Indisponibilidads.Where(i => i.IdIndisponibilidad == id).Include("Usuario").FirstOrDefault();
            if (indisponibilidad == null)
            {
                return HttpNotFound();
            }
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
            {
                int idUsuario = int.Parse(Session["idUsuarioLogueado"].ToString());
                if (idUsuario != indisponibilidad.Usuario.IdUsuario) return RedirectToAction("Index", "Home");
            }
            IndisponibilidadUnica iU = (IndisponibilidadUnica)indisponibilidad;
            ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
            return View(iU);
        }


        [HttpPost]
        public ActionResult EditarUnica(int id, DateTime FechaInicio, DateTime FechaFin, string HoraInicio, string HoraFin)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                Indisponibilidad indisponibilidad = db.Indisponibilidads.Where(i => i.IdIndisponibilidad == id).Include("Usuario").FirstOrDefault();
                if (indisponibilidad == null || indisponibilidad.Usuario == null) return RedirectToAction("Index");
                int idUsuario = 0;
                if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
                {
                    idUsuario = int.Parse(Session["idUsuarioLogueado"].ToString());
                    if (idUsuario != indisponibilidad.Usuario.IdUsuario) return RedirectToAction("Index", "Home");
                }
                DateTime fechaI = new DateTime(FechaInicio.Year,
                    FechaInicio.Month,
                    FechaInicio.Day,
                    Int32.Parse(HoraInicio.Substring(0, 2)),
                    Int32.Parse(HoraInicio.Substring(HoraInicio.Length - 2, 2)), 0);
                DateTime fechaF = new DateTime(FechaFin.Year,
                  FechaFin.Month,
                  FechaFin.Day,
                  Int32.Parse(HoraFin.Substring(0, 2)),
                  Int32.Parse(HoraFin.Substring(HoraFin.Length - 2, 2)), 0);
                if (fechaF >= fechaI)
                {
                    if (indisponibilidad.Usuario == null) return RedirectToAction("Index");
                    Usuario usuario = db.DbUsuarios.Find(indisponibilidad.Usuario.IdUsuario);
                    IndisponibilidadUnica iU = (IndisponibilidadUnica)indisponibilidad;
                    iU.FechaInicio = fechaI;
                    iU.FechaFin = fechaF;
                    ModificarIndisponibilidadUsuario(usuario, iU);
                    db.Entry(iU).State = EntityState.Modified;
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();
                    if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
                        return RedirectToAction("IndisponibilidadesUsuario", new { id = idUsuario });
                   
                }
                else
                {
                    ViewBag.ErrorIndisponibilidad = "La fecha fin debe ser mayor a la fecha inicio.";
                    return RedirectToAction("Create");
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CrearUnica(DateTime FechaInicio, DateTime FechaFin, string HoraInicio, string HoraFin, string IdUsuario)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            
            if (ModelState.IsValid)
            {
                if (IdUsuario == null) IdUsuario = Session["idUsuarioLogueado"].ToString();
                Usuario usuario = db.DbUsuarios.Find(Int32.Parse(IdUsuario));
                if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
                {
                    int idUsuario = int.Parse(IdUsuario);
                    if (idUsuario != usuario.IdUsuario) return RedirectToAction("Index", "Home");
                }
                DateTime fechaI = new DateTime(FechaInicio.Year,
                    FechaInicio.Month,
                    FechaInicio.Day,
                    Int32.Parse(HoraInicio.Substring(0, 2)),
                    Int32.Parse(HoraInicio.Substring(HoraInicio.Length - 2, 2)), 0);
                DateTime fechaF = new DateTime(FechaFin.Year,
                  FechaFin.Month,
                  FechaFin.Day,
                  Int32.Parse(HoraFin.Substring(0, 2)),
                  Int32.Parse(HoraFin.Substring(HoraFin.Length - 2, 2)), 0);
                if(fechaF >= fechaI)
                {
                    IndisponibilidadUnica indisponibilidad = new IndisponibilidadUnica(fechaI, fechaF, usuario);
                    usuario.Indisponibilidades.Add(indisponibilidad);
                    db.Indisponibilidads.Add(indisponibilidad);
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();
                    if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista"))
                        return RedirectToAction("IndisponibilidadesUsuario", new { id = usuario.IdUsuario });
                    else
                        return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorIndisponibilidad = "La fecha fin debe ser mayor a la fecha inicio.";
                    return RedirectToAction("Create");
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Indisponibilidades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indisponibilidad indisponibilidad = db.Indisponibilidads.Find(id);
            if (indisponibilidad == null)
            {
                return HttpNotFound();
            }
            return View(indisponibilidad);
        }

        // POST: Indisponibilidades/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdIndisponibilidad,HoraInicio,HoraFin")] Indisponibilidad indisponibilidad)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(indisponibilidad).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(indisponibilidad);
        }

        // GET: Indisponibilidades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Indisponibilidad indisponibilidad = db.Indisponibilidads.Find(id);
            if (indisponibilidad == null)
            {
                return HttpNotFound();
            }
            return View(indisponibilidad);
        }

        // POST: Indisponibilidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Indisponibilidad indisponibilidad = db.Indisponibilidads.Find(id);
            db.Indisponibilidads.Remove(indisponibilidad);
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
