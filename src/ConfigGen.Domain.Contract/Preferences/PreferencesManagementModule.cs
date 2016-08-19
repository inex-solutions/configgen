using Autofac;

namespace ConfigGen.Domain.Contract.Preferences
{
    public class PreferencesManagementModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreferencesManager>().As<IPreferencesManager>().SingleInstance();
        }
    }
}