using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace chuanhe
{
  // public enum Environment
  // {
  //   Production, Development
  // }
  public class GameControllerClient : MonoBehaviour
  {
    // public Client client;
    public Text message;
    public static GameControllerClient instant;

    public GameObject sphere;

    private string sceneName = "1";

    public VRClassClient client;

    public SocketControllerClient socket;

    void Awake()
    {
      instant = this;
    }

    public void ChangeScene(string name)
    {
      if (sceneName == name) return;
      sceneName = name;
      StartCoroutine(LoadScene(name));
    }

    public void UpdateScene(JSONObject obj)
    {
      ChangeScene(obj["scene"].str);
    }

    IEnumerator LoadScene(string name)
    {
      Debug.Log(Request.GetPersistentPath("resources/" + name + ".jpg"));
      WWW www = new WWW(Request.GetPersistentPath("resources/" + name + ".jpg"));
      yield return www;
      if (string.IsNullOrEmpty(www.error))
      {
        Debugger.Log("no error");
        Texture2D tex = www.texture;
        MeshRenderer mr = sphere.GetComponent<MeshRenderer>();
        Material material = mr.material;
        Debug.Log(tex + " " + tex.width + " " + tex.height);
        material.SetTexture("_MainTex", tex);
      }
      else
      {
        Debugger.Log("error " + www.error);
      }
    }
  }
}
