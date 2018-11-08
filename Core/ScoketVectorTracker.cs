using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace chuanhe
{
  public enum TransformProperty
  {
    Forward, Position
  }
  public class ScoketVectorTracker : MonoBehaviour
  {

    // Use this for initialization
    public Transform targetTransform;
    public TransformProperty property;
    public int updateFrequency = 5;
    //如果距离小于该值 则不发送事件
    public float updateLimit = 0;
    private int tick = 0;

    private Vector3 lastVector = Vector3.zero;


    // Update is called once per frame
    void Update()
    {
      tick++;
      if (tick % updateFrequency == 0)
      {
        Vector3 v3;
        if (property == TransformProperty.Forward)
          v3 = targetTransform.forward;
        else
          v3 = targetTransform.position;
        float dis = (lastVector - v3).magnitude;
        if (dis > updateLimit)
        {
          // SocketController.instant.EmitVector("MOVE", v3);
          lastVector = v3;
        }
      }
    }
  }
}