namespace IncOrmBenchmark.Domain
{
    #region << Using >>

    using Incoding.CQRS;

    #endregion

    public class AddCustomerCommand : CommandBase
    {
        #region Properties

        public string Name { get; set; }

        #endregion

        public override void Execute()
        {
            Repository.Save(new Customer
                                {
                                        Name = Name
                                });
        }
    }
}