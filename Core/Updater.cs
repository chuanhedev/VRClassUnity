﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace chuanhe
{
  public class Updater : MonoBehaviour
  {
    // private string version;
    [HideInInspector]
    public string url;
    private JSONObject config;
    public Action OnReadyHandler;
    // Use this for initialization
    public IEnumerator Init(string ip = "")
    {
      if (ip != "")
        socketIP = ip;
      yield return checkUpdate();
    }

    public IEnumerator GetConfig()
    {
      yield return Request.ReadPersistent("config.json", str =>
      {
        config = new JSONObject(str);
      }, () =>
      {
        config = new JSONObject();
      });
    }

    public string version
    {
      get
      {
        return config.HasField("version") ? config["version"].str : "";
      }
      set
      {
        config["version"] = new JSONObject(value);
      }
    }

    public string socketIP
    {
      get
      {
        return config.HasField("ip") ? config["ip"].str : "";
      }
      set
      {
        config["ip"] = new JSONObject(value);
      }
    }

    // IEnumerator readConfig()
    // {
    //   yield return Request.ReadPersistent("config.json", str =>
    //   {
    //     Debugger.Log("Succcess to load");
    //     JSONObject config = new JSONObject(str);
    //     version = config["version"].str;
    //     StartCoroutine(checkUpdate());
    //   }, () =>
    //   {
    //     Debugger.Log("failed to load");
    //     version = "1.0.0";
    //     SaveVersionFile();
    //     StartCoroutine(checkUpdate());
    //   });
    // }

    private void SaveConfigFile()
    {
      string path = UnityEngine.Application.persistentDataPath + "/" + "config.json";
      Debugger.Log(path);
      StreamWriter writer = new StreamWriter(path, false);
      // Dictionary<string, string> config = new Dictionary<string, string>();
      // config.Add("version", version);
      writer.Write(new JSONObject(config).ToString());
      writer.Close();
    }

    IEnumerator updateFiles(List<JSONObject> files)
    {
      Debugger.Log("updateFiles");
      string tempurl = Request.RemoteUrl;
      // Request.RemoteUrl = "http://" + SocketController.instant.url;
      Request.RemoteUrl = url;
      for (int i = 0; i < files.Count; i++)
      {
        string fileName = files[i]["name"].str;
        yield return Request.DownloadFile("resources/" + fileName, "resources/" + fileName);
      }
      Request.RemoteUrl = tempurl;
      Debugger.Log(Color.blue, "updateFiles done");
      SaveConfigFile();
      if (OnReadyHandler != null)
        OnReadyHandler();
    }
    IEnumerator checkUpdate()
    {
      Dictionary<string, string> param = new Dictionary<string, string>();
      param.Add("version", version);
      yield return Request.Get("version", param, (str) =>
      {
        Debugger.Log(Color.green, str);
        JSONObject obj = new JSONObject(str);
        string serverVersion = obj["version"].str;
        Debugger.Log(serverVersion);
        if (serverVersion != version)
        {
          //update game
          version = serverVersion;
          StartCoroutine(updateFiles(obj["files"].list));
        }
      });
    }

  }
}