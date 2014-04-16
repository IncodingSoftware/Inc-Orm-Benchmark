namespace IncOrmBenchmark.Domain
{
    #region << Using >>

    using Incoding.CQRS;

    #endregion

    public class DeleteAllCommand : CommandBase
    {
        #region Properties

        public string Name { get; set; }

        #endregion

        public override void Execute()
        {
            Repository.DeleteAll<Order>();
            Repository.DeleteAll<Customer>();
        }
    }
}