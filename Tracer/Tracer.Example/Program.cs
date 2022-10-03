using System;
using System.IO;
using System.Threading.Tasks;

namespace Tracer.Example
{
    class Program
    {

        public static long GetFactorial(int val)
        {
            long result = 1;
            for (int i = 1; i < val; i++)
            {
                result *= val;
            }

            return result;
        }

        static void Main(string[] args)
        {
            var serializers =  PluginHelper.GetSerializers();
            var tracer = new Core.Tracer();
            MyClass myClass = new MyClass(tracer);
            Task.Run(() =>
            {
                var asyncClass = new MyAsyncClass(tracer);
                asyncClass.Method1();
                asyncClass.FastMethod();
                GetFactorial(5);
            });
            myClass.FastMethod();
            myClass.Method1();
            GetFactorial(5);
            var result = tracer.GetTraceResult();

            foreach (var serializer in serializers)
            {
                var fs = new FileStream($"data.{serializer.Format}", FileMode.Create);
                serializer.Serialize(result,fs);
                fs.Close();
            }
        }
    }
}