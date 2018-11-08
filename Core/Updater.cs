using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace chuanhe
{
  public class Updater : MonoBehaviour
  {
    public string version;

		public string serverPath;

    // Use this for initialization
    void Start()
    {
      StartCoroutine(readConfig());

    }
    IEnumerator readConfig()
    {
      yield return Request.ReadPersistent("config.json", str =>
      {
        Debugger.Log("Succcess to load");
        JSONObject config = new JSONObject(str);
        version = config["version"].str;
        StartCoroutine(checkUpdate());
      }, () =>
      {
        Debugger.Log("failed to load");
        version = "1.0.0";
        SaveVersionFile();
        StartCoroutine(checkUpdate());
      });
    }

		private void SaveVersionFile()
    {
      string path = UnityEngine.Application.persistentDataPath + "/" + "config.json";
			Debugger.Log(path);
      StreamWriter writer = new StreamWriter(path, false);
      Dictionary<string, string> config = new Dictionary<string, string>();
      config.Add("version", version);
      writer.Write(new JSONObject(config).ToString());
      writer.Close();
    }

    IEnumerator updateFiles(List<JSONObject> files)
    {
      Debugger.Log("updateFiles");
      for (int i = 0; i < files.Count; i++)
      {
        string fileName = files[i]["name"].str;
        yield return Request.DownloadFile("resources/" + fileName, "resources/" + fileName);
				if(serverPath != null && serverPath != ""){
					string sourceFile = Path.Combine(UnityEngine.Application.persistentDataPath, "resources/" + fileName);
					string desFile = Path.Combine(serverPath, "resources/" + fileName);
					Debugger.Log(Color.blue, sourceFile + " to " + desFile);
					System.IO.File.Copy(sourceFile, desFile, true);
				}
      }
      Debugger.Log(Color.blue, "updateFiles done");
      SaveVersionFile();
    }
    IEnumerator checkUpdate()
    {
      Dictionary<string, string> param = new Dictionary<string, string>();
      param.Add("version", version);
      yield return Request.Get("version", param, (str) =>
      {
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