using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace chuanhe
{
  public class Client : MonoBehaviour
  {
    // Use this for initialization
    public string clientName;
    [HideInInspector]
    // public Transform transform;
    void Start()
    {
      // transform = GetComponent<Transform>();
    }

    void setPosition(float x, float y, float z)
    {
      transform.position = new Vector3(x, y, z);
    }
  }
}