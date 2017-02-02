using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace TranslatorServer.Controllers
{
  public static class Utils
  {
    /// <summary>
    /// Read config information on the web.config file
    /// </summary>
    /// <param name="settingKey"></param>
    /// <returns></returns>
    public static string GetAppSetting(string settingKey)
    {
      return WebConfigurationManager.AppSettings[settingKey];
    }

    /// <summary>
    /// Generate a GUID based random name for the bucket
    /// </summary>
    /// <returns></returns>
    public static string GenerateRandomBucketName()
    {
      // the "t" at the begining is in case the GUID starts with number
      return "t" + Guid.NewGuid().ToString("N").ToLower();
    }

    /// <summary>
    /// Base64 encode a string (source: http://stackoverflow.com/a/11743162)
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string Base64Encode(this string plainText)
    {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return System.Convert.ToBase64String(plainTextBytes);
    }

    /// <summary>
    /// Base64 dencode a string (source: http://stackoverflow.com/a/11743162)
    /// </summary>
    /// <param name="base64EncodedData"></param>
    /// <returns></returns>
    public static string Base64Decode(this string base64EncodedData)
    {
      var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
      return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }
  }
}