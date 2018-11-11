using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;

namespace chuanhe
{
  public class GameControllerServer : MonoBehaviour
  {
    private Dictionary<string, Client> clients = new Dictionary<string, Client>();
    public Text message;
    public InputField input;
    public static GameControllerServer instant;
    public Transform mainCamera;
    public GameObject sphere;

    [HideInInspector]
    public Client focusClient;
    private Tracker tracker;

    [HideInInspector]
    public string school;

    [HideInInspector]
    public string deviceName;


    [HideInInspector]
    private string sceneName = "";

    private float sceneEnterTime;
    public GameObject clientPrefab;
    public Transform canvas;

    private SocketController socket;

    public VRClassServer server;

    void Awake()
    {
      instant = this;
    }

    // Use this for initialization
    void Start()
    {
      socket = SocketController.instant;
      socket.Init();
      tracker = new Tracker();
      sceneEnterTime = Time.time;
      StartCoroutine(tracker.Event("GameStart"));
      ChangeScene("1");
    }

    private void OnSceneEnter()
    {
      Debugger.Log(Color.green, "OnSceneEnter " + sceneName);
      Dictionary<string, string> p1 = new Dictionary<string, string>();
      p1.Add("Name", sceneName);
      StartCoroutine(tracker.Event("SceneEnter", p1, null));
    }

    private void OnSceneExit()
    {
      if (sceneName == "") return;
      Debugger.Log(Color.green, "OnSceneExit " + sceneName);
      Dictionary<string, string> p1 = new Dictionary<string, string>();
      p1.Add("Name", sceneName);
      Dictionary<string, string> m1 = new Dictionary<string, string>();
      m1.Add("Time", (Time.time - sceneEnterTime).ToString());
      StartCoroutine(tracker.Event("SceneExit", p1, m1));
      sceneEnterTime = Time.time;
    }

    void OnApplicationQuit()
    {
      OnSceneExit();
    }

    public void ChangeScene(string name)
    {
      Debugger.Log(Color.blue, "ChangeScene " + name + (name != "") + (name != sceneName));
      if (name != "" && name != sceneName)
      {
        StartCoroutine(GameControllerServer.instant.LoadScene(name, () =>
        {
          OnSceneExit();
          sceneName = name;
          OnSceneEnter();
          Dictionary<string, string> data = new Dictionary<string, string>();
          data["path"] = name;
          socket.EmitEvent("SCENE", new JSONObject(data));
        }));
      }
    }

    public void ChangeScene(Button btn)
    {
      string name = btn.transform.GetComponentInChildren<Text>().text;
      ChangeScene(name);
      // if (name != "" && name != sceneName)
      // {
      //   OnSceneExit();
      //   sceneName = name;
      //   OnSceneEnter();
      //   Dictionary<string, string> data = new Dictionary<string, string>();
      //   data["name"] = name;
      //   socket.EmitEvent("SCENE", new JSONObject(data));
      //   StartCoroutine(GameControllerServer.instant.LoadScene(name));
      // }
    }

    public void ChangeSceneByPath()
    {
      string name = input.text;
      ChangeScene(name);
      // if (name != "" && name != sceneName)
      // {
      //   StartCoroutine(GameControllerServer.instant.LoadScene(name, () =>
      //   {
      //     OnSceneExit();
      //     sceneName = name;
      //     OnSceneEnter();
      //     Dictionary<string, string> data = new Dictionary<string, string>();
      //     data["path"] = name;
      //     socket.EmitEvent("SCENE", new JSONObject(data));
      //   }));
      // }
    }

    public IEnumerator LoadScene(string name, Action callback = null)
    {
      string url = server.socketUrl + "/resources/" + name + ".jpg";
      Debugger.Log("Loading " + url);
      WWW www = new WWW(url);
      yield return www;
      if (string.IsNullOrEmpty(www.error))
      {
        Debugger.Log(Color.green, "Loaded " + url);
        Texture2D tex = www.texture;
        MeshRenderer mr = sphere.GetComponent<MeshRenderer>();
        Material material = mr.material;
        Debug.Log(tex + " " + tex.width + " " + tex.height);
        material.SetTexture("_MainTex", tex);
        // material.SetColor("_Color", Color.red);
        //bg.sprite = Sprite.Create (tex, new Rect (0.0f, 0.0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
        if (callback != null)
          callback();
      }
      else
        Debugger.Log(Color.red, "Failed to load " + url);
    }

    public void CreateClient(JSONObject obj)
    {
      string name = obj["name"].str;
      if (clients.ContainsKey(name)) return;
      GameObject ins = GameObject.Instantiate(clientPrefab);
      Client c = ins.GetComponent<Client>();
      ins.transform.GetChild(0).GetComponent<Text>().text = name;
      ins.GetComponent<Button>().onClick.AddListener(() => focusClient = c);
      c.clientName = name;
      c.transform.SetParent(canvas);
      clients.Add(name, c);
      ArrangeClients();
    }

    public void RemoveClient(JSONObject obj)
    {
      string name = obj["name"].str;
      if (clients.ContainsKey(name))
      {
        GameObject.Destroy(clients[name].gameObject);
        clients.Remove(name);
      }
      ArrangeClients();
    }

    private void ArrangeClients()
    {
      int count = 0;
      foreach (string key in clients.Keys)
      {
        Debug.Log(clients[key].GetComponent<RectTransform>().position);
        clients[key].GetComponent<RectTransform>().localPosition = new Vector3(340, 180 - count * 45, 0);
        count++;
      }
    }

    public void OnClientClick(Client client)
    {
      // Dictionary<string, string> data = new Dictionary<string, string>();
      // data["name"] = input.text;
      Debug.Log(client);
      Debug.Log(client.clientName);
      focusClient = client;
    }

  }
}