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
    /// <summary>
    /// List of endpoints on the TranslatorServer that this app will use
    /// </summary>
    private struct EndPoints
    {
      // ToDo: obfuscate these strings to avoid reflection
      public static string BaseURL { get { return ConfigurationManager.AppSettings["TranslatorServer"]; } }
      public const string Upload = "api/forge/translator/uploadFile";
      public const string Status = "api/forge/translator/status";
      public const string XLS = "api/forge/translator/xls";
    }

    /// <summary>
    /// Upload the specified file and return the GUID that identifies it on the server
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Check the translation status for this GUID
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static async Task<int> GetTranslationStatus(string guid)
    {
      var client = new RestClient(EndPoints.BaseURL);
      var request = new RestRequest(EndPoints.Status, Method.POST);
      request.AddParameter("guid", guid);
      IRestResponse response = await client.ExecuteTaskAsync(request);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
        return -1;// throw new System.Exception("Cannot get translation status: " + response.StatusCode);

      return JsonConvert.DeserializeObject<int>(response.Content);
    }

    /// <summary>
    /// Download the Excel file for a given GUID
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static async Task<byte[]> Download(string guid)//, string destinationFolder)
    {
      var client = new RestClient(EndPoints.BaseURL);
      var request = new RestRequest(EndPoints.XLS, Method.GET);
      request.AddParameter("guid", guid);

      // This checking process needs improvement
      IRestResponse response;
      do
      {
        System.Threading.Thread.Sleep(1000);
        response = await client.ExecuteTaskAsync(request);
      } while (response.StatusCode != System.Net.HttpStatusCode.OK);

      // the RestSharp library don't return the request status
      // when downloading files
      return client.DownloadData(request);
    }
  }
}
