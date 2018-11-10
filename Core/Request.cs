using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace chuanhe
{
  public class Request
  {
    public static WWW current;
    public static string RemoteUrl = "http://localhost/chuanhe";
    //public static Action<float> downloadProgressHandler;

    //	public static IEnumerator ReadRemote (string str, Action<string> handler)
    //	{
    //		string url = RemoteUrl+"/" +str;
    //		Debugger.Log ("loading " + url);
    //		UnityWebRequest www = UnityWebRequest.Get (Utils.ApplyRandomVersion (url));
    //		yield return www.Send ();
    //		if (www.isError) {
    //			Debugger.Log ("unabled to load " + url);
    //		} else {
    //			Debugger.Log ("Loaded successfully");
    //			handler.Invoke (www.downloadHandler.text);
    //		}
    //	}


    public static IEnumerator ReadRemote(string str, Action<string> handler, Action failedHandler = null)
    {
      //		string url = RemoteUrl + "/" + str;
      //		Debugger.Log ("loading " + url);
      //		WWW www = new WWW (Utils.ApplyRandomVersion (url));
      //		current = www;
      //		yield return www;
      //		if (!String.IsNullOrEmpty (www.error)) {
      //			Debugger.Log ("unabled to load " + url);
      //			if (failedHandler != null)
      //				failedHandler.Invoke ();
      //		} else {
      //			Debugger.Log ("Loaded successfully");
      //			handler.Invoke (www.text);
      //		}
      yield return ReadUrl(RemoteUrl + "/" + str, handler, failedHandler);
    }

    public static IEnumerator ReadUrl(string str, Action<string> handler, Action failedHandler = null)
    {
      Debugger.Log("loading " + str);
      WWW www = new WWW(Utils.ApplyRandomVersion(str));
      current = www;
      yield return www;
      if (!String.IsNullOrEmpty(www.error))
      {
        Debugger.Log("unabled to load " + str);
        if (failedHandler != null)
          failedHandler.Invoke();
      }
      else
      {
        Debugger.Log("Loaded successfully");
        handler.Invoke(www.text);
      }
    }


    public static IEnumerator ReadStreaming(string str, Action<string> handler, Action failedHandler = null)
    {
      yield return Read(GetStreamingPath(str), handler, failedHandler);
    }

    public static IEnumerator ReadPersistent(string str, Action<string> handler, Action failedHandler = null)
    {
      yield return Read(GetPersistentPath(str), handler, failedHandler);
    }

    public static string ResolvePath(string path, bool addFilePrefix = true)
    {
      if (!addFilePrefix)
        return path;
      string str = "file:///" + path;
      str = str.Replace("file:////", "file:///");
      return str;
    }

    public static string GetStreamingPath(string str)
    {
      if (string.IsNullOrEmpty(str))
        return "";
      return ResolvePath(UnityEngine.Application.streamingAssetsPath + "/" + str);
    }

    public static string GetPersistentPath(string str)
    {
      if (string.IsNullOrEmpty(str))
        return "";
      // Debugger.Log(UnityEngine.Application.persistentDataPath);
      return ResolvePath(UnityEngine.Application.persistentDataPath + "/" + str);
    }

    public static string GetRemoteAPI(string api)
    {
      return RemoteUrl + "/" + api + ".php";
    }

    public static IEnumerator Read(string str, Action<string> handler, Action failedHandler = null)
    {
      Debugger.Log("loading " + str);
      WWW www = new WWW(str);
      current = www;
      yield return www;
      if (!String.IsNullOrEmpty(www.error))
      {
        Debugger.Log("unabled to load " + str);
        if (failedHandler != null)
          failedHandler.Invoke();
      }
      else
      {
        Debugger.Log("Loaded successfully");
        handler.Invoke(www.text);
      }
    }


    public static IEnumerator Get(string src, Dictionary<string, string> data = null, Action<string> callback = null, Action<string> failedCallback = null, bool strict = false)
    {
      src = strict ? src : RemoteUrl + "/" + src + ".php";
      if (data != null && data.Keys.Count > 0)
      {
        src += "?";
        foreach (string key in data.Keys)
        {
          src += key + "=" + data[key] + "&";
        }
        src = src.Substring(0, src.Length - 1);
      }
      Debugger.Log(src);
      WWW www = new WWW(src);
      yield return www;
      if (www.error != null && www.error != "")
      {
        Debugger.Log("get failed " + www.error);
        if (failedCallback != null)
          failedCallback(www.error);
      }
      else
      {
        Debugger.Log("get " + www.text);
        if (callback != null)
          callback(www.text);
      }
    }


    public static IEnumerator Post(string src, string value = "", Action<string> callback = null, Action failedCallback = null)
    {
      WWWForm form = new WWWForm();
      form.AddField("data", value);
      yield return post(GetRemoteAPI(src), form, callback, failedCallback);
    }

    public static IEnumerator Post(string src, JSONObject json, Action<string> callback = null, Action failedCallback = null)
    {
      WWWForm form = new WWWForm();
      form.AddField("data", json.ToString());
      yield return post(GetRemoteAPI(src), form, callback, failedCallback);
    }

    public static IEnumerator post(string src, WWWForm form, Action<string> callback = null, Action failedCallback = null)
    {
      Debug.Log("posting " + src);
      using (UnityWebRequest www = UnityWebRequest.Post(src, form))
      {
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
          Debug.Log("post error " + src + " " + www.error);
          if (failedCallback != null)
            failedCallback();
        }
        else
        {
          Debug.Log("post " + src + " " + www.downloadHandler.text);
          if (callback != null)
            callback(www.downloadHandler.text);
        }
      }
    }
    public static IEnumerator DownloadFile(string src, string dest, bool absolute = false, Action<float> progressHandler = null)
    {
      //
      src = absolute ? src : Utils.ApplyRandomVersion(RemoteUrl + "/" + src);
      Debugger.Log("Downloading " + src + " to " + dest);
      WWW www = new WWW(src);
      current = www;
      while (current != null && current.progress < 1)
      {
        if (progressHandler != null)
          progressHandler.Invoke(www.progress);
        yield return null;
      }
      yield return current;
      dest = absolute ? dest : Path.Combine(UnityEngine.Application.persistentDataPath, dest);
      if (!Directory.Exists(Path.GetDirectoryName(dest)))
        Directory.CreateDirectory(Path.GetDirectoryName(dest));
      if (www.error == null || www.error == "")
      {
        try
        {
          File.WriteAllBytes(dest, www.bytes);
          Debugger.Log("Downloaded " + src + " to " + dest);
        }
        catch (Exception ex)
        {
          Debugger.Log(Color.red, "Downloaded " + src + " to " + dest + " with error ");
          Debugger.Log(Color.red, ex.ToString());
        }
      }
      else
      {
        Debugger.Log(Color.red, "Failed to download " + src + " to " + dest + " with error ");
        Debugger.Log(Color.red, www.error);
      }

    }

    public static void Cancel()
    {

      if (current != null)
      {
        //Debugger.Log (current.isDone.ToString (), "blue");
        current.Dispose();
        current = null;
        //Debugger.Log (current.isDone.ToString (), "blue");
      }
    }
  }
}
