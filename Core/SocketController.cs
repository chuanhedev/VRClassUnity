using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SocketIO;

namespace chuanhe
{
  public class SocketController : MonoBehaviour
  {
    public SocketIOComponent socket;
    public static SocketController instant;
    [HideInInspector]
    public bool started = false;

    [HideInInspector]
    public Action<SocketIOEvent> OnErrorHandler;
    [HideInInspector]
    public Action<SocketIOEvent> OnConnectHandler;
    [HideInInspector]
    public Action<SocketIOEvent> OnCloseHandler;
    [HideInInspector]
    public Action<SocketIOEvent> OnOpenHandler;
    [HideInInspector]
    public Action<SocketIOEvent> OnDisconnectHandler;

    void Awake()
    {
      instant = this;
    }

    void Start()
    {
      socket.On("connect", OnConnect);
      socket.On("close", OnClose);
      socket.On("open", OnOpen);
      socket.On("disconnect", OnDisconnect);
      socket.On("error", OnError);
    }

    private void OnConnect(SocketIOEvent evt)
    {
      started = true;
      Debugger.Log(Color.blue, "OnUserConnected " + evt.data);
      if (OnConnectHandler != null)
        OnConnectHandler(evt);
    }

    private void OnClose(SocketIOEvent evt)
    {
      started = false;
      Debugger.Log(Color.blue, "close " + socket.IsConnected + " " + evt.ToString());
      if (OnCloseHandler != null)
        OnCloseHandler(evt);
    }

    private void OnOpen(SocketIOEvent evt)
    {
      started = true;
      Debugger.Log(Color.blue, "open " + socket.IsConnected + " " + evt.ToString());
      if (OnOpenHandler != null)
        OnOpenHandler(evt);
    }

    private void OnError(SocketIOEvent evt)
    {
      started = false;
      Debugger.Log(Color.blue, "error " + socket.IsConnected + " " + evt.ToString());
      if (OnErrorHandler != null)
        OnErrorHandler(evt);
    }

    private void OnDisconnect(SocketIOEvent evt)
    {
      started = false;
      Debugger.Log(Color.blue, "disconnect " + socket.IsConnected + " " + evt.ToString());
      if (OnDisconnectHandler != null)
        OnDisconnectHandler(evt);
    }

    public void On(string eventName, Action<SocketIOEvent> callback)
    {
      socket.On(eventName, callback);
    }

    public void Emit(string eventName)
    {
      if (!started) return;
      this.socket.Emit(eventName);
    }

    public void Emit(string eventName, string str)
    {
      if (!started) return;
      Dictionary<string, string> data = new Dictionary<string, string>();
      data["value"] = str;
      JSONObject obj = new JSONObject(data);
      this.socket.Emit(eventName, obj);
    }

    public void Emit(string eventName, JSONObject obj)
    {
      if (!started) return;
      this.socket.Emit(eventName, obj);
    }

    public void EmitVector(string eventName, Vector3 v3)
    {
      this.Emit(eventName, v3.x + "," + v3.y + "," + v3.z);
    }

  }
}