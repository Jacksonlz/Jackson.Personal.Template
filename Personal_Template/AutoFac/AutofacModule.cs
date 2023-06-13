using Autofac;
using Personal.Infrastructure.IocLifeTime;

namespace Personal.Template.Web.Api.AutoFac
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Personal.Template.*.dll");
            foreach (var assemblyFile in assemblies)
            {
                var assembly = System.Reflection.Assembly.Load(System.Reflection.AssemblyName.GetAssemblyName(assemblyFile));
                builder.RegisterAssemblyTypes(assembly)
                .Where(t => !t.IsInterface && typeof(ISingleton).IsAssignableFrom(t))
                .AsImplementedInterfaces().SingleInstance();
                builder.RegisterAssemblyTypes(assembly)
                .Where(t => !t.IsInterface && typeof(IScope).IsAssignableFrom(t))
                .AsImplementedInterfaces().InstancePerLifetimeScope();
                builder.RegisterAssemblyTypes(assembly)
                .Where(t => !t.IsInterface && typeof(ITransient).IsAssignableFrom(t))
                .AsImplementedInterfaces().InstancePerDependency();
            }
            base.Load(builder);
        }
    }
}