namespace IncOrmBenchmark.Domain
{
    using Incoding.CQRS;

    public class UpdateCustomerCommand : CommandBase
    {
        #region Properties

        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsTracking { get; set; }

        #endregion

        public override void Execute()
        {
            var customer = this.Repository.GetById<Customer>(this.Id);
            customer.Name = this.Name;
            if (!this.IsTracking)
                this.Repository.SaveOrUpdate(customer);
        }
    }
}