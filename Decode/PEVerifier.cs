using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
//using Microsoft.Build.Utilities;
using Mono.Cecil;

namespace Emit.Test
{
    public class PEVerifier
    {
        /// <summary>
        /// Use ILSpy's decompiler to analyse the specified methods.
        /// 
        /// http://ilspy.net/
        /// </summary>
        /// <param name="methods"></param>
        public static void VerifyMethods(IEnumerable<MethodDefinition> methods)
        {
            foreach (var method in methods)
            {
                var astBuilder =
                    new AstBuilder(new DecompilerContext(method.Module) { CurrentType = method.DeclaringType });
                astBuilder.AddMethod(method);
            }
        }

        /// <summary>
        /// Method to call PEVerify.exe on the specified assebmly and process any errors propagating them
        /// as exception.
        /// 
        /// Field not visible errors (error code 0x8013187D) are ignored since this tool is run on 
        /// </summary>
        /// <param name="modulePath"></param>
        //public static void VerifyPEFile(string modulePath)
        //{
        //    var process = StartPEVerifyProcess(modulePath);
        //    var output = process.StandardOutput.ReadToEnd();
        //    process.WaitForExit();
        //    if (process.ExitCode != 0)
        //    {
        //        var msg = string.Format("PEVerify exit code: {0}\n{1}", process.ExitCode, output);
        //        throw new InvalidOperationException(msg);
        //    }
        //}

        //private static Process StartPEVerifyProcess(string modulePath)
        //{
        //    // ignore field not visible errors (error code 0x8013187D)
        //    var verifierPath = Path.Combine(
        //        ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.Version40),
        //        @"bin\NETFX 4.0 Tools\peverify.exe");
        //    var args = string.Format("\"{0}\" /verbose /nologo /hresult /ignore=0x8013187D", modulePath);
        //    var process = new Process
        //                      {
        //                          StartInfo =
        //                              {
        //                                  CreateNoWindow = true,
        //                                  FileName = verifierPath,
        //                                  RedirectStandardOutput = true,
        //                                  UseShellExecute = false,
        //                                  WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
        //                                  Arguments = args
        //                              }
        //                      };
        //    process.Start();
        //    return process;
        //}
    }
}