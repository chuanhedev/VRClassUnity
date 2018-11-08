using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

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

    public string port;

    [HideInInspector]
    public string url;

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
      updater.OnReadyHandler = () =>
      {
        socket.socket.url = "ws://" + url + "/socket.io/?EIO=4&transport=websocket";
        socket.gameObject.SetActive(true);
        if (OnReadyHandler != null)
          OnReadyHandler();
      };
      Dictionary<string, string> data = new Dictionary<string, string>();
      data.Add("id", SystemInfo.deviceUniqueIdentifier);
      StartCoroutine(GetIPAddress());
    }

    IEnumerator GetIPAddress()
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      data.Add("id", SystemInfo.deviceUniqueIdentifier);
      yield return Request.Post("ipaddress", new JSONObject(data), str =>
      {
        if (str != "")
        {
          url = str + ":" + port;
          // if (OnReadyHandler != null)
          //   OnReadyHandler(str);
          StartCoroutine(updater.Init(str));
        }
        else
        {
          StartCoroutine(OnGetIPError());
        }
      }, () =>
      {
        StartCoroutine(OnGetIPError());
      });
    }

    IEnumerator OnGetIPError()
    {
      yield return updater.GetConfig();
      url = updater.socketIP + ":" + port;
      if (updater.socketIP == "")
      {
        Debug.LogError("Socket IP cannot be empty");
      }
      else
      {
        yield return updater.Init();
      }
    }

    // IEnumerator PrepareToStart(string ip = "")
    // {
    //   yield return updater.Init(ip);
    //   if(OnReadyHandler!=null)
    //     OnReadyHandler(ip);
    // }
  }
}