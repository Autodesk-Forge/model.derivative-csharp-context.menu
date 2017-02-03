using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace TranslatorServer
{
  public class ExceptionLogger : IExceptionLogger
  {
    public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
    {
#if DEBUG
      Debug.Write(context.Exception.Message);
      Debug.Write(context.Exception.StackTrace);
      throw new System.Exception(context.Exception.Message, context.Exception); // debugger will stop here, see context.Exception (or console output)
#else
      // store on your error log

      // return a generic error to end-user
      throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
#endif
    }
  }

  public class ExceptionHandler : IExceptionHandler
  {
    public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
    {
#if DEBUG
      Debug.Write(context.Exception.Message);
      Debug.Write(context.Exception.StackTrace);
      throw new System.Exception(context.Exception.Message, context.Exception); // debugger will stop here, see context.Exception (or console output)
#else
      // store on your error log
      
      // return a generic error to end-user
      throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
#endif
    }
  }
}