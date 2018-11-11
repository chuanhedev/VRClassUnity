using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace chuanhe
{
  public delegate void OnFileDownloadingDelegate(int currentFile, int totalFile, string fileName, float progress);
  public class Updater : MonoBehaviour
  {
    // private string version;
    private string socketUrl;
    private string localVersionNumber;
    // private JSONObject config;
    public Action OnReadyHandler;

    public OnFileDownloadingDelegate OnFileDownloading;
    // public Action OnFileDownloaded;

    public Action<string> OnApkUpdate;
    public string appVersionNumber;

    [HideInInspector]
    public string serverVersion;
    // Use this for initialization
    public IEnumerator Init(string ip, string v)
    {
      socketUrl = ip;
      localVersionNumber = v;
      yield return checkUpdate();
    }
    IEnumerator updateFiles(List<JSONObject> files)
    {
      Debugger.Log("updateFiles");
      string tempurl = Request.RemoteUrl;
      Request.RemoteUrl = socketUrl;
      for (int i = 0; i < files.Count; i++)
      {
        string fileName = files[i]["name"].str;
        string pathName = files[i]["path"].str;
        Debugger.Log(pathName + fileName);
        string fullPath = Path.Combine(Path.Combine("resources/", pathName), fileName);
        yield return Request.DownloadFile(fullPath, fullPath, false, p =>
        {
          if (OnFileDownloading != null)
            OnFileDownloading(i + 1, files.Count, fileName, p);
        });
      }
      Request.RemoteUrl = tempurl;
      OnUpdateComplete();
    }

    private void OnUpdateComplete()
    {
      Debugger.Log(Color.blue, "OnUpdateComplete");
      // SaveConfigFile();
      if (OnReadyHandler != null)
        OnReadyHandler();
    }

    IEnumerator checkUpdate()
    {
      Dictionary<string, string> param = new Dictionary<string, string>();
      param.Add("app_version", appVersionNumber);
      param.Add("version", localVersionNumber);
      yield return Request.Get("version", param, (str) =>
      {
        Debugger.Log(Color.green, str);
        JSONObject obj = new JSONObject(str);
        serverVersion = obj["version"].str;
        bool update = obj["update"].str != "0";
        bool updateApk = obj.HasField("update_student_client");
        if (updateApk)
        {
          Debugger.Log(Color.red, "update apk " + obj["update_student_client"].str);
          // Application.OpenURL("http://" + url + "/resources/client.apk");
          if (OnApkUpdate != null)
          {
            OnApkUpdate(obj["update_student_client"].str);
          }
          return;
        }
        if (update)
        {
          StartCoroutine(updateFiles(obj["files"].list));
        }
        else
        {
          OnUpdateComplete();
        }
      }, str =>
      {
        OnUpdateComplete();
      });
    }

  }
}