using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

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
    public string sceneName = "1";

    private float sceneEnterTime;
    public GameObject clientPrefab;
    public Transform canvas;

    private SocketController socket;

    void Awake()
    {
      instant = this;
    }

    // Use this for initialization
    void Start()
    {
      socket = SocketController.instant;
      tracker = new Tracker();
      sceneEnterTime = Time.time;
      StartCoroutine(tracker.Event("GameStart"));
      OnSceneEnter();
    }

    private void OnSceneEnter()
    {
      Dictionary<string, string> p1 = new Dictionary<string, string>();
      p1.Add("Name", sceneName);
      StartCoroutine(tracker.Event("SceneEnter", p1, null));
    }

    private void OnSceneExit()
    {
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

    public void ChangeScene(Button btn)
    {
      string name = btn.transform.GetComponentInChildren<Text>().text;
      if (name != "" && name != sceneName)
      {
        OnSceneExit();
        sceneName = name;
        OnSceneEnter();
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["name"] = name;
        SocketController.instant.Emit("SCENE", new JSONObject(data));
        StartCoroutine(GameControllerServer.instant.LoadScene(name));
      }
    }

    public IEnumerator LoadScene(string name)
    {
      WWW www = new WWW(Request.GetPersistentPath("/resources/" + name + ".jpg"));
      yield return www;
      if (string.IsNullOrEmpty(www.error))
      {
        Texture2D tex = www.texture;
        MeshRenderer mr = sphere.GetComponent<MeshRenderer>();
        Material material = mr.material;
        Debug.Log(tex + " " + tex.width + " " + tex.height);
        material.SetTexture("_MainTex", tex);
        // material.SetColor("_Color", Color.red);
        //bg.sprite = Sprite.Create (tex, new Rect (0.0f, 0.0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
      }
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