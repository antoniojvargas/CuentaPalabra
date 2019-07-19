namespace CuentaPalabra
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Literatura.ContenidoEntradas")]
    public partial class ContenidoEntradas
    {
        public Guid Id { get; set; }

        public Guid IdEntrada { get; set; }

        [Required]
        public string Contenido { get; set; }

        public virtual Entradas Entradas { get; set; }
    }
}
