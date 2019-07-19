using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CuentaPalabra.Models
{
    public class Palabra
    {
        public string NomPalabra { get; set; }

        public int Conteo { get; set; }

        public string PorcUso { get; set; }
    }
}