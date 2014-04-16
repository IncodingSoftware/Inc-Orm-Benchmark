namespace IncOrmBenchmark.Domain
{
    #region << Using >>

    using System;
    using System.Diagnostics;
    using System.Linq;
    using Incoding.Block.IoC;
    using Incoding.CQRS;
    using Incoding.Extensions;

    #endregion

    internal class Program
    {
        #region Constants

        const int item = 1000;

        #endregion

        static void Run(Func<IMessage<object>> message, string key, int repeat, string label)
        {
            var dispatcher = IoCFactory.Instance.TryResolve<IDispatcher>();
            var stopwatch = new Stopwatch();
            stopwatch.Restart();
            for (int i = 0; i < repeat; i++)
            {
                dispatcher.Push(composite => composite.Quote(message())
                                                      .WithDateBaseString(key));
            }

            stopwatch.Stop();
            decimal second = (decimal)stopwatch.ElapsedMilliseconds / 1000;
            Console.WriteLine("{0} performance for {1} by {2} item : {3}".F(key, label, item, Math.Round(second, 5)));
        }

        static void Main(string[] args)
        {
            Bootstrapper.Start();

            foreach (var key in new[]
                                    {
                                            Bootstrapper.nhKey, 
                                            Bootstrapper.entityFramework, 
                                            Bootstrapper.ravenKey, 
                                            Bootstrapper.mongoDb
                                    })
            {
                Run(message: () => new AddCustomerCommand { Name = "Any name" }, 
                    repeat: 1000, 
                    key: key, 
                    label: "insert on isolated transaction");
                Run(message: () => new BatchCustomerCommand { Name = "Any name", Count = item }, 
                    repeat: 1, 
                    key: key, 
                    label: "insert batch on shared transaction");
                string id = IoCFactory.Instance.TryResolve<IDispatcher>()
                                      .Query(new GetEntitiesQuery<Customer>(), new MessageExecuteSetting
                                                                                   {
                                                                                           DataBaseInstance = key
                                                                                   })
                                      .First().Id;
                Run(message: () => new UpdateCustomerCommand
                                       {
                                               Id = id, 
                                               Name = "New Name", 
                                               IsTracking = key != Bootstrapper.mongoDb
                                       }, 
                    repeat: 1000, 
                    key: key, 
                    label: "update on isolated transaction");
                Run(message: () => new AddOrderCommand
                                       {
                                               CustomerId = id
                                       }, 
                    repeat: 1000, 
                    key: key, 
                    label: "insert with reference on isolated transaction");
                Run(message: () => new GetCustomerByIdQuery
                                       {
                                               Id = id
                                       }, 
                    repeat: 1000, 
                    key: key, 
                    label: "get by id on isolated transaction");
                Run(message: () => new GetCustomerQuery
                                       {
                                               Name = "Any name", 
                                               FetchSize = item
                                       }, 
                    repeat: 1, 
                    key: key, 
                    label: "read");
                Run(message: () => new DeleteAllCommand(), 
                    repeat: 1, 
                    key: key, 
                    label: "delete all");
                Run(message: () => new EmptyCommand(), 
                    repeat: 1000, 
                    key: key, 
                    label: "empty");
                Console.WriteLine();
            }

            Console.Read();
            Console.WriteLine("Benchmark finished");
        }
    }
}