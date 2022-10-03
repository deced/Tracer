using System;
using System.IO;
using Tracer.Core;
using Tracer.Serialization.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tracer.Serialization.Yaml
{
    public class YamlTraceResultSerializer : ITraceResultSerializer
    {
        public string Format { get; } = "Yaml";

        public void Serialize(TraceResult traceResult, Stream to)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(traceResult);
            StreamWriter sw = new StreamWriter(to);
            sw.Write(yaml);
            sw.Close();
        }
    }
}