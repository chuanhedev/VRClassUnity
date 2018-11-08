using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace chuanhe
{
  public enum Environment
  {
    Production, Development
  }
  public class VRClassServer : MonoBehaviour
  {

    // Use this for initialization
    public Environment environment;

    [Header("Production")]
    public string prodRemoteUrl = "http://www.iyoovr.com/hsyx";
    [Header("Development")]
    public string devRemoteUrl = "http://localhost/chuanhe";

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
      Dictionary<string, string> data = new Dictionary<string, string>();
      data.Add("ip", LocalIPAddress());
      data.Add("id", SystemInfo.deviceUniqueIdentifier);
      StartCoroutine(Request.Post("ipaddress", new JSONObject(data)));
    }

    public string LocalIPAddress()
    {
      IPHostEntry host;
      string localIP = "";
      host = Dns.GetHostEntry(Dns.GetHostName());
      foreach (IPAddress ip in host.AddressList)
      {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
          localIP = ip.ToString();
          break;
        }
      }
      return localIP;
    }

    
  }
}