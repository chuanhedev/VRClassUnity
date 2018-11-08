﻿using System.Collections;
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

  private GameControllerClient game;

  void Start()
  {
    game = GameControllerClient.instant;
    socket = SocketController.instant;
    socket.OnConnectHandler = ConnectToServer;
  }

  public void Init()
  {
    socket.On("JOIN", OnUserJoin);
    socket.On("SCENE", OnScene);
    socket.On("CHECK", OnCheck);
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
    game.ChangeScene(evt.data["scene"].str);
  }

  private void OnCheck(SocketIOEvent evt)
  {
    ConnectToServer();
  }

  private void OnScene(SocketIOEvent evt)
  {
    game.ChangeScene(evt.data["name"].str);
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
