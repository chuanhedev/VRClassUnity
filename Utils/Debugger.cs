using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace chuanhe
{
  public class Debugger
  {

    // Use this for initialization
    public static void Log(string str)
    {
      Debug.Log(str);
      //		GameManager.instant.message.text += str + "\n";
    }

    public static void Log(Color color, string str)
    {
      Debug.Log(string.Format("<color=#{0:X2}{1:X2}{2:X2}>{3}</color>", (byte)(color.r * 255f), (byte)(color.g * 255f), (byte)(color.b * 255f), str));
    }
  }
}