/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace TranslatorServer
{
  public class ExceptionLogger : IExceptionLogger
  {
    public Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
    {
      return null;
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