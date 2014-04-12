namespace IncOrmBenchmark.Domain
{
    using Incoding.CQRS;

    public class DeleteAllCustomerCommand : CommandBase
    {
        #region Properties

        public string Name { get; set; }

        #endregion

        public override void Execute()
        {
            this.Repository.DeleteAll<Customer>();
        }
    }
}