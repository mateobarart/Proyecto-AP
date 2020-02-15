using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class PartidosController : Controller
    {
        private Context db = new Context();

        // GET: Partidos
        public ActionResult Index()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            return View(db.DbPartidos.OrderBy(p => p.FechaPartido).ToList());
        }

        // GET: Partidos/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partido partido = db.DbPartidos.Find(id);
            if (partido == null)
            {
                return HttpNotFound();
            }
            return View(partido);
        }

        // GET: Partidos/Create
        public ActionResult Create()
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            ViewBag.ListaCompeticiones = db.DbCompeticiones.ToList();
            ViewBag.ListaEquipos = db.DbEquipos.ToList();
            ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
            return View();
        }

        // POST: Partidos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DateTime FechaPartido, string Hora, string Competicion, string Local, string Visitante)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                Equipo local = db.DbEquipos.Find(Int32.Parse(Local));
                Equipo visitante = db.DbEquipos.Find(Int32.Parse(Visitante));
                if (local.Equals(visitante))
                {
                    ModelState.AddModelError("CreatePartidoIncorrecto", "Los equipos no pueden ser iguales.");
                }else { 
                    Competicion competicion = db.DbCompeticiones.Find(Int32.Parse(Competicion));
                    DateTime fechaHora = new DateTime(FechaPartido.Year,
                        FechaPartido.Month,
                        FechaPartido.Day,
                        Int32.Parse(Hora.Split(':')[0]),
                        Int32.Parse(Hora.Split(':')[1]),0);
                    Partido partido = new Partido(fechaHora, Hora, competicion, local, visitante);
                    db.DbPartidos.Add(partido);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }


        // GET: Partidos/Create
        public ActionResult ReasignarAnalista(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partido partido = db.DbPartidos.Find(id);
            if (partido == null)
            {
                return HttpNotFound();
            }
            return View(partido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReasignarAnalista(int? id, string ReasignarPartido, string ReasignarLocal, string ReasignarVisitante)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                Partido partido = db.DbPartidos.Find(id);
                if(partido != null) { 
                    DateTime fecha = partido.FechaPartido;
                    string hora = partido.Hora;
                    Competicion competicion = partido.Competicion;
                    Equipo eLocal = partido.Local;
                    Equipo eVisitante = partido.Visitante;
                    Partido nuevoPartido = new Partido(fecha, hora, competicion, eLocal, eVisitante);
                    bool analistaLocal = false;
                    bool analistaVisitante = false;
                    if (ReasignarPartido == "Partido" && ReasignarLocal == null && ReasignarVisitante == null)
                    {
                        analistaLocal = true;
                        analistaVisitante = true;
                    }
                    if (ReasignarPartido == null && ReasignarLocal == "Analista Local" && ReasignarVisitante == null)
                    {
                        analistaLocal = true;
                        nuevoPartido.AnalistaVisitante = partido.AnalistaVisitante;
                    }
                    if (ReasignarPartido == null && ReasignarLocal == null && ReasignarVisitante == "Analista Visitante")
                    {
                        analistaVisitante = true;
                        nuevoPartido.AnalistaLocal = partido.AnalistaLocal;
                    }
                    AsignarPartido(nuevoPartido);
                    EliminarPartido(partido, analistaLocal, analistaVisitante);
                    
                }
            }
            return RedirectToAction("Index");
        }

        private void EliminarPartido(Partido partido, bool analistaLocal, bool analistaVisitante)
        {
            EliminarIndisponibilidades(partido, analistaLocal, analistaVisitante);
            db.DbPartidos.Remove(partido);
            db.SaveChanges();
        }

        private void EliminarIndisponibilidades(Partido partido, bool analistaLocal, bool analistaVisitante)
        {
            DateTime comienzoDia = new DateTime(partido.FechaPartido.Year, partido.FechaPartido.Month, partido.FechaPartido.Day, 0, 0, 0);
            DateTime finDia = new DateTime(partido.FechaPartido.Year, partido.FechaPartido.Month, partido.FechaPartido.Day, 23, 59, 0);
            DateTime fechaInicioLimite = partido.FechaPartido.AddMinutes(-30);
            DateTime fechaFinLimite = partido.FechaPartido.AddMinutes(120);
            if(analistaLocal)
            {
                EliminarIndisponibilidad(partido.AnalistaLocal, comienzoDia, finDia);
                EliminarIndisponibilidad(partido.AnalistaLocal, fechaInicioLimite, fechaFinLimite);
            }
            if (analistaVisitante)
            {
                EliminarIndisponibilidad(partido.AnalistaVisitante, comienzoDia, finDia);
                EliminarIndisponibilidad(partido.AnalistaVisitante, fechaInicioLimite, fechaFinLimite);
            }
        }

        private void EliminarIndisponibilidad(Usuario usuario, DateTime comienzo, DateTime fin)
        {
            if(usuario != null)
            {
                for (int i = usuario.Indisponibilidades.Count() - 1; i >= 0; i--)
                {
                    if(usuario.Indisponibilidades[i] is IndisponibilidadUnica)
                    {
                        IndisponibilidadUnica iu = (IndisponibilidadUnica)usuario.Indisponibilidades[i];
                        if(iu.FechaInicio == comienzo && iu.FechaFin == fin)
                        {
                            db.Indisponibilidads.Remove(usuario.Indisponibilidades[i]);
                            //usuario.Indisponibilidades.Remove(usuario.Indisponibilidades[i]);
                            break;
                        }
                    }
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CargarArchivoCSV(HttpPostedFileBase postedFile)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                try
                {
                    string filePath = string.Empty;
                    if (postedFile != null)
                    {
                        string path = Server.MapPath("~/Uploads/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        filePath = path + Path.GetFileName(postedFile.FileName);
                        string extension = Path.GetExtension(postedFile.FileName);
                        postedFile.SaveAs(filePath);
                        string csvData = System.IO.File.ReadAllText(filePath);

                        bool cancelar = false;
                        foreach (string row in csvData.Split('\n'))
                        {
                            if (!string.IsNullOrEmpty(row))
                            {
                                Partido partido = CrearPartido(row);
                                if (partido == null)
                                {
                                    cancelar = true;
                                    ModelState.AddModelError("CargaArchivo", "No se pudo cargar el archivo seleccionado.");
                                    break;
                                }
                                db.DbPartidos.Add(partido);
                            }
                        }
                        if (!cancelar)
                        {
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("CargaArchivo", "No se pudo cargar el archivo seleccionado.");
                }
            }
            return Redirect("Index");
        }

        private Partido CrearPartido(string row)
        {
            try
            {
                row = row.Split('\r')[0];
                string fecha = row.Split(';')[0];
                int dia = int.Parse(fecha.Split('/')[0]);
                int mes = int.Parse(fecha.Split('/')[1]);
                int anio = int.Parse(fecha.Split('/')[2]);


                string hora = row.Split(';')[1];

                DateTime dFecha = new DateTime(
                                                anio,
                                                mes,
                                                dia,
                                                int.Parse(hora.Split(':')[0]),
                                                int.Parse(hora.Split(':')[1]),
                                                0);
                string nombreCompeticion = row.Split(';')[2];
                Competicion comp = db.DbCompeticiones.Where(c => c.NombreCompeticion == nombreCompeticion).FirstOrDefault();

                string nombreEquipoLocal = row.Split(';')[3];
                Equipo equipoLocal = db.DbEquipos.Where(e => e.NombreEquipo == nombreEquipoLocal).FirstOrDefault();

                string nombreEquipoVisitante = row.Split(';')[4];
                Equipo equipoVisitante = db.DbEquipos.Where(e => e.NombreEquipo == nombreEquipoVisitante).FirstOrDefault();
                if (dFecha != null && hora != "" && comp != null && equipoLocal != null && equipoVisitante != null)
                {
                    return new Partido(dFecha, hora, comp, equipoLocal, equipoVisitante);
                }
                return null;
            }
            catch (Exception)
            {
                ModelState.AddModelError("CargaArchivo", "No se pudo cargar el archivo seleccionado.");
                return null;
            }
        }

        // POST: Partidos/FiltrarOAsignarPartidos/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FiltrarOAsignarPartidos(DateTime? fechaInicio, DateTime? fechaFin, string FiltrarButton, string AsignarButton)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if(FiltrarButton == "Filtrar" && AsignarButton == null)
            {
                return FiltrarPartidos(fechaInicio, fechaFin);
            }
            else if ((FiltrarButton == null && AsignarButton == "Asignar"))
            {
                return AsignarPartidos(fechaInicio, fechaFin);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        private ActionResult FiltrarPartidos(DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (fechaInicio == null && fechaFin == null)
            {
                return View("Index", db.DbPartidos.OrderBy(p => p.FechaPartido).ToList());
            }else if(fechaInicio == null && fechaFin != null)
            {
                DateTime fechaAuxI = (DateTime)fechaFin;
                ViewBag.FechaFIn = fechaAuxI.ToString("yyyy-MM-dd");
                fechaAuxI = fechaAuxI.AddDays(1);
                return View("Index", db.DbPartidos.Where(p => p.FechaPartido < fechaAuxI).OrderBy(p => p.FechaPartido).ToList());
            }else if (fechaInicio != null && fechaFin == null)
            {
                DateTime fechaAuxF = (DateTime)fechaInicio;
                ViewBag.FechaInicio = fechaAuxF.ToString("yyyy-MM-dd");
                return View("Index", db.DbPartidos.Where(p => p.FechaPartido >= fechaAuxF).OrderBy(p => p.FechaPartido).ToList());
            }
            DateTime fechaInicio2 = (DateTime)fechaInicio;
            DateTime fechaFin2 = (DateTime)fechaFin;
            ViewBag.FechaInicio = fechaInicio2.ToString("yyyy-MM-dd");
            ViewBag.FechaFIn = fechaFin2.ToString("yyyy-MM-dd");
            fechaFin2 = fechaFin2.AddDays(1);
            return View("Index", db.DbPartidos.Where(p => p.FechaPartido >= fechaInicio).Where(p => p.FechaPartido < fechaFin2).OrderBy(p => p.FechaPartido).ToList());

        }

        private ActionResult AsignarPartido(Partido partido)
        {
            if (partido == null)
            {
                return Redirect("Index");
            }
            List<Partido> partidos = new List<Partido>();
            partidos.Add(partido);
            RecorrerPartidos(partidos);
            db.DbPartidos.Add(partido);
            db.SaveChanges();
            return Redirect("Index");
        }

        private ActionResult AsignarPartidos(DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (fechaInicio == null || fechaFin == null)
            {
                return View("Index", db.DbPartidos.OrderBy(p => p.FechaPartido).ToList());
            }
            DateTime fechaInicio2 = (DateTime)fechaInicio;
            DateTime fechaFin2 = (DateTime)fechaFin;
            fechaFin2 = fechaFin2.AddDays(1);
            List<Partido> partidos = db.DbPartidos.Where(p => p.FechaPartido >= fechaInicio).Where(p => p.FechaPartido < fechaFin2).OrderBy(p => p.FechaPartido).ToList();
            RecorrerPartidos(partidos);
            db.SaveChanges();
            return View("Index", db.DbPartidos.Where(p => p.FechaPartido >= fechaInicio).Where(p => p.FechaPartido < fechaFin2).OrderBy(p => p.FechaPartido).ToList());
        }

        private void RecorrerPartidos(List<Partido> partidos)
        {
            if(BuscarAnalistas(partidos, "titular")) 
            {
                if(BuscarAnalistas(partidos, "suplente"))
                {
                    if(BuscarAnalistas(partidos, "reserva"))
                    {
                        BuscarAnalistasPorRanking(partidos);
                    }
                }
            }
        }

        private void BuscarAnalistasPorRanking(List<Partido> partidos)
        {
            List<Usuario> ranking = db.DbUsuarios.Where(usu => usu.TipoUsuario == TipoUsuario.Analista).Where(usu => usu.Activo == true).OrderBy(usu => usu.Puntaje).ToList();
            foreach (Partido p in partidos)
            {
                if (p.AnalistaLocal == null)
                {
                    foreach (Usuario usuario in ranking)
                    {
                        if (usuario != null)
                        {
                            if (AsignarUsuario(usuario, p, true)) break;
                        }
                    }
                }
                
                if(p.AnalistaVisitante == null)
                {
                    foreach (Usuario usuario in ranking)
                    {
                        if (usuario != null)
                        {
                            if (AsignarUsuario(usuario, p, false)) break;
                        }
                    }
                }

                   
            }
        }

        private bool BuscarAnalistas(List<Partido> partidos, string analista)
        {
            bool partidosPorAnalizar = false;
            foreach (Partido p in partidos)
            {
                Usuario u = BuscarUsuarioAAnalizar(p, analista, true);
                if (p.AnalistaLocal == null && u != null)
                {
                    AsignarUsuario(u, p, true);
                }

                u = BuscarUsuarioAAnalizar(p, analista, false);
                if (p.AnalistaVisitante == null && u != null)
                {
                    AsignarUsuario(u, p, false);
                }

                if (!partidosPorAnalizar && (p.AnalistaLocal == null || p.AnalistaVisitante == null))
                {
                    partidosPorAnalizar = true;
                }
            }
            return partidosPorAnalizar;
        }

        private Usuario BuscarUsuarioAAnalizar(Partido p, string analista, bool local)
        {   Usuario u = null;
            switch (analista)
            {
                case "titular":
                    if (local) { u = p.Local.Titular; }
                    else { u = p.Visitante.Titular; }
                    break;
                case "suplente":
                    if (local) { u = p.Local.Suplente; }
                    else { u = p.Visitante.Suplente; }
                    break;
                case "reserva":
                    if (local) { u = p.Local.Reserva; }
                    else { u = p.Visitante.Reserva; }
                    break;
                default:
                    break;
            }
            return u;
        }

        private Boolean AsignarUsuario(Usuario u, Partido p, bool local)
        {
            if (VerificarDisponibilidad(u, p.FechaPartido))
            {
                DateTime fechaInicioLimite = p.FechaPartido.AddMinutes(-30);
                DateTime fechaFinLimite = p.FechaPartido.AddMinutes(120);
                if (local)
                {
                    p.AnalistaLocal = u;
                }
                else
                {
                    p.AnalistaVisitante = u;
                }
                Indisponibilidad ind = new IndisponibilidadUnica(fechaInicioLimite, fechaFinLimite, u);
                u.Indisponibilidades.Add(ind);
                db.Indisponibilidads.Add(ind);
                VerificarCantidadPartidos(u, p.FechaPartido);
                //db.Entry(p).State = EntityState.Modified;
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        private void VerificarCantidadPartidos(Usuario u, DateTime fechaPartido)
        {
            List<Partido> partidosLocal = db.DbPartidos.Where(p => p.AnalistaLocal.IdUsuario == u.IdUsuario)
                .Where(p => p.FechaPartido.Year == fechaPartido.Year)
                .Where (p => p.FechaPartido.Month == fechaPartido.Month)
                .Where(p => p.FechaPartido.Day == fechaPartido.Day).ToList();
            List<Partido> partidosVisitante = db.DbPartidos.Where(p => p.AnalistaVisitante.IdUsuario == u.IdUsuario)
                .Where(p => p.FechaPartido.Year == fechaPartido.Year)
                .Where(p => p.FechaPartido.Month == fechaPartido.Month)
                .Where(p => p.FechaPartido.Day == fechaPartido.Day).ToList();
            if(partidosLocal.Count() + partidosVisitante.Count() == 2)
            {
                DateTime fechaInicio = new DateTime(fechaPartido.Year, fechaPartido.Month, fechaPartido.Day, 0,0,0);
                DateTime fechaFin = new DateTime(fechaPartido.Year, fechaPartido.Month, fechaPartido.Day, 23, 59, 0);
                Indisponibilidad ind = new IndisponibilidadUnica(fechaInicio, fechaFin, u);
                u.Indisponibilidades.Add(ind);
                db.Indisponibilidads.Add(ind);
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        private bool VerificarDisponibilidad(Usuario u, DateTime fechaPartido)
        {
            DateTime fechaInicioLimite = fechaPartido.AddMinutes(-30);
            DateTime fechaFinLimite = fechaPartido.AddMinutes(120);
            foreach (Indisponibilidad ind in u.Indisponibilidades)
            {
                if (ind is IndisponibilidadRecurrente)
                {
                    IndisponibilidadRecurrente iR = (IndisponibilidadRecurrente)ind;
                    bool sumarDia = false;
                    bool restarDia = false;
                    int horaI = int.Parse(iR.HoraInicio.Split(':')[0]);
                    int minI = int.Parse(iR.HoraInicio.Split(':')[1]);
                    int horaF = int.Parse(iR.HoraFin.Split(':')[0]);
                    int minF = int.Parse(iR.HoraFin.Split(':')[1]);
                    if (!VerificarDiaSemana(iR, ObtenerDiaSemana(iR.DiaSemana, ""), fechaPartido, sumarDia, restarDia))
                    {
                        return false;
                    }
                    if (fechaPartido.Hour == 0 && fechaPartido.Minute < 30 && horaF == 23 && minF >= 30)
                    {
                        restarDia = true;
                        if (!VerificarDiaSemana(iR, ObtenerDiaSemana(iR.DiaSemana, "siguiente"), fechaPartido, sumarDia, restarDia))
                        {
                            return false;
                        }
                    }
                    if (fechaPartido.Hour >= 22 && horaI < 2)
                    {
                        sumarDia = true;
                        if (!VerificarDiaSemana(iR, ObtenerDiaSemana(iR.DiaSemana, "anterior"), fechaPartido, sumarDia, restarDia))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    IndisponibilidadUnica iU = (IndisponibilidadUnica)ind;
                    if ((iU.FechaInicio < fechaFinLimite && iU.FechaInicio >= fechaInicioLimite) ||
                    (iU.FechaFin <= fechaFinLimite && iU.FechaFin > fechaInicioLimite))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool VerificarDiaSemana(IndisponibilidadRecurrente iR, DayOfWeek diaSemana,DateTime fechaPartido, bool sumarDia, bool restarDia)
        {
            DateTime fechaInicioLimite = fechaPartido.AddMinutes(-30);
            DateTime fechaFinLimite = fechaPartido.AddMinutes(120);
            if (fechaPartido.DayOfWeek == diaSemana)
            {
                int horaI = int.Parse(iR.HoraInicio.Split(':')[0]);
                int minI = int.Parse(iR.HoraInicio.Split(':')[1]);
                int horaF = int.Parse(iR.HoraFin.Split(':')[0]);
                int minF = int.Parse(iR.HoraFin.Split(':')[1]);
                DateTime fechaInicio = new DateTime(2020, 1, 5, horaI, minI, 0);
                DateTime fechaFin = new DateTime(2020, 1, 5, horaF, minF, 0);
                DateTime fechaInicioLimiteAux = new DateTime(2020, 1, 5, fechaInicioLimite.Hour, fechaInicioLimite.Minute, 0);
                DateTime fechaFinLimiteAux = new DateTime(2020, 1, 5, fechaFinLimite.Hour, fechaFinLimite.Minute, 0);
                if (sumarDia)
                {
                    fechaInicio = fechaInicio.AddDays(1);
                    fechaFin = fechaFin.AddDays(1);
                    fechaFinLimiteAux = fechaFinLimiteAux.AddDays(1);
                }
                if (restarDia){
                    fechaInicio = fechaInicio.AddDays(-1);
                    fechaFin = fechaFin.AddDays(-1);
                    fechaInicioLimiteAux = fechaInicioLimiteAux.AddDays(-1);
                }
                if ((fechaInicio < fechaFinLimiteAux && fechaInicio >= fechaInicioLimiteAux) ||
                (fechaFin <= fechaFinLimiteAux && fechaFin > fechaInicioLimiteAux))
                {
                    return false;
                }
            }
            return true;
        }

        private DayOfWeek ObtenerDiaSemana(string diaSemana, string opcion)
        {
            if (opcion == "anterior")
            {
                switch (diaSemana)
                {
                    case "Lunes":
                        return DayOfWeek.Sunday;
                    case "Martes":
                        return DayOfWeek.Monday;
                    case "Miércoles":
                        return DayOfWeek.Tuesday;
                    case "Jueves":
                        return DayOfWeek.Wednesday;
                    case "Viernes":
                        return DayOfWeek.Thursday;
                    case "Sábado":
                        return DayOfWeek.Friday;
                    case "Domingo":
                        return DayOfWeek.Saturday;
                    default:
                        return DayOfWeek.Sunday;
                }
            }
            else if (opcion == "siguiente")
            {
                switch (diaSemana)
                {
                    case "Lunes":
                        return DayOfWeek.Tuesday;
                    case "Martes":
                        return DayOfWeek.Wednesday;
                    case "Miércoles":
                        return DayOfWeek.Thursday;
                    case "Jueves":
                        return DayOfWeek.Friday;
                    case "Viernes":
                        return DayOfWeek.Saturday;
                    case "Sábado":
                        return DayOfWeek.Sunday;
                    case "Domingo":
                        return DayOfWeek.Monday;
                    default:
                        return DayOfWeek.Sunday;
                }
            }else { 
                switch (diaSemana)
                {
                    case "Lunes":
                        return DayOfWeek.Monday;
                    case "Martes":
                        return DayOfWeek.Tuesday;
                    case "Miércoles":
                        return DayOfWeek.Wednesday;
                    case "Jueves":
                        return DayOfWeek.Thursday;
                    case "Viernes":
                        return DayOfWeek.Friday;
                    case "Sábado":
                        return DayOfWeek.Saturday;
                    case "Domingo":
                        return DayOfWeek.Sunday;
                    default:
                        return DayOfWeek.Sunday;
                }
            }
        }

        // GET: Partidos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partido partido = db.DbPartidos.Find(id);
            if (partido == null)
            {
                return HttpNotFound();
            }
            ViewBag.ListaCompeticiones = db.DbCompeticiones.ToList();
            ViewBag.ListaEquipos = db.DbEquipos.ToList();
            ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).Where(x => x.Activo == true).ToList();
            return View(partido);
        }

        // POST: Partidos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DateTime FechaPartido, string Hora, string Competicion, string Local, string Visitante, string idPartido)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            Partido partido = db.DbPartidos.Find(Int32.Parse(idPartido));
            if (ModelState.IsValid)
            {

                if (!Local.Equals(Visitante))
                {
                    partido.Competicion = db.DbCompeticiones.Find(Int32.Parse(Competicion));
                    partido.Local = db.DbEquipos.Find(Int32.Parse(Local));
                    partido.Visitante = db.DbEquipos.Find(Int32.Parse(Visitante));
                    partido.FechaPartido = new DateTime(FechaPartido.Year,
                                                        FechaPartido.Month,
                                                        FechaPartido.Day,
                                                        Int32.Parse(Hora.Split(':')[0]),
                                                        Int32.Parse(Hora.Split(':')[1]), 0);
                    partido.Hora = Hora;
                    db.Entry(partido).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ListaCompeticiones = db.DbCompeticiones.ToList();
                    ViewBag.ListaEquipos = db.DbEquipos.ToList();
                    ViewBag.ListaAnalistas = db.DbUsuarios.Where(x => x.TipoUsuario == TipoUsuario.Analista).ToList();
                    ModelState.AddModelError("EditIncorrecto", "Los equipos no pueden ser iguales.");
                }
               
            }
            return View(partido);
        }

        // GET: Partidos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partido partido = db.DbPartidos.Find(id);
            if (partido == null)
            {
                return HttpNotFound();
            }
            return View(partido);
        }

        // POST: Partidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["mailUsuarioLogueado"] == null) return RedirectToAction("Login", "Account");
            if (Session["tipoUsuarioLogueado"].ToString().Equals("Analista")) return RedirectToAction("Index", "Home");
            Partido partido = db.DbPartidos.Find(id);
            EliminarPartido(partido, true, true);
           
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
