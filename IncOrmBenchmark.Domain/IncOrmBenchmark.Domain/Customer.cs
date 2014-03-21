namespace IncOrmBenchmark.Domain
{
    using Incoding.Data;

    public class Customer : IncEntityBase
    {
        public virtual new string Id { get; set; }

        public virtual string Name { get; set; }

        public class Map : NHibernateEntityMap<Customer>
        {
            protected Map()
            {
                IdGenerateByGuid(r => r.Id);
                MapEscaping(r => r.Name);
            }
        }
    }
}