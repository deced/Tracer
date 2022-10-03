using System;
using System.Threading;
using Tracer.Core;

namespace Tracer.Example
{
    public class MyAsyncClass
    {
        private readonly ITracer _tracer;

        public MyAsyncClass(Tracer.Core.Tracer tracer)
        {
            _tracer = tracer;
        }

        public void FastMethod()
        {
            _tracer.StartTrace();
            int x = 0;
            for (int i = 0; i < 100; i++)
                x += i;
            _tracer.StopTrace();
        }

        public void Method1()
        {
            _tracer.StartTrace();
            Thread.Sleep(123);
            Method2();
            Method3();
            _tracer.StopTrace();
        }

        private void Method2()
        {
            _tracer.StartTrace();
            Thread.Sleep(321);
            Method4();
            _tracer.StopTrace();
        }

        private void Method3()
        {
            _tracer.StartTrace();
            Thread.Sleep(4000);
            _tracer.StopTrace();
        }

        private void Method4()
        {
            _tracer.StartTrace();
            long b = 0;
            for (int i = 0; i < Int32.MaxValue; i++)
                b = i - 1;
            _tracer.StopTrace();
        }
    }
}