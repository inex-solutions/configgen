using System.IO;
using System.Reflection;

namespace ConfigGen.Utilities.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Returns a stream for the specified resource file.
        /// </summary>
        /// <param name="assembly">Assembly from which to get resource</param>
        /// <param name="resourcePath">Resource path of file, not including the assembly name; e.g. for resource MyDomain.MyAssembly.MyFolder.MyResource 
        /// in assembly MyDomain.MyAssembly, resource path is MyFolder.MyResource</param>
        /// <returns>Stream</returns>
        public static Stream GetEmbeddedResourceFileStream(this Assembly assembly, string resourcePath)
        {
            var resourceName = assembly.FullName.Split(',')[0] + "." + resourcePath;
            return assembly.GetManifestResourceStream(resourceName);
        }
    }
}