using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CuentaPalabra.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Translate.v2;
using Google.Cloud.Translation.V2;
using OfficeOpenXml;

namespace CuentaPalabra.Controllers
{
    [Authorize]
    public class EntradasController : Controller
    {
        private GSTest_RAE db = new GSTest_RAE();

        public static GoogleCredential _cred;
        public static string _exePath;

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Analizar(FormCollection fc)
        {
            _exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Replace(@"file:\", "");
            using (var stream = new FileStream(_exePath + "\\CuentaPalabra-da877b259beb.json", FileMode.Open, FileAccess.Read))
            {
                _cred = GoogleCredential.FromStream(stream);
            }
            if (_cred.IsCreateScopedRequired)
            {
                _cred = _cred.CreateScoped("https://www.googleapis.com/auth/cloud-platform");
            }

            TranslationClient client = TranslationClient.Create(_cred);
            List<Palabra> palabras = new List<Palabra>();

            string id = fc["listEntradas"];
            var opcion = fc["boton"];

            Guid idEntrada = new Guid(id);
            
            var contenido = db.ContenidoEntradas.Where(u => u.IdEntrada == idEntrada).Select(u => u.Contenido).SingleOrDefault();
            
            string lasPalabras = contenido.ToString();
            string[] todasPalabras = lasPalabras.Split(' ');
            int totalPalabras = todasPalabras.Length;

            foreach (var unaPlabra in todasPalabras)
            {                
                var plabraLimpia = RemoveSpecialChars(unaPlabra);
                var palabraActual = palabras.Find(x => x.NomPalabra == plabraLimpia);
                //var trad = client.TranslateText(plabraLimpia, "en");
                //string traduccion = trad.TranslatedText;
                if (palabraActual == null)
                {
                    palabras.Add(new Palabra
                    {
                        NomPalabra = plabraLimpia,
                        Conteo = 1,
                        PorcUso = "",
                        //Traduccion = traduccion
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

            if(opcion == "Exportar")
            {
                DownloadExcel(palabras);
                return new EmptyResult();
            }

            return View("Analizar", palabras);

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

        public void DownloadExcel(List<Palabra> palabras)
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Palabra";
            Sheet.Cells["B1"].Value = "Conteo";
            Sheet.Cells["C1"].Value = "% de Uso";
            //Sheet.Cells["D1"].Value = "Traducción";

            int row = 2;
            foreach (var item in palabras)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.NomPalabra;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Conteo;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.PorcUso;
                //Sheet.Cells[string.Format("D{0}", row)].Value = item.Traduccion;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Analizar.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            
        }
    }
}
