using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tracer.Serialization.Abstractions;

namespace Tracer.Example
{
    public static class PluginHelper
    {

        public static List<ITraceResultSerializer> GetSerializers()
        {
            var serializers = new List<ITraceResultSerializer>();
            var files = Directory.GetFiles("./", "*.dll");
            foreach (var file in files)
            {
                var asm = Assembly.LoadFrom(file);
                var types = asm.GetTypes().
                    Where(t => t.GetInterfaces().Any(i => i.FullName == typeof(ITraceResultSerializer).FullName));
                foreach (var type in types)
                {
                    var obj = (ITraceResultSerializer)Activator.CreateInstance(type);
                    serializers.Add(obj);
                }
                
            }

            return serializers;
        }
    }
}