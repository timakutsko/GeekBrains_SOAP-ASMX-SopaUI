using System.ServiceModel;

namespace PumpService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class PumpService : IPumpService
    {
        private readonly IStatisticsService _statisticsService;

        private readonly ISettingService _settingService;

        private readonly IScriptService _scriptService;

        public PumpService()
        {
            _statisticsService = new StatisticsService();
            _settingService = new SettingService();
            _scriptService = new ScriptService(_statisticsService, _settingService, Callback);
        }

        public void RunScript()
        {
            _scriptService.Run(10);
        }

        public void UpdateAndCompileScript(string fileName)
        {
            _settingService.FileName = fileName;
            _scriptService.Compile();
        }

        IPumpServiceCallback Callback
        {
            get
            {
                if (OperationContext.Current != null)
                    return OperationContext.Current.GetCallbackChannel<IPumpServiceCallback>();

                return null;
            }
        }
    }
}
