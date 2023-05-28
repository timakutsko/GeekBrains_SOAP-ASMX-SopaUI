using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PumpService
{
    public class ScriptService : IScriptService
    {
        private CompilerResults _results;
        
        private readonly IStatisticsService _statisticsService;

        private readonly ISettingService _settingService;

        private readonly IPumpServiceCallback _pumpServiceCallback;

        public ScriptService(IStatisticsService statisticsService, ISettingService settingService, IPumpServiceCallback pumpServiceCallback)
        {
            _statisticsService = statisticsService;
            _settingService = settingService;
            _pumpServiceCallback = pumpServiceCallback;
        }

        public bool Compile()
        {
            try
            {
                CompilerParameters compilerParameters = new CompilerParameters();
                compilerParameters.GenerateInMemory = true;
                compilerParameters.ReferencedAssemblies.Add("System.dll");
                compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
                compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
                compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
                compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

                FileStream fileStream = new FileStream(_settingService.FileName, FileMode.Open);
                byte[] buffer;
                try
                {
                    int lenght = (int)fileStream.Length;
                    buffer = new byte[lenght];
                    int count;
                    int sum = 0;

                    while ((count = fileStream.Read(buffer, sum, lenght - sum)) > 0)
                        sum += count;
                }
                finally
                {
                    fileStream.Close();
                }

                CSharpCodeProvider provider = new CSharpCodeProvider();
                _results = provider.CompileAssemblyFromSource(compilerParameters, System.Text.Encoding.UTF8.GetString(buffer));
                if (_results.Errors != null && _results.Errors.Count > 0)
                {
                    string compileErrors = string.Empty;
                    for (int i = 0; i < _results.Errors.Count; i++)
                    {
                        if (compileErrors != string.Empty)
                            compileErrors += "\n";
                        compileErrors += _results.Errors[i];
                    }

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Run(int count)
        {
            if (_results == null || (_results != null && _results.Errors != null && _results.Errors.Count > 0))
            {
                if (Compile() == false)
                    return;
            }

            Type type = _results.CompiledAssembly.GetType("Sample.SampleScript");
            if (type == null)
                return;

            MethodInfo entryPointMethod = type.GetMethod("EntryPoint");
            if (entryPointMethod == null)
                return;

            Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    if ((bool)entryPointMethod.Invoke(Activator.CreateInstance(type), null))
                        _statisticsService.SuccessTacts++;
                    else
                        _statisticsService.ErrorTacts++;
                    _statisticsService.AllTacts++;

                    _pumpServiceCallback.UpdateStatistics((StatisticsService)_statisticsService);

                    Thread.Sleep(1000);
                }
            });
        }
    }
}