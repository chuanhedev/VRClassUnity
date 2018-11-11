using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;

namespace chuanhe
{
  public class VRClassClient : MonoBehaviour
  {

    // Use this for initialization
    public Environment environment;

    [Header("Production")]
    public string prodRemoteUrl = "http://www.iyoovr.com/hsyx";
    [Header("Development")]
    public string devRemoteUrl = "http://localhost/chuanhe";

    private SocketController socket;

    public Action OnReadyHandler;

    public Updater updater;

    public string socketPort;

    // [HideInInspector]
    // public string socketIP;

    public JSONObject localConfig;


    // public Tracker tracker;

    void Awake()
    {
      if (environment == Environment.Development)
      {
        Request.RemoteUrl = devRemoteUrl;
      }
      else
      {
        Request.RemoteUrl = prodRemoteUrl;
      }
    }

    void Start()
    {
      socket = SocketController.instant;
      updater.OnReadyHandler = () =>
      {
        SaveConfigFile();
        socket.socket.url = "ws://" + socketIP + ":" + socketPort + "/socket.io/?EIO=4&transport=websocket";
        socket.Init();
        if (OnReadyHandler != null)
          OnReadyHandler();
      };
      StartCoroutine(AppStart());
    }

    private void SaveConfigFile()
    {
      if (!String.IsNullOrEmpty(updater.serverVersion))
        version = updater.serverVersion;
      string path = UnityEngine.Application.persistentDataPath + "/" + "config.json";
      Debugger.Log(path);
      StreamWriter writer = new StreamWriter(path, false);
      writer.Write(localConfig.ToString());
      writer.Close();
    }


    public IEnumerator AppStart()
    {
      yield return GetConfig();
      yield return GetIPAddress();
    }


    public IEnumerator GetConfig()
    {
      yield return Request.ReadPersistent("config.json", str =>
      {
        Debugger.Log(Color.blue, "GetConfig done " + str);
        localConfig = new JSONObject(str);
      }, () =>
      {
        Debugger.Log(Color.red, "GetConfig fail");
        localConfig = new JSONObject();
        localConfig["ip"] = JSONObject.CreateStringObject("");
        localConfig["version"] = JSONObject.CreateStringObject("");
      });
    }

    public string version
    {
      get
      {
        return localConfig["version"].str;
      }
      set
      {
        localConfig["version"] = JSONObject.CreateStringObject(value);
      }
    }

    public string socketUrl
    {
      get
      {
        return "http://" + socketIP + ":" + socketPort;
      }
    }

    public string socketIP
    {
      get
      {
        return localConfig["ip"].str;
      }
      set
      {
        localConfig["ip"] = JSONObject.CreateStringObject(value);
      }
    }
    IEnumerator GetIPAddress()
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      data.Add("id", SystemInfo.deviceUniqueIdentifier);
      yield return Request.Post("ipaddress", new JSONObject(data)
      , ip => HandleServerIP(ip)
      , () => HandleServerIP());
    }

    private void HandleServerIP(string ip = "")
    {
      if (!String.IsNullOrEmpty(ip))
      {
        //update ip
        socketIP = ip;
      }

      if (String.IsNullOrEmpty(socketIP))
      {
        //Throw Exception;
        Debug.LogError("Socket IP cannot be empty");
      }
      else
        StartCoroutine(updater.Init(socketUrl, version));
    }
  }
}