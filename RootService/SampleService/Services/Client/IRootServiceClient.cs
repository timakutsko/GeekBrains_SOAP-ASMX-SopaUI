using RootServiceNamespace;

namespace SampleService.Services.Client
{
    public interface IRootServiceClient
    {
        public RootServiceClient RootServiceClient { get; }
        public Task<IEnumerable<WeatherForecast>> Get();
    }
}
