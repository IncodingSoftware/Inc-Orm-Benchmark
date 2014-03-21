namespace IncOrmBenchmark.Domain
{
    #region << Using >>

    using System.Configuration;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using Incoding.Block.IoC;
    using Incoding.CQRS;
    using Incoding.Data;
    using Incoding.EventBroker;
    using Incoding.MvcContrib;
    using NHibernate;
    using NHibernate.Context;
    using Raven.Client;
    using Raven.Client.Document;

    #endregion

    public static class Bootstrapper
    {
        #region Constants

        public const string nhKey = "nhibernate";

        public const string ravenKey = "ravendb";

        #endregion

        #region Factory constructors

        public static void Start()
        {
            IoCFactory.Instance.Initialize(init => init.WithProvider(new StructureMapIoCProvider(registry =>
                                                                                                     {
                                                                                                         registry.For<IDispatcher>().Singleton().Use<DefaultDispatcher>();
                                                                                                         registry.For<IEventBroker>().Singleton().Use<DefaultEventBroker>();
                                                                                                         registry.For<ITemplateFactory>().Singleton().Use<TemplateHandlebarsFactory>();

                                                                                                         var configure = Fluently
                                                                                                                 .Configure()
                                                                                                                 .Database(MsSqlConfiguration.MsSql2008.ConnectionString(ConfigurationManager.ConnectionStrings["Main"].ConnectionString))
                                                                                                                 .Mappings(configuration => configuration.FluentMappings.AddFromAssembly(typeof(Bootstrapper).Assembly))
                                                                                                                 .CurrentSessionContext<ThreadStaticSessionContext>();
                                                                                                         registry.For<IManagerDataBase>().Singleton().Use(() => new NhibernateManagerDataBase(configure));
                                                                                                         registry.For<ISessionFactory<ISession>>().Singleton().Use(() => new NhibernateSessionFactory(configure));
                                                                                                         registry.For<IUnitOfWorkFactory>().Singleton().Use<NhibernateUnitOfWorkFactory>().Named(nhKey);
                                                                                                         registry.For<IRepository>().Use<NhibernateRepository>().Named(nhKey);

                                                                                                         registry.For<ISessionFactory<IDocumentSession>>().Use(() => new RavenDbSessionFactory(new DocumentStore
                                                                                                                                                                                                   {
                                                                                                                                                                                                           Url = "http://localhost:8080",
                                                                                                                                                                                                           DefaultDatabase = "IncTest"
                                                                                                                                                                                                   }));

                                                                                                         registry.For<IUnitOfWorkFactory>().Singleton().Use<RavenDbUnitOfWorkFactory>().Named(ravenKey);
                                                                                                         registry.For<IRepository>().Use<RavenDbRepository>().Named(ravenKey);
                                                                                                     })));
        }

        #endregion
    }
}