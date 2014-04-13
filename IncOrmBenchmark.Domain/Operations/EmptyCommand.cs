namespace IncOrmBenchmark.Domain
{
    using Incoding.CQRS;

    public class EmptyCommand : CommandBase
    {
        public override void Execute() { }
    }
}