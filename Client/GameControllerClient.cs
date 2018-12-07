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

    private string sceneName = "";

    public VRClassClient client;

    public SocketControllerClient socket;

    public ScoketVectorTracker cameraTracker;

    void Awake()
    {
      instant = this;
    }

    void Start(){
      client.OnReadyHandler = ()=>{
        message.text = "Done";
        this.socket.Init();
      };
      client.updater.OnFileDownloading = (fileIdx, fileTotal, fileName, progress)=>{
        message.text = string.Format("Downloading {0}({1}/{2}) {3}%", fileName, fileIdx, fileTotal, progress);
      };
      client.OnApkUpdate = ()=>{
        message.text = string.Format("Apk需要更新 程序将在5秒钟后关闭\n并下载新的apk 请手动进行安装");
        StartCoroutine(ExitGame());
      };
    }

    private IEnumerator ExitGame(){
      yield return new WaitForSeconds(4);
      Application.OpenURL(client.socketUrl + "/resources/student.apk");
      yield return new WaitForSeconds(1);
      Application.Quit();
    }

    public void ChangeScene(string name)
    {
      // ChangeScene(obj["scene"].str);
      Debugger.Log(Color.blue, "ChangeScene " + sceneName + " to " + name);
      if (sceneName == name) return;
      sceneName = name;
      StartCoroutine(LoadScene(name));
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
