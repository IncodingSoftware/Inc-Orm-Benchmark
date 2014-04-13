namespace IncOrmBenchmark.Domain
{
    #region << Using >>

    using System;
    using System.Data.Entity.ModelConfiguration;
    using Incoding.Data;

    #endregion

    public class Customer : IncEntityBase
    {
        #region Constructors

        public Customer()
        {
            Id = Guid.NewGuid().ToString();
        }

        #endregion

        #region Properties

        public new virtual string Id { get; set; }

        public virtual string Name { get; set; }

        #endregion

        #region Nested classes

        public class Map : NHibernateEntityMap<Customer>
        {
            #region Constructors

            protected Map()
            {
                Id(r => r.Id).GeneratedBy.Assigned();
                MapEscaping(r => r.Name);
            }

            #endregion
        }

        public class EFMap : EFClassMap<Customer>
        {
            public override void OnModel(EntityTypeConfiguration<Customer> entity)
            {
                entity.ToTable("Customer_Tbl")
                      .HasKey(r => r.Id)
                      .Property(r => r.Id);

                entity.Property(r => r.Name)
                      .HasColumnName("Name_Col");
            }
        }

        #endregion
    }
}