namespace Orchard.Environment.Extensions.Loaders
{
    public interface IExtensionLoader
    {
        ExtensionEntry Load(IExtensionInfo extensionInfo);
    }
}
