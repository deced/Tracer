namespace Tracer.Core
{
    public class SingleTraceResult
    {
        public string ParentMethodName { get; set; }
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        
        public int ThreadId { get; set; }
        public long ExecutionTime { get; set; }
    }
}