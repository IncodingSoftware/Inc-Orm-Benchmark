namespace IncOrmBenchmark.Domain
{
    #region << Using >>

    using Incoding.CQRS;

    #endregion

    public class BatchCustomerCommand : CommandBase
    {
        #region Properties

        public string Name { get; set; }

        public int Count { get; set; }

        #endregion

        public override void Execute()
        {
            for (int i = 0; i < Count; i++)
            {
                Repository.Save(new Customer
                                    {
                                            Name = Name
                                    });
            }
        }
    }
}