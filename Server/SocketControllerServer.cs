using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using chuanhe;
using SocketIO;
public class SocketControllerServer : MonoBehaviour
{

  // Use this for initialization
  private SocketController socket;

	private GameControllerServer game;
  void Start()
  {
		game = GameControllerServer.instant;
    socket = SocketController.instant;
    socket.On("JOIN", OnUserJoin);
    socket.On("LEAVE", OnUserLeave);
    socket.On("MOVE", OnUserMove);
    socket.On("USERS", OnUsers);
		socket.OnConnectHandler = ConnectToServer;
  }

  private void ConnectToServer(SocketIOEvent evt)
  {
    Dictionary<string, string> data = new Dictionary<string, string>();
    data["name"] = "teacher";
    data["role"] = "t";
    Vector3 position = Vector3.zero;
    data["position"] = position.x + "," + position.y + "," + position.z;
    socket.Emit("JOIN", new JSONObject(data));
  }

  

  private void OnUsers(SocketIOEvent evt)
  {
    List<JSONObject> users = evt.data["users"].list;
    Debugger.Log(Color.blue, "OnUser");
    for (int i = 0; i < users.Count; i++)
    {
      game.CreateClient(users[i]);
    }
  }

  private void OnUserJoin(SocketIOEvent evt)
  {
    Debug.Log("message from server OnUserJoin" + evt.data);
    game.CreateClient(evt.data);
  }

  private void OnUserLeave(SocketIOEvent evt)
  {
    Debug.Log("message from server OnUserLeave" + evt.data);
    game.RemoveClient(evt.data);
  }

  private void OnUserMove(SocketIOEvent evt)
  {
    if (game.focusClient && evt.data["name"].str == game.focusClient.clientName)
    {
      Debug.Log("message from server OnUserMove" + evt.data);
      string[] l = evt.data["position"].str.Split(',');
      Vector3 newPos = new Vector3(float.Parse(l[0]), float.Parse(l[1]), float.Parse(l[2]));
      game.mainCamera.forward = newPos;
    }
  }
}
