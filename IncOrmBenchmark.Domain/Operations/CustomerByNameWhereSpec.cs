namespace IncOrmBenchmark.Domain
{
    using System;
    using System.Linq.Expressions;
    using Incoding;

    public class CustomerByNameWhereSpec : Specification<Customer>
    {
        #region Fields

        readonly string name;

        #endregion

        #region Constructors

        public CustomerByNameWhereSpec(string name)
        {
            this.name = name;
        }

        #endregion

        public override Expression<Func<Customer, bool>> IsSatisfiedBy()
        {
            return r => r.Name == this.name;
        }
    }
}