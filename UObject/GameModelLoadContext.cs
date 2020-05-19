using System;
using System.Reflection;
using System.Runtime.Loader;
using JetBrains.Annotations;

namespace UObject
{
    [PublicAPI]
    public class GameModelLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver Resolver;

        public GameModelLoadContext(string mainAssemblyToLoadPath) : base(true) => Resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);

        public static Lazy<GameModelLoadContext> Instance { get; } = new Lazy<GameModelLoadContext>();

        protected override Assembly? Load(AssemblyName name)
        {
            var assemblyPath = Resolver.ResolveAssemblyToPath(name);
            return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
        }
    }
}
