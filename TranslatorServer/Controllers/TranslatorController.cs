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

using Autodesk.Forge;
using Autodesk.Forge.Model;
using ExcelLibrary.SpreadSheet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace TranslatorServer.Controllers
{
  public class TranslatorController : ApiController
  {
    [HttpPut]
    [Route("api/forge/translator/uploadFile")]
    public async Task<string> UploadObject()
    {
      HttpRequest req = HttpContext.Current.Request;

      // we must have 1 file on the request (multiple files not handles on this sample)
      if (req.Files.Count != 1)
        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unexpected number of files"));
      HttpPostedFile file = req.Files[0];

      // save the file on the server
      string bucketKey = Utils.GenerateRandomBucketName();
      var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), bucketKey, file.FileName);
      if (!Directory.Exists(fileSavePath)) Directory.CreateDirectory(Path.GetDirectoryName(fileSavePath));
      file.SaveAs(fileSavePath);

      try
      {
        // authenticate with Forge
        dynamic oauth = await Get2LeggedTokenAsync(new Scope[] { Scope.BucketCreate, Scope.DataRead, Scope.DataCreate, Scope.DataWrite });

        // create the bucket
        BucketsApi buckets = new BucketsApi();
        buckets.Configuration.AccessToken = oauth.access_token;
        PostBucketsPayload bucketPayload = new PostBucketsPayload(bucketKey, null, PostBucketsPayload.PolicyKeyEnum.Transient);
        dynamic bucketResult = await buckets.CreateBucketAsync(bucketPayload);

        // upload the file/object, which will create a new object
        ObjectsApi objects = new ObjectsApi();
        objects.Configuration.AccessToken = oauth.access_token;
        dynamic uploadedObj;
        using (StreamReader streamReader = new StreamReader(fileSavePath))
        {
          uploadedObj = await objects.UploadObjectAsync(bucketKey,
                 file.FileName, (int)streamReader.BaseStream.Length, streamReader.BaseStream,
                 "application/octet-stream");
        }

        // start translating the file
        List<JobPayloadItem> outputs = new List<JobPayloadItem>()
        {
         new JobPayloadItem(
           JobPayloadItem.TypeEnum.Svf,
           new List<JobPayloadItem.ViewsEnum>()
           {
             JobPayloadItem.ViewsEnum._2d,
             JobPayloadItem.ViewsEnum._3d
           })
        };
        JobPayload job = new JobPayload(new JobPayloadInput(Utils.Base64Encode(uploadedObj.objectId)), new JobPayloadOutput(outputs));
        DerivativesApi derivative = new DerivativesApi();
        derivative.Configuration.AccessToken = oauth.access_token;
        dynamic jobPosted = await derivative.TranslateAsync(job);

      }
      catch (System.Exception ex)
      {
        // for this testing app, let's throw a full descriptive expcetion,
        // which is not a good idea in production
        throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message + ex.InnerException));
      }

      // cleanup server
      File.Delete(fileSavePath);

      return bucketKey;
    }

    public class RequestModel
    {
      public string guid { get; set; }
    }

    [HttpPost]
    [Route("api/forge/translator/status")]
    public async Task<int> TranslationProgress([FromBody] RequestModel request)
    {
      Guid testOutput;
      if (string.IsNullOrWhiteSpace(request.guid) || !Guid.TryParse(request.guid.TrimStart('t'), out testOutput)) throw new System.Exception("Invalid GUID");

      // authenticate with Forge
      dynamic oauth = await Get2LeggedTokenAsync(new Scope[] { Scope.DataRead });

      // get object on the bucket, should be just 1
      ObjectsApi objects = new ObjectsApi();
      objects.Configuration.AccessToken = oauth.access_token;
      dynamic objectsInBucket = await objects.GetObjectsAsync(request.guid);

      string objectId = string.Empty;
      foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsInBucket.items))
        objectId = objInfo.Value.objectId;

      // get the manifest, that includes the status of the translation
      DerivativesApi derivative = new DerivativesApi();
      derivative.Configuration.AccessToken = oauth.access_token;
      dynamic manifest = await derivative.GetManifestAsync(objectId.Base64Encode());

      return (string.IsNullOrWhiteSpace(Regex.Match(manifest.progress, @"\d+").Value) ? 100 : Int32.Parse(Regex.Match(manifest.progress, @"\d+").Value));
    }

    [HttpPost]
    [Route("api/forge/translator/xls")]
    public async Task<HttpResponseMessage> GetSpreadsheet([FromBody] RequestModel request)
    {
      Guid testOutput;
      if (string.IsNullOrWhiteSpace(request.guid) || !Guid.TryParse(request.guid.TrimStart('t'), out testOutput)) throw new System.Exception("Invalid GUID");

      // authenticate with Forge
      dynamic oauth = await Get2LeggedTokenAsync(new Scope[] { Scope.DataRead });

      // get object on the bucket, should be just 1
      ObjectsApi objects = new ObjectsApi();
      objects.Configuration.AccessToken = oauth.access_token;
      dynamic objectsInBucket = await objects.GetObjectsAsync(request.guid);

      string objectId = string.Empty;
      string objectKey = string.Empty;
      foreach (KeyValuePair<string, dynamic> objInfo in new DynamicDictionaryItems(objectsInBucket.items))
      {
        objectId = objInfo.Value.objectId;
        objectKey = objInfo.Value.objectKey;
      }

      string xlsFileName = objectKey.Replace(".rvt", ".xls");
      var xlsPath = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), request.guid, xlsFileName);
      if (File.Exists(xlsPath))
        return SendFile(xlsPath);// if the Excel file was already generated

      DerivativesApi derivative = new DerivativesApi();
      derivative.Configuration.AccessToken = oauth.access_token;

      // get the derivative metadata
      dynamic metadata = await derivative.GetMetadataAsync(objectId.Base64Encode());
      foreach (KeyValuePair<string, dynamic> metadataItem in new DynamicDictionaryItems(metadata.data.metadata))
      {
        dynamic hierarchy = await derivative.GetModelviewMetadataAsync(objectId.Base64Encode(), metadataItem.Value.guid);
        dynamic properties = await derivative.GetModelviewPropertiesAsync(objectId.Base64Encode(), metadataItem.Value.guid);

        Workbook xls = new Workbook();
        foreach (KeyValuePair<string, dynamic> categoryOfElements in new DynamicDictionaryItems(hierarchy.data.objects[0].objects))
        {
          string name = categoryOfElements.Value.name;
          Worksheet sheet = new Worksheet(name);
          for (int i = 0; i < 100; i++) sheet.Cells[i, 0] = new Cell(""); // unless we have at least 100 cells filled, Excel understand this file as corrupted

          List<long> ids = GetAllElements(categoryOfElements.Value.objects);
          int row = 1;
          foreach (long id in ids)
          {
            Dictionary<string, object> props = GetProperties(id, properties);
            int collumn = 0;
            foreach (KeyValuePair<string, object> prop in props)
            {
              sheet.Cells[0, collumn] = new Cell(prop.Key.ToString());
              sheet.Cells[row, collumn] = new Cell(prop.Value.ToString());
              collumn++;
            }

            row++;
          }

          xls.Worksheets.Add(sheet);
        }
        xls.Save(xlsPath);
      }
      return SendFile(xlsPath);
    }

    /// <summary>
    /// Get a list of properties for a given ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    private Dictionary<string, object> GetProperties(long id, dynamic properties)
    {
      Dictionary<string, object> returnProps = new Dictionary<string, object>();
      foreach (KeyValuePair<string, dynamic> objectProps in new DynamicDictionaryItems(properties.data.collection))
      {
        if (objectProps.Value.objectid != id) continue;
        string name = objectProps.Value.name;
        long elementId = long.Parse(Regex.Match(name, @"\d+").Value);
        returnProps.Add("ID", elementId);
        returnProps.Add("Name", name.Replace("[" + elementId.ToString() + "]", string.Empty));
        foreach (KeyValuePair<string, dynamic> objectPropsGroup in new DynamicDictionaryItems(objectProps.Value.properties))
        {
          if (objectPropsGroup.Key.StartsWith("__")) continue;
          foreach (KeyValuePair<string, dynamic> objectProp in new DynamicDictionaryItems(objectPropsGroup.Value))
          {
            if (!returnProps.ContainsKey(objectProp.Key))
              returnProps.Add(objectProp.Key, objectProp.Value);
            else
              Debug.Write(objectProp.Key);
          }
        }
      }
      return returnProps;
    }

    /// <summary>
    /// Recursively run through the list of objects hierarchy getting alls IDs with no children
    /// </summary>
    /// <param name="objects"></param>
    /// <returns></returns>
    private List<long> GetAllElements(dynamic objects)
    {
      List<long> ids = new List<long>();
      foreach (KeyValuePair<string, dynamic> item in new DynamicDictionaryItems(objects))
      {
        foreach (KeyValuePair<string, dynamic> keys in item.Value.Dictionary)
        {
          if (keys.Key.Equals("objects"))
          {
            return GetAllElements(item.Value.objects);
          }
        }
        foreach (KeyValuePair<string, dynamic> element in objects.Dictionary)
        {
          if (!ids.Contains(element.Value.objectid))
            ids.Add(element.Value.objectid);
        }

      }
      return ids;
    }

    /// <summary>
    /// Prepare a HTTP Response with a file for download
    /// </summary>
    /// <param name="xlsPath"></param>
    /// <returns></returns>
    public HttpResponseMessage SendFile(string xlsPath)
    {
      HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
      var stream = new FileStream(xlsPath, FileMode.Open);
      result.Content = new StreamContent(stream);
      result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = Path.GetFileName(xlsPath)
      };
      return result;
    }


    /// <summary>
    /// Request a 2-legged token
    /// </summary>
    /// <param name="scopes"></param>
    /// <returns></returns>
    public static async Task<dynamic> Get2LeggedTokenAsync(Scope[] scopes)
    {
      TwoLeggedApi apiInstance = new TwoLeggedApi();
      string grantType = "client_credentials";
      dynamic bearer = await apiInstance.AuthenticateAsync(
        Utils.GetAppSetting("FORGE_CLIENT_ID"),
        Utils.GetAppSetting("FORGE_CLIENT_SECRET"),
        grantType,
        scopes);
      return bearer;
    }
  }
}
