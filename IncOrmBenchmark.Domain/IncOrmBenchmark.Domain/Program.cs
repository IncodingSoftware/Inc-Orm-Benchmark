namespace IncOrmBenchmark.Domain
{
    #region << Using >>

    using Incoding.Block.IoC;
    using Incoding.CQRS;

    #endregion

    public class DeleteAllCustomerByNhCommand : CommandBase
    {
        #region Properties

        public string Name { get; set; }

        #endregion

        public override void Execute()
        {
            Repository.ExecuteSql("Delete Customer_Tbl");
        }
    } 

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

    internal class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Start();
            var dispatcher = IoCFactory.Instance.TryResolve<IDispatcher>();
            dispatcher.Push(new DeleteAllCustomerByNhCommand(),new MessageExecuteSetting()
                                                                   {
                                                                       DataBaseInstance = Bootstrapper.nhKey
                                                                   });
                        
            dispatcher.Push(composite =>
                                {
                                    AddCustomerCommand nhCommand = new AddCustomerCommand { Name = "Nhibernate" };
                                    composite.Quote(nhCommand, new MessageExecuteSetting()
                                                                   {
                                                                           DataBaseInstance = Bootstrapper.nhKey,
                                                                           Commit =  true
                                                                   });

                                    AddCustomerCommand ravenCommand = new AddCustomerCommand { Name = "Raven" };
                                    composite.Quote(ravenCommand, new MessageExecuteSetting() { DataBaseInstance = Bootstrapper.ravenKey });
                                });
            
        }
    }
}