namespace CuentaPalabra
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GSTest_RAE : DbContext
    {
        public GSTest_RAE()
            : base("name=GSTest_RAE")
        {
        }

        public virtual DbSet<ContenidoEntradas> ContenidoEntradas { get; set; }
        public virtual DbSet<Entradas> Entradas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entradas>()
                .HasMany(e => e.ContenidoEntradas)
                .WithRequired(e => e.Entradas)
                .HasForeignKey(e => e.IdEntrada)
                .WillCascadeOnDelete(false);
        }
    }
}
