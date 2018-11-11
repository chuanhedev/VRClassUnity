using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using chuanhe;

public class SocketControllerClient : MonoBehaviour
{
  public SocketController socket;
  // Use this for initialization
  private float connectionChecker = 0.0f;

  private GameControllerClient gameController;

  void Start()
  {
    gameController = GameControllerClient.instant;
    socket = SocketController.instant;
    socket.OnConnectHandler = ConnectToServer;
  }

  public void Init()
  {
    socket.On("JOIN", OnUserJoin);
    socket.On("CHECK", OnCheck);
    socket.OnEvent("SCENE", OnScene);
    socket.OnEvent("URL", OnOpenUrl);
    socket.OnEvent("APK", OnOpenApk);
    gameController.cameraTracker.OnForwardUpdateHandler = (v3) =>
    {
      JSONObject obj = new JSONObject();
      obj["id"] = JSONObject.CreateStringObject(SystemInfo.deviceUniqueIdentifier);
      obj["value"] = JSONObject.CreateStringObject(v3.x + "," + v3.y + "," + v3.z);
      socket.EmitEvent("LOOKAT", obj);
    };
  }

  private void ConnectToServer(SocketIOEvent evt = null)
  {
    Dictionary<string, string> data = new Dictionary<string, string>();
    data["name"] = SystemInfo.deviceUniqueIdentifier;
    data["role"] = "s";
    Vector3 position = Vector3.zero;
    data["position"] = position.x + "," + position.y + "," + position.z;
    socket.Emit("JOIN", new JSONObject(data));
  }

  private void OnUserJoin(SocketIOEvent evt)
  {
    Debug.Log("message from server OnUserJoin" + evt.data);
    gameController.ChangeScene(evt.data["scene"].str);
  }

  private void OnCheck(SocketIOEvent evt)
  {
    ConnectToServer();
  }

  private void OnScene(JSONObject obj)
  {
    gameController.ChangeScene(obj["name"].str);
  }

  void OpenPackage(string pkgName)//包名
  {
    using (AndroidJavaClass jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    {
      using (AndroidJavaObject joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
      {
        using (AndroidJavaObject joPackageManager = joActivity.Call<AndroidJavaObject>("getPackageManager"))
        {
          using (AndroidJavaObject joIntent = joPackageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", pkgName))
          {
            if (null != joIntent)
            {
              joActivity.Call("startActivity", joIntent);
            }
          }
        }
      }
    }
  }

  private void OnOpenApk(JSONObject obj)
  {
    OpenPackage(obj["value"].str);
  }
  private void OnOpenUrl(JSONObject obj)
  {
    Application.OpenURL(obj["value"].str);
  }

  void Update()
  {
    connectionChecker += Time.deltaTime;
    if (connectionChecker > 2f)
    {
      connectionChecker = 0;
      if (socket.started)
        socket.Emit("CHECK");
    }
  }
}
