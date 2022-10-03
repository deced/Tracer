using System;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tracer.Core.Tests
{
    [TestClass]
    public class UnitTest
    {
        public void Method1(Tracer tracer)
        {
            tracer.StartTrace();
            Thread.Sleep(100);
            Method2(tracer);
            Method3(tracer);
            tracer.StopTrace();
        }

        public void Method2(Tracer tracer)
        {
            tracer.StartTrace();
            Thread.Sleep(100);
            tracer.StopTrace();
        }

        public void Method3(Tracer tracer)
        {
            tracer.StartTrace();
            Thread.Sleep(100);
            Method4(tracer);
            tracer.StopTrace();
        }

        public void Method4(Tracer tracer)
        {
            tracer.StartTrace();
            Thread.Sleep(100);
            tracer.StopTrace();
        }

        private void Method200Ms(Tracer tracer)
        {
            tracer.StartTrace();
            Thread.Sleep(200);
            tracer.StopTrace();
        }

        private void Method1000Ms(Tracer tracer)
        {
            tracer.StartTrace();
            Thread.Sleep(1000);
            tracer.StopTrace();
        }

        private void EmptyMethod(Tracer tracer)
        {
            tracer.StartTrace();
            tracer.StopTrace();
        }

        [TestMethod]
        public void TestEmptyMethod()
        {
            var tracer = new Tracer();
            EmptyMethod(tracer);
            var result = tracer.GetTraceResult();
            Assert.AreEqual(1, result.ThreadTraceResult.Count);
            Assert.IsTrue( result.ThreadTraceResult[0].Time < 10);
            Assert.AreEqual(1, result.ThreadTraceResult[0].MethodTraceResults.Count);
            Assert.AreEqual("EmptyMethod", result.ThreadTraceResult[0].MethodTraceResults[0].Name);
            Assert.AreEqual("UnitTest", result.ThreadTraceResult[0].MethodTraceResults[0].ClassName);
            Assert.IsTrue(result.ThreadTraceResult[0].MethodTraceResults[0].ExecutionTime < 10);
        }

        [TestMethod]
        public void Test1000MsMethod()
        {
            var tracer = new Tracer();
            Method1000Ms(tracer);
            var result = tracer.GetTraceResult();
            Assert.AreEqual(1, result.ThreadTraceResult.Count);
            Assert.IsTrue(result.ThreadTraceResult[0].Time >= 1000 && result.ThreadTraceResult[0].Time <= 1010);
            Assert.AreEqual(1, result.ThreadTraceResult[0].MethodTraceResults.Count);
            Assert.AreEqual("Method1000Ms", result.ThreadTraceResult[0].MethodTraceResults[0].Name);
            Assert.AreEqual("UnitTest", result.ThreadTraceResult[0].MethodTraceResults[0].ClassName);
            Assert.IsTrue(result.ThreadTraceResult[0].MethodTraceResults[0].ExecutionTime >= 1000 &&
                          result.ThreadTraceResult[0].MethodTraceResults[0].ExecutionTime <= 1010);
        }

        [TestMethod]
        public void TestTwoSeparateMethods()
        {
            var tracer = new Tracer();
            Method1000Ms(tracer);
            Method200Ms(tracer);
            var result = tracer.GetTraceResult();
            Assert.AreEqual(1, result.ThreadTraceResult.Count);
            Assert.IsTrue(result.ThreadTraceResult[0].Time >= 1200 && result.ThreadTraceResult[0].Time <= 1210);
            Assert.AreEqual(2, result.ThreadTraceResult[0].MethodTraceResults.Count);
            Assert.IsNotNull(result.ThreadTraceResult[0].MethodTraceResults
                .FirstOrDefault(x => x.Name == "Method1000Ms"));
            Assert.IsNotNull(
                result.ThreadTraceResult[0].MethodTraceResults.FirstOrDefault(x => x.Name == "Method200Ms"));
            Assert.IsTrue(result.ThreadTraceResult[0].MethodTraceResults.FirstOrDefault(x => x.Name == "Method1000Ms")
                              .ExecutionTime >= 1000 &&
                          result.ThreadTraceResult[0].MethodTraceResults.FirstOrDefault(x => x.Name == "Method1000Ms")
                              .ExecutionTime <= 1010);
            Assert.IsTrue(result.ThreadTraceResult[0].MethodTraceResults.FirstOrDefault(x => x.Name == "Method200Ms")
                              .ExecutionTime >= 200 &&
                          result.ThreadTraceResult[0].MethodTraceResults.FirstOrDefault(x => x.Name == "Method200Ms")
                              .ExecutionTime <= 210);
        }

        [TestMethod]
        public void TestMethodsTree()
        {
            var tracer = new Tracer();
            Method1(tracer);
            var result = tracer.GetTraceResult();
            Assert.AreEqual(1, result.ThreadTraceResult.Count);
            Assert.IsTrue(result.ThreadTraceResult[0].Time >= 400 && result.ThreadTraceResult[0].Time <= 410);
            Assert.AreEqual(1, result.ThreadTraceResult[0].MethodTraceResults.Count);
            var method1 = result.ThreadTraceResult[0].MethodTraceResults[0];
            Assert.AreEqual("Method1", method1.Name);
            Assert.IsNotNull(method1.MethodTraceResults.FirstOrDefault(x => x.Name == "Method2"));
            Assert.IsNotNull(method1.MethodTraceResults.FirstOrDefault(x => x.Name == "Method3"));
            var method3 = method1.MethodTraceResults.FirstOrDefault(x => x.Name == "Method3");
            Assert.AreEqual("Method4", method3.MethodTraceResults[0].Name);
        }

        [TestMethod]
        public void TestMultiThreading()
        {
            var tracer = new Tracer();
            Method1(tracer);
            int a = 0;
            new Thread(() =>
            {
                Method1(tracer);
            }).Start();
            Thread.Sleep(1000);
            var result = tracer.GetTraceResult();
            Assert.AreEqual(2, result.ThreadTraceResult.Count);
            Assert.AreNotEqual(result.ThreadTraceResult[0].Id, result.ThreadTraceResult[1].Id);
            Assert.IsTrue(Math.Abs(result.ThreadTraceResult[0].Time - result.ThreadTraceResult[1].Time) < 10);
        }
    }
}