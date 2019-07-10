using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TeileListe.Common.Interface;

namespace TeileListe.Classes
{
    internal static class PluginManager
    {
        public static IExportManager ExportManager;
        public static IDbManager DbManager;

        public static string Version { get { return "v1.05"; } }

        internal static bool InitPlugins()
        {
            var files = new List<String>(Directory.GetFiles(Directory.GetCurrentDirectory(), "TeileListe.*.dll"));

            foreach (var dll in files)
            {
                try
                {
                    if (!dll.EndsWith("TeileListe.Common.dll"))
                    {
                        var assembly = Assembly.LoadFile(dll);
                        var types = assembly.GetTypes();
                        foreach (var cls in types)
                        {
                            if (cls.IsPublic)
                            {
                                var interfaces = cls.GetInterfaces();
                                foreach (var iface in interfaces)
                                {
                                    if (cls.GetInterface(iface.FullName) == typeof(IExportManager))
                                    {
                                        if (ExportManager == null)
                                        {
                                            ExportManager = assembly.CreateInstance(cls.FullName) as IExportManager;

                                            if(ExportManager.InterfaceVersion != Version)
                                            {
                                                ExportManager.Dispose();
                                                ExportManager = null;
                                            }
                                        }
                                    }

                                    if (cls.GetInterface(iface.FullName) == typeof(IDbManager))
                                    {
                                        if (DbManager == null)
                                        {
                                            DbManager = assembly.CreateInstance(cls.FullName) as IDbManager;
                                            if (DbManager.InterfaceVersion != Version)
                                            {
                                                DbManager.Dispose();
                                                DbManager = null;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return DbManager != null && ExportManager != null;
        }

        public static void CleanUp()
        {
            if (ExportManager != null)
            {
                ExportManager.Dispose();
            }

            if (DbManager != null)
            {
                DbManager.Dispose();
            }
        }
    }
}
