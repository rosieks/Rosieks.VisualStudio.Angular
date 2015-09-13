namespace Rosieks.VisualStudio.Angular.Services
{
    internal class AppHierarchyFactory
    {
        public static AppHierarchy Find(string path)
        {
            var rootPath = path.Substring(0, path.LastIndexOf("app"));

            return new AppHierarchy(rootPath);
        }
    }
}
