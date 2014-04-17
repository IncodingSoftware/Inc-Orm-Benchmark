namespace IncOrmBenchmark.Domain
{
    #region << Using >>

    using System;
    using System.Data.Entity.ModelConfiguration;
    using Incoding.Data;
    using Raven.Imports.Newtonsoft.Json;

    #endregion

    [JsonObject(IsReference = true, ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
    public class Order : IncEntityBase
    {
        #region Constructors

        public Order()
        {
            Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region Properties

        public new virtual string Id { get; set; }

        public virtual string Number { get; set; }

        public virtual Customer Customer { get; set; }

        #endregion

        #region Nested classes

        public class Map : NHibernateEntityMap<Order>
        {
            #region Constructors

            protected Map()
            {
                Id(r => r.Id).GeneratedBy.Assigned();
                MapEscaping(r => r.Number);
                DefaultReference(r => r.Customer);
            }

            #endregion
        }

        public class EFMap : EFClassMap<Order>
        {
            public override void OnModel(EntityTypeConfiguration<Order> entity)
            {
                entity.ToTable("Order_Tbl")
                      .HasKey(r => r.Id)
                      .Property(r => r.Id);

                entity.Property(r => r.Number)
                      .HasColumnName("Number_Col");
            }
        }

        #endregion
    }
}