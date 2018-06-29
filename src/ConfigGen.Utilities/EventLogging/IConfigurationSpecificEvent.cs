namespace ConfigGen.Utilities.EventLogging
{
    public interface IConfigurationSpecificEvent : IEvent
    {
        int ConfigurationIndex { get; }
    }
}