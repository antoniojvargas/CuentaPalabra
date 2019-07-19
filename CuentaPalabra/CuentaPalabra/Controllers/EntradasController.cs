using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CuentaPalabra.Models;

namespace CuentaPalabra.Controllers
{
    [Authorize]
    public class EntradasController : Controller
    {
        private GSTest_RAE db = new GSTest_RAE();

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Analizar(FormCollection fc)
        {
            List<Palabra> palabras = new List<Palabra>();

            string id = fc["listEntradas"];

            Guid idEntrada = new Guid(id);
            
            var contenido = db.ContenidoEntradas.Where(u => u.IdEntrada == idEntrada).Select(u => u.Contenido).SingleOrDefault();
            
            string lasPalabras = contenido.ToString();
            string[] todasPalabras = lasPalabras.Split(' ');
            int totalPalabras = todasPalabras.Length;

            foreach (var unaPlabra in todasPalabras)
            {                
                var plabraLimpia = RemoveSpecialChars(unaPlabra);
                var palabraActual = palabras.Find(x => x.NomPalabra == plabraLimpia);
                if(palabraActual == null)
                {
                    palabras.Add(new Palabra
                    {
                        NomPalabra = plabraLimpia,
                        Conteo = 1,
                        PorcUso = ""
                    });
                }
                else
                {
                    palabraActual.Conteo = palabraActual.Conteo + 1;
                }
            }

            foreach(var cadaPalabra in palabras)
            {
                double porc = (cadaPalabra.Conteo * 100.0) / totalPalabras;
                porc = Math.Round(porc, 2);
                cadaPalabra.PorcUso = "% " + porc.ToString();
            }

            var lasentradas = from e in db.Entradas orderby e.Titulo select e;
            var entradas = new List<CuentaPalabra.ViewModels.Entradas>();
            foreach (var unaentrada in lasentradas)
            {
                entradas.Add(new ViewModels.Entradas
                {
                    Id = unaentrada.Id,
                    Titulo = unaentrada.Titulo,
                    Autor = unaentrada.Autor
                });
            }

            ViewBag.Entradas = entradas;

            return View("Analizar",palabras);
        }
        // GET: Entradas
        public ActionResult Index()
        {
            var lasentradas = from e in db.Entradas orderby e.Titulo select e;
            var entradas = new List<CuentaPalabra.ViewModels.Entradas>();
            foreach(var unaentrada in lasentradas)
            {
                entradas.Add(new ViewModels.Entradas {
                    Id = unaentrada.Id,
                    Titulo = unaentrada.Titulo,
                    Autor = unaentrada.Autor
                });
            }

            ViewBag.Entradas = entradas;            

            return View();
        }

        // GET: Entradas/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Entradas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Entradas/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Entradas/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Entradas/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Entradas/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Entradas/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public string RemoveSpecialChars(string input)
        {
            return Regex.Replace(input, @"[¿!¡;,:\.\?#@()<>]", string.Empty);
        }
    }
}
