using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
//using Xunit;

namespace Emit.Test
{
    /// <summary>
    /// Collection of tests to check the validity of the emmited dynamic methods generated at runtime.
    /// </summary>
    public class Serializer
    {
        //[Fact]
        public void CheckWithLSpy()
        {
            // create the dynamic methods and store them in a DLL 1 per 
            // object and to run the IL analysis.
            var dllPath = Path.Combine(Environment.CurrentDirectory, "$_test.dll");
            TestTargets.CreateDLL(dllPath);
            
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(dllPath, new ReaderParameters(ReadingMode.Immediate));
            var typeDef = assemblyDefinition.MainModule.GetType(string.Empty, "EmitClass");
            var emitCode = typeDef.Methods.First(m => m.Name == "EmitCode");

            PEVerifier.VerifyMethods(new[] { emitCode });
        }

        //[Fact]
        public void CheckWithPEVerify()
        {
            var dllPath = Path.Combine(Environment.CurrentDirectory, "$_test.dll");
            TestTargets.CreateDLL(dllPath);
            
            //PEVerifier.VerifyPEFile(dllPath);
        }
    }
}