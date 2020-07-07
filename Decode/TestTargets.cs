using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace Emit.Test
{
    public class TestTargets
    {
        private int _pos;
        
        public int Pos { get { return _pos++; } }

        public static void CreateDLL(string dllPath)
        {
            var dllFile = Path.GetFileName(dllPath);

            // var dllDir = Path.GetDirectoryName(dllPath);
            var assemblyName = new AssemblyName { Name = Path.GetFileNameWithoutExtension(dllFile) };
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                assemblyName, AssemblyBuilderAccess.Save);

            var myModuleBldr = assemblyBuilder.DefineDynamicModule(dllFile, dllFile);

            var myTypeBldr = myModuleBldr.DefineType("EmitClass");
            var methodBldr = myTypeBldr.DefineMethod(
                "EmitCode",
                MethodAttributes.Public | MethodAttributes.Static,
                typeof(void),
                new[] { typeof(TestTargets) });
            var serIL = methodBldr.GetILGenerator();
            EmitCode(serIL);

            myTypeBldr.CreateType();

            assemblyBuilder.Save(dllFile);
        }

        private static void EmitCode(ILGenerator il)
        {
            var count = il.DeclareLocal(typeof(int));
            var dictionary = il.DeclareLocal(typeof(Dictionary<int, int>));
            var ctor = typeof(Dictionary<int, int>).GetConstructor(new Type[0]);
            il.Emit(OpCodes.Ldc_I4_5);
            il.Emit(OpCodes.Stloc, count);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Stloc, dictionary);
            
            var whileHead = il.DefineLabel();
            var whileBody = il.DefineLabel();
            il.Emit(OpCodes.Br_S, whileHead);
            
            il.MarkLabel(whileBody);
            {
                var method = typeof(TestTargets).GetMethod("DoSomething");
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, method);
                // TODO: Find the minimum set of code where PEVerify shows Ok, but AstBuilder shows the error.
                // il.Emit(OpCodes.Pop); 

                il.Emit(OpCodes.Ldloc, dictionary);
                il.Emit(OpCodes.Ldloc, count);
                il.Emit(OpCodes.Ldc_I4_0);
                var add = typeof(Dictionary<int, int>).GetMethod("Add", new[] { typeof(int), typeof(int) });
                il.Emit(OpCodes.Callvirt, add);
            }

            il.MarkLabel(whileHead);
            il.Emit(OpCodes.Ldarg_0);
            var pos = typeof(TestTargets).GetProperty("Pos").GetGetMethod();
            il.Emit(OpCodes.Callvirt, pos);
            il.Emit(OpCodes.Ldloc, count);
            il.Emit(OpCodes.Blt_S, whileBody);

            il.Emit(OpCodes.Ret);
        }

        protected void TestCode()
        {
            var count = 5;
            var dictionary = new Dictionary<int, int>();
            while (Pos < count)
            {
                DoSomething();
                dictionary.Add(count, 0);
                count--;
            }
        }

        public int DoSomething()
        {
            return 0;
        }
    }
}
