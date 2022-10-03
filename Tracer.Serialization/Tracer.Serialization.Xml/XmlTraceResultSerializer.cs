using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Tracer.Core;
using Tracer.Serialization.Abstractions;

namespace Tracer.Serialization.Xml
{
    public class XmlTraceResultSerializer : ITraceResultSerializer
    {
        public string Format { get; } = "Xml";
        
        public void Serialize(TraceResult traceResult, Stream to)
        {
            var ser = new DataContractSerializer(typeof(TraceResult));
            using (XmlWriter xw = XmlWriter.Create(to))
            {
                ser.WriteObject(xw, traceResult);
            }
        }
    }
}