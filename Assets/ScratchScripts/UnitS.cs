using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitS : MonoBehaviour
{
    public enum unitType { pawn, king};
    public enum player{p1, p2};
    public GameObject unitObj;
    public int unitId;
    public GridNodeS node;
    public unitType usType;
    public player ptype;

    public void MoveToPosition(Vector3 pos)
    {
        StartCoroutine(MoveAnimation(pos));
    }

    private IEnumerator MoveAnimation(Vector3 pos)
    {
        AudioManager.PlayClip("startMove");
        StartCoroutine(MoveUtils.SmoothLerp(.5f,unitObj.transform.position, pos +  new Vector3(0, 2.5f, 0), unitObj));
        yield return new WaitForSeconds(.5f);
        StartCoroutine(MoveUtils.SmoothLerp(.5f,unitObj.transform.position, pos +  new Vector3(0, .5f, 0), unitObj));
        yield return new WaitForSeconds(.3f);
        AudioManager.PlayClip("endMove");
    }

    public void MoveToGridNode( GridNodeS targetNode)
    {
        
        //GridManagerS.GetNodeS(node.x, node.z).occupyingUnit = null;
        targetNode.occupyingUnit = this;
        //Debug.Log("before moving, occupied by "+ node.occupyingUnit + this.node.x + "," + this.node.z);
        node.occupyingUnit = null;
        //Debug.Log("after moving, occupied by "+ node.occupyingUnit + this.node.x + "," + this.node.z);
        node = targetNode;
        

        MoveToPosition(targetNode.position);
    }

    public void Destroy()
    {
        Destroy(this.unitObj);
        Destroy(this);
    }


}
