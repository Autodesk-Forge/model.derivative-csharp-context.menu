using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace TranslatorServer
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // track erros globally, see ExceptionHandling.cs
      config.Services.Replace(typeof(IExceptionHandler), new ExceptionHandler());
      config.Services.Replace(typeof(IExceptionLogger), new ExceptionLogger());

      // Web API routes
      config.MapHttpAttributeRoutes();
    }
  }
}
