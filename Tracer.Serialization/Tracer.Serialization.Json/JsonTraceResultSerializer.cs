using System;
using System.IO;
using System.Text.Json;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Json
{
    public class JsonTraceResultSerializer : ITraceResultSerializer
    {
        public string Format { get; } = "Json";
        public void Serialize(TraceResult traceResult, Stream to)
        {
            to.Write(JsonSerializer.SerializeToUtf8Bytes(traceResult,new JsonSerializerOptions(){WriteIndented = true}));
        }
    }
}