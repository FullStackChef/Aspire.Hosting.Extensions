namespace Aspire.WebApplicationBuilder
{
    public interface IWebApplication
    {
        public Action<WebApplicationBuilder> ConfigureApplication { get; }
    }
}
