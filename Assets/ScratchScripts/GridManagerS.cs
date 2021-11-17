using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class GridManagerS : MonoBehaviour
{
    public static GridManagerS instance;
    public static GridManagerS GetInstance()
    { 
        if(instance == null)
        {
            instance = (GridManagerS)FindObjectOfType(typeof(GridManagerS));
        }
        return instance; 
    }

    public void Awake()
    {
        //singleton management, based off the official singleton wiki, which looks a little strange compared to how I've seen other people do it?
        //mayber better way to write this?
        if(instance == null)
        {
            instance = (GridManagerS)FindObjectOfType(typeof(GridManagerS));
            if(instance == null)
            {
                instance = this;
            }
        }
        GenerateGridList();
    }

    public int dimensionX = 5;
    public static int DimensionX(){return instance.dimensionX;}
    public int dimensionZ = 5;
    public static int DimensionZ(){return instance.dimensionZ;}

    public float nodeSize = 1f;
    public static float GetNodeSize()
    {
        return instance.nodeSize;
    }

    public float offsetX;
    public static float GetOffsetX()
    {
        instance.offsetX = DimensionX()*GetNodeSize()*.5f-GetNodeSize()*.5f;
        return instance.offsetX;
    }
    public float offsetZ;
    public static float GetOffsetZ()
    {
         instance.offsetZ = DimensionZ()*GetNodeSize()*.5f-GetNodeSize()*.5f;
        return instance.offsetZ;
    }
    public Transform nodePrefab;
    public Transform gridParent;

    public List<List<GridNodeS>> grid =  new List<List<GridNodeS>>();

    private List<GridNodeS> allNodes = new List<GridNodeS>();

    private GameObject cube;

    public void GenerateGridList()
    {
        int count = 0;
        for(int x=0; x < instance.dimensionX; x++)
        {
            List<GridNodeS> tempList = new List<GridNodeS>();
            for(int z=0; z < instance.dimensionZ; z++)
            {
                tempList.Add(new GridNodeS(count, x,z, new Vector3(x,0,z)*instance.nodeSize-new Vector3(offsetX,0,offsetZ)));
                count+=1;
                allNodes.Add(tempList[z]);
            }
            grid.Add(tempList);
        }
        EnableGridObjects();
        //makeMovableToken();
    }

    public void EnableGridObjects()
    {
        if(gridParent==null){
            gridParent=new GameObject("Grid").transform;
            gridParent.parent=transform.parent;
        }

        for(int x=0; x<dimensionX; x++){
            for(int z=0; z<dimensionZ; z++){
                GridNodeS node= grid[x][z];

                node.objHolder=(Transform)Instantiate(nodePrefab, node.GetPosition(), Quaternion.identity);

                node.objHolder.transform.parent=gridParent;
                node.objHolder.transform.localScale*=nodeSize*1;
                node.objHolder.transform.Rotate(90f,0,0,Space.World); //for planes or quads
                node.objHolder.tag = "GridNode";
                
                node.objHolder.gameObject.GetComponent<Renderer>().enabled=node.walkable;
            }
        }
    }

    public void Update()
    {
        //some initial code to check if the gridmanager was working, also to make sure the coordinates were correct

        // if ( Input.GetMouseButtonDown (0)){ 
        //     RaycastHit hit; 
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
        //     if ( Physics.Raycast (ray,out hit,100.0f)) {
        //         //StartCoroutine(ScaleMe(hit.transform));
        //         GridNodeS tempNode = GetNodeS(hit.transform.gameObject);
        //         //GridNodeS tempNode = hit.transform.GetComponent<GridNodeS>();
        //         //Debug.Log(tempNode);
        //         if(tempNode != null)
        //         {
        //             List<int> tempList = tempNode.GetXZ();
        //             Debug.Log("You selected the " + tempList[0] + ","+tempList[1]); // ensure you picked right object
        //             //cube.transform.position = tempNode.position;
        //         }
        //     }
        // }
    }


    public static GridNodeS GetNodeS(GameObject go)
    {
        for(int x=0; x<instance.grid.Count; x++){
            for(int z=0; z<instance.grid[x].Count; z++){
                if(instance.grid[x][z].GetNodeObject().gameObject==go) return instance.grid[x][z];
            }
        }
        return null;
    }

    public static GridNodeS GetNodeS(int x, int z)
    {
        if(x < 0 || x > 4 || z < 0 || z > 4)
        {
            return null;
        }
        
        return instance.grid[x][z];
    }

    //for temporary testing until I make player units.
    public static void makeMovableToken()
    {
        instance.cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.cube.transform.position = new Vector3(0, 0.5f, 0);
        instance.cube.GetComponent<Renderer>().material.color = Color.red;
        GridNodeS tempNode = GetNodeS(0,0);
        tempNode.tempUnit = instance.cube;
    }
}
