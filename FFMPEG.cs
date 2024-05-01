using System;
using Wasm.Interpret;
using Wasm;
using System.IO;
using FFMpeg.wasm.Properties;
using System.Collections.Generic;
using System.Linq;
namespace FFMpeg.wasm
{

    public struct FFMPEGLog
    {
        public string type;
        public string message;
    }
    public class FFMPEG
    {
        WasmFile Wasm { get; set; }

        ModuleInstance Module {  get; set; }

        public FFMPEG()
        {
            PredefinedImporter importer = new PredefinedImporter();
            Wasm = WasmFile.ReadBinary(new MemoryStream(Resources.ffmpeg_core));
            #region Importer
            //defino todos los metodos del ffmpeg aqui
            importer.DefineFunction("load", new DelegateFunctionDefinition(
                new WasmValueType[] { },
                new WasmValueType[] { },
                load

            ));

            importer.DefineFunction("exit", new DelegateFunctionDefinition(
                new WasmValueType[] { },
                new WasmValueType[] { },
                exit
            ));


            importer.DefineFunction("FS", new DelegateFunctionDefinition(

                new WasmValueType[] { },//string,string,data?
                new WasmValueType[] { },
                FS


            ));

            importer.DefineFunction("run", new DelegateFunctionDefinition(

                new WasmValueType[] { },//string
                new WasmValueType[] { },
                run


            ));
            importer.DefineFunction("setlogger", new DelegateFunctionDefinition(

               new WasmValueType[] { },//delegate out -> {type:string,message:string} (FFMPEGLog)
               new WasmValueType[] { },
               setlogger


           ));
            #endregion

            Module = ModuleInstance.Instantiate(Wasm, importer);
        }
        #region Importer
        private IReadOnlyList<object> setlogger(IReadOnlyList<object> list)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyList<object> run(IReadOnlyList<object> list)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyList<object> FS(IReadOnlyList<object> list)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyList<object> exit(IReadOnlyList<object> list)
        {
            throw new NotImplementedException();
        }

        private IReadOnlyList<object> load(IReadOnlyList<object> list)
        {
            throw new NotImplementedException();
        }
        #endregion


        public void WriteFile(string fileName, byte[] data)
        {
            FunctionDefinition funcDef = Module.ExportedFunctions[nameof(FS)];
            funcDef.Invoke(new object[] { "writeFile",fileName,data });
        }

        public byte[] ReadFile(string fileName)
        {
            FunctionDefinition funcDef = Module.ExportedFunctions[nameof(FS)];
            IReadOnlyList<object> results = funcDef.Invoke(new object[] { "readFile", fileName });
            if (results.Count == 0)
            {
                throw new Exception("file not found");
            }
            return (byte[])results[0];
        }

        public void RemoveFile(string fileName)
        {
            FunctionDefinition funcDef = Module.ExportedFunctions[nameof(FS)];
            funcDef.Invoke(new object[] { "unlink", fileName });
        }

        public void Load()
        {
            FunctionDefinition funcDef = Module.ExportedFunctions[nameof(load)];
            funcDef.Invoke(new object[] { });
        }

        public void Exit()
        {
            FunctionDefinition funcDef = Module.ExportedFunctions[nameof(exit)];
            funcDef.Invoke(new object[] { });
        }

        public void Run(params string[] commands)
        {
            string command = string.Join(" ", commands);
            FunctionDefinition funcDef = Module.ExportedFunctions[nameof(run)];
            funcDef.Invoke(new object[] { command });
        }



    }
}
