using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace chuanhe
{
  public class Tracker
  {

    public IEnumerator Event(string eventName)
    {
      Dictionary<string, string> d = new Dictionary<string, string>();
      d.Add("event", eventName);
      d.Add("device", SystemInfo.deviceUniqueIdentifier);
      JSONObject data = new JSONObject(d);
      yield return Request.Post("event", data);
    }

    // public IEnumerator Event(string eventName, Dictionary<string, string> properties){
    // 	Dictionary<string, string> d = new Dictionary<string, string>();
    // 	d.Add("event", eventName);
    // 	JSONObject data = new JSONObject(d);
    // 	yield return Request.Post("event", data);
    // }

    public IEnumerator Event(string eventName, Dictionary<string, string> properties, Dictionary<string, string> measurements)
    {
      Dictionary<string, string> d = new Dictionary<string, string>();
      d.Add("event", eventName);
      d.Add("device", SystemInfo.deviceUniqueIdentifier);
      JSONObject data = new JSONObject(d);
      if (properties != null)
      {
        data["properties"] = new JSONObject(properties);
      }
      if (measurements != null)
      {
        data["measurements"] = new JSONObject(measurements);
      }
      Debugger.Log(data.ToString());
      yield return Request.Post("event", data);
    }
  }
}