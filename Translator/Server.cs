using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
  public static class Server
  {
    private struct EndPoints
    {
      public static string BaseURL { get { return ConfigurationManager.AppSettings["TranslatorServer"]; } }
      public const string Upload = "api/forge/translator/uploadFile";
      public const string Status = "api/forge/translator/{guid}";
      public const string XLS = "api/forge/translator/{guid}/xls";
    }

    public static async Task<string> UploadFile(string filePath)
    {
      var client = new RestClient(EndPoints.BaseURL);
      var request = new RestRequest(EndPoints.Upload, Method.PUT);
      request.AddFile("FileToTranslate", File.ReadAllBytes(filePath), Path.GetFileName(filePath));
      IRestResponse response = await client.ExecuteTaskAsync(request);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
        throw new System.Exception("Error uploading file: " + response.StatusCode);

      return JsonConvert.DeserializeObject<string>(response.Content);
    }

    public static async Task<int> GetTranslationStatus(string guid)
    {
      var client = new RestClient(EndPoints.BaseURL);
      var request = new RestRequest(EndPoints.Status.Replace("{guid}", guid), Method.GET);
      IRestResponse response = await client.ExecuteTaskAsync(request);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
        return -1;// throw new System.Exception("Cannot get translation status: " + response.StatusCode);

      return JsonConvert.DeserializeObject<int>(response.Content);
    }

    public static async Task<byte[]> Download(string guid)//, string destinationFolder)
    {
      var client = new RestClient(EndPoints.BaseURL);
      var request = new RestRequest(EndPoints.XLS.Replace("{guid}", guid), Method.GET);

      IRestResponse response;
      do
      {
        System.Threading.Thread.Sleep(1000);
        response = await client.ExecuteTaskAsync(request);
      } while (response.StatusCode != System.Net.HttpStatusCode.OK);

      return client.DownloadData(request);
    }
  }
}
