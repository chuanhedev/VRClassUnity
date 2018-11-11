using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace chuanhe
{
  public class ScoketVectorTracker : MonoBehaviour
  {

    // Use this for initialization
    public Transform targetTransform;
    // public TransformProperty property;
    public int updateFrequency = 5;
    //如果距离小于该值 则不发送事件
    public float updateLimit = 0;
    private int tick = 0;

    private Vector3 lastForwardVector = Vector3.zero;

    private Vector3 lastPositionVector = Vector3.zero;

    public Action<Vector3> OnPositionUpdateHandler;
    public Action<Vector3> OnForwardUpdateHandler;



    // Update is called once per frame
    void Update()
    {
      tick++;
      if (tick % updateFrequency == 0)
      {
        if(OnPositionUpdateHandler!=null){
          Vector3 p = targetTransform.position;
          if ((lastPositionVector - p).magnitude > updateLimit)
          {
            OnPositionUpdateHandler(p);
            lastPositionVector = p;
          }
        }
        if(OnForwardUpdateHandler!=null){
          Vector3 f = targetTransform.forward;
          if ((lastForwardVector - f).magnitude > updateLimit)
          {
            OnForwardUpdateHandler(f);
            lastForwardVector = f;
          }
        }
      }
    }
  }
}