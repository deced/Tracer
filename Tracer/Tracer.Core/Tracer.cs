using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Tracer.Core
{
    public class Tracer : ITracer
    {
        private List<SingleTraceResult> _singleTraceResults;
        private Stack<Stopwatch> _stopwatches;

        public Tracer()
        {
            _singleTraceResults = new List<SingleTraceResult>();
            _stopwatches = new Stack<Stopwatch>();
        }

        public void StartTrace()
        {
            var stopwatch = new Stopwatch();
                    lock (_stopwatches)
            {
                _stopwatches.Push(stopwatch);
                stopwatch.Start();
            }
        }

        public void StopTrace()
        {
            StackTrace stackTrace = new StackTrace();
            var stopwatch = _stopwatches.Pop();
            stopwatch.Stop();
            var singleTraceResult = new SingleTraceResult()
            {
                MethodName = stackTrace.GetFrame(1)?.GetMethod()?.Name,
                ParentMethodName = stackTrace.GetFrame(2)?.GetMethod()?.Name,
                ClassName = stackTrace.GetFrame(1)?.GetMethod()?.ReflectedType?.Name,
                ExecutionTime = stopwatch.ElapsedMilliseconds,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };
            _singleTraceResults.Add(singleTraceResult);
        }

        public TraceResult GetTraceResult()
        {
            var traceResult = new TraceResult();
            for (int i = _singleTraceResults.Count - 1; i >= 0; i--)
            {
                traceResult.AddTrace(_singleTraceResults[i]);
            }

            return traceResult;
        }
    }
}