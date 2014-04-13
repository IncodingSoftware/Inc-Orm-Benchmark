namespace IncOrmBenchmark.Domain
{
    using Incoding.CQRS;

    public class GetCustomerByIdQuery : QueryBase<Customer>
    {
        #region Properties

        public string Id { get; set; }

        #endregion

        protected override Customer ExecuteResult()
        {
            return this.Repository.GetById<Customer>(this.Id);
        }
    }
}