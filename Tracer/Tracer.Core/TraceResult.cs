using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Tracer.Core
{
    [DataContract]
    public class TraceResult
    {
        [DataMember(Name="ThreadTraceResult")]
        private List<ThreadTraceResult> _threadTraceResults;

        public IReadOnlyList<ThreadTraceResult> ThreadTraceResult { get => _threadTraceResults; }

        public TraceResult()
        {
            _threadTraceResults = new List<ThreadTraceResult>();
        }

        public void AddTrace(SingleTraceResult traceResult)
        {
            var methodTraceResult = new MethodTraceResult()
            {
                Name = traceResult.MethodName,
                ClassName = traceResult.ClassName,
                ExecutionTime = traceResult.ExecutionTime
            };

            ThreadTraceResult threadTraceResult = _threadTraceResults.FirstOrDefault(x => x.Id == traceResult.ThreadId);

            if (threadTraceResult == null)
            {
                threadTraceResult = new ThreadTraceResult() { Id = traceResult.ThreadId };
                _threadTraceResults.Add(threadTraceResult);
            }


            var parentMethod =
                GetParentMethodTraceResult(threadTraceResult.MethodTraceResults, traceResult.ParentMethodName);
            if (parentMethod == null)
            {
                threadTraceResult.MethodTraceResults.Add(methodTraceResult);
                threadTraceResult.Time += traceResult.ExecutionTime;
            }
            else
                parentMethod.MethodTraceResults.Add(methodTraceResult);
        }

        private MethodTraceResult GetParentMethodTraceResult(List<MethodTraceResult> methodTraceResults,
            string parentName)
        {
            var parent = methodTraceResults.FirstOrDefault(x => x.Name == parentName);
            if (parent != null)
                return parent;
            foreach (var traceResult in methodTraceResults)
            {
                parent = GetParentMethodTraceResult(traceResult.MethodTraceResults, parentName);
                if (parent != null)
                    return parent;
            }

            return null;
        }
    }

    public class ThreadTraceResult
    {
        public int Id { get; set; }
        public long Time { get; set; }
        public List<MethodTraceResult> MethodTraceResults { get; set; }

        public ThreadTraceResult()
        {
            MethodTraceResults = new List<MethodTraceResult>();
        }
    }

    public class MethodTraceResult
    {
        public long ExecutionTime { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public List<MethodTraceResult> MethodTraceResults { get; set; }

        public MethodTraceResult()
        {
            MethodTraceResults = new List<MethodTraceResult>();
        }

        public override string ToString()
        {
            return $"{ClassName}.{Name} {ExecutionTime}ms";
        }
    }
}