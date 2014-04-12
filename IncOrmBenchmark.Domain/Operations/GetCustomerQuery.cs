namespace IncOrmBenchmark.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using Incoding.CQRS;
    using Incoding.Data;

    public class GetCustomerQuery : QueryBase<List<Customer>>
    {
        #region Properties

        public int FetchSize { get; set; }

        public string Name { get; set; }

        #endregion

        protected override List<Customer> ExecuteResult()
        {
            return this.Repository.Query(paginatedSpecification: new PaginatedSpecification(0, this.FetchSize), 
                                    whereSpecification: new CustomerByNameWhereSpec(this.Name))
                             .ToList();
        }
    }
}