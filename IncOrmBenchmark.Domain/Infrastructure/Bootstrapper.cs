namespace IncOrmBenchmark.Domain
{
    #region << Using >>

    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.SqlServer;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using Incoding.Block.IoC;
    using Incoding.CQRS;
    using Incoding.Data;
    using Incoding.EventBroker;
    using Incoding.MvcContrib;
    using MongoDB.Bson.Serialization;
    using NHibernate.Context;
    using Raven.Client.Document;

    #endregion

    public static class Bootstrapper
    {
        #region Constants

        public const string nhKey = "nhibernate";

        public const string ravenKey = "ravendb";

        public const string entityFramework = "ef";

        public const string mongoDb = "mongoDb";

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
                                                                                                         registry.For<INhibernateSessionFactory>().Singleton().Use(() => new NhibernateSessionFactory(configure));
                                                                                                         registry.For<IUnitOfWorkFactory>().Singleton().Use<NhibernateUnitOfWorkFactory>().Named(nhKey);
                                                                                                         registry.For<IRepository>().Use<NhibernateRepository>().Named(nhKey);

                                                                                                         registry.For<IRavenDbSessionFactory>().Use(() => new RavenDbSessionFactory(new DocumentStore
                                                                                                                                                                                        {
                                                                                                                                                                                                Url = "http://localhost:8080", 
                                                                                                                                                                                                DefaultDatabase = "IncOrmBenchmark"
                                                                                                                                                                                        }));
                                                                                                         registry.For<IUnitOfWorkFactory>().Singleton().Use<RavenDbUnitOfWorkFactory>().Named(ravenKey);
                                                                                                         registry.For<IRepository>().Use<RavenDbRepository>().Named(ravenKey);
                                                                                                         
                                                                                                         registry.For<IEntityFrameworkSessionFactory>().Use(() => new EntityFrameworkSessionFactory(() =>
                                                                                                                                                                                                        {
                                                                                                                                                                                                            DbContext incDbContext = new IncDbContext("Main", typeof(Bootstrapper).Assembly);
                                                                                                                                                                                                            incDbContext.Configuration.ValidateOnSaveEnabled = false;
                                                                                                                                                                                                            return incDbContext;
                                                                                                                                                                                                        }));
                                                                                                         registry.For<IUnitOfWorkFactory>().Singleton().Use<EntityFrameworkUnitOfWorkFactory>().Named(entityFramework);
                                                                                                         registry.For<IRepository>().Use<EntityFrameworkRepository>().Named(entityFramework);

                                                                                                         BsonClassMap.RegisterClassMap<IncEntityBase>(map => map.UnmapProperty(r => r.Id));
                                                                                                         registry.For<IMongoDbSessionFactory>().Use(() => new MongoDbSessionFactory("mongodb://localhost:27017/benchmark"));
                                                                                                         registry.For<IUnitOfWorkFactory>().Singleton().Use<MongoDbUnitOfWorkFactory>().Named(mongoDb);
                                                                                                         registry.For<IUnitOfWork>().Singleton().Use<MongoDbUnitOfWork>().Named(mongoDb);
                                                                                                         registry.For<IRepository>().Use<MongoDbRepository>().Named(mongoDb);
                                                                                                     })));

            var nhManagerDb = IoCFactory.Instance.TryResolve<IManagerDataBase>();
            if (!nhManagerDb.IsExist())
            {
                nhManagerDb.Drop();
                nhManagerDb.Create();
            }
        }

        #endregion
    }
}