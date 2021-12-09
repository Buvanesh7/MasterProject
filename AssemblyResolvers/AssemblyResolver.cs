using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.AssemblyResolvers
{
    public class AssemblyResolver
    {

        private readonly Dictionary<string, string> _files = new Dictionary<string, string>();
        private readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();

        public AssemblyResolver()
        {
            var installDir = Path.GetDirectoryName(GetType().Assembly.Location);
            var dlls = Directory.GetFiles(installDir, "*.dll", SearchOption.AllDirectories);
            var exes = Directory.GetFiles(installDir, "*.exe", SearchOption.AllDirectories);
            var files = new List<string>(dlls);
            files.AddRange(exes);

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                if (!_files.ContainsKey(name))
                    _files.Add(name, file);
            }
        }

        public void Register()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public void Unregister()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        public Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Split(',').First();
            if (name.EndsWith(".resources"))
                return null;

            if (_loadedAssemblies.TryGetValue(args.Name, out var loadedAssembly))
            {
                return loadedAssembly;
            }


            if (!_files.TryGetValue(name, out var assemblyPath))
            {
                return null;
            }


            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                _loadedAssemblies.Add(assembly.FullName, assembly);

                if (IsSameMajor(assembly.FullName, args.Name))
                {
                    return assembly;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool IsSameMajor(string name1, string name2)
        {
            try
            {
                var majorString1 = name1.Split('=')[1].Split(',')[0].Split('.')[0];
                var majorString2 = name2.Split('=')[1].Split(',')[0].Split('.')[0];
                var major1 = int.Parse(majorString1);
                var major2 = int.Parse(majorString2);
                return major1 == major2;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}


