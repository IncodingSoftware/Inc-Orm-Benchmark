namespace IncOrmBenchmark.Domain
{
    using System;
    using Incoding.CQRS;

    public class AddOrderCommand : CommandBase
    {
        public string CustomerId { get; set; }

        public override void Execute()
        {
            Repository.Save(new Order
                                {
                                        Number = Guid.NewGuid().ToString(),
                                        Customer = Repository.GetById<Customer>(CustomerId)
                                });
        }
    }
}