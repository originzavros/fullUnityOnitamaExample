using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//: MonoBehaviour
public class GridNodeS 
{
    public int x, z;
    public int index;
    public Vector3 position;

    public Transform objHolder;
    public bool walkable = true;

    public UnitS occupyingUnit = null;
    public GameObject tempUnit;

    public GridNodeS(int indexv,int xv,int zv,Vector3 posv)
    {
        index=indexv; x=xv; z=zv; position=posv;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public List<int> GetXZ()
    {
        List<int> temp = new List<int>();
        temp.Add(x);
        temp.Add(z);
        return temp;
    }

    public Transform GetNodeObject()
    {
        return objHolder;
    }

    public void TurnOnHighlight()
    {
        objHolder.GetComponent<Renderer>().material.color = Color.yellow;
    }
    public void TurnOffHighlight()
    {
        objHolder.GetComponent<Renderer>().material.color = Color.white;
    }

    public void TurnOnMoveHighlight()
    {
        objHolder.GetComponent<Renderer>().material.color = Color.green;
    }

}
