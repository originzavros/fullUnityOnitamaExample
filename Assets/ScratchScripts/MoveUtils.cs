using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUtils
{
    public static IEnumerator SmoothLerp( float time, Vector3 start, Vector3 end, GameObject go)
    {
         float elapsedTime = 0;
         
         while (elapsedTime < time)
         {
             go.transform.position = Vector3.Lerp(start, end, (elapsedTime / time));
             elapsedTime += Time.deltaTime;
             yield return null;
         }
    }

    public static IEnumerator SmoothRotate(float time, Quaternion target, GameObject go)
    {
        float elapsedTime = 0;
        while(elapsedTime < time)
        {
            go.transform.rotation = Quaternion.Slerp(go.transform.rotation, target, (elapsedTime /time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


}
