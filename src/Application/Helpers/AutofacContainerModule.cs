using Autofac;
using Infrastructure.Contracts;
using Infrastructure.Repositories;
using Module = Autofac.Module;

namespace Application.Helpers
{
    public class AutofacContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(IAutoDependencyService).Assembly)
                .AssignableTo<IAutoDependencyService>()
                .As<IAutoDependencyService>()
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
