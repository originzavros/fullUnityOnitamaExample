using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<UnitS> player1Units = new List<UnitS>();
    public List<UnitS> player2Units = new List<UnitS>();
    public GameObject playerPawnPrefab;
    public GameObject playerKingPrefab;

    public bool pturn = true;
    public int turnNum = -1;
    private bool p1KingCaptured = false;
    private bool p2KingCaptured = false;
    private GridNodeS p1GoalNode;
    private GridNodeS p2GoalNode;
    private GameObject goal1Tmp;

    public GameObject GoalText;

    public int activePlayer = 0;

    public bool aiPlayer = true;

    public static GameManager instance;
    public static GameManager GetInstance()
    { 
        if(instance == null)
        {
            instance = (GameManager)FindObjectOfType(typeof(GameManager));
        }
        return instance; 
    }
    public void Awake()
    {
        //singleton management, based off the official singleton wiki, which looks a little strange compared to how I've seen other people do it?
        //mayber better way to write this?
        if(instance == null)
        {
            instance = (GameManager)FindObjectOfType(typeof(GameManager));
            if(instance == null)
            {
                instance = this;
            }
        }
    }

    void Start()
    {
        InitPlayers(); //need to init at start because can't guarantee gridmanager exists yet with awake, and awake is always called first.
        InitGoalNodes();
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
        RunGame();
    }

    public void InitPlayers()
    {
        for(int i = 0;i < 5;i++)
        {
            if( i == 2)
            {
                CreateUnit(1, i, 0, UnitS.unitType.king);
            }
            else
            {
                CreateUnit(1, i, 0, UnitS.unitType.pawn);
            }
            
        }
        for(int i = 0;i < 5;i++)
        {
            if( i == 2)
            {
                CreateUnit(2, i, 4, UnitS.unitType.king);
            }
            else
            {
                CreateUnit(2, i, 4, UnitS.unitType.pawn);
            }
        }
    }
    public void InitGoalNodes()
    {
        p1GoalNode = GridManagerS.GetNodeS(2, 4);
        p2GoalNode = GridManagerS.GetNodeS(2,0);
        // GameObject temp = Instantiate(GoalText, p1GoalNode.GetPosition() + new Vector3(.2f,2f,0), Quaternion.identity);
        // TextMeshPro tmp = temp.GetComponent<TextMeshPro>();
        // tmp.color = Color.red;
    }

    public void RunGame()
    {
        activePlayer = 1;
        StartCoroutine(setStartingActivePlayer()); //need to wait a sec for other stuff to initialize before sending our event
    }

    IEnumerator setStartingActivePlayer()
    {
        yield return new WaitForSeconds(.5f);
        ActionParams data = new ActionParams();
        data.Put("activePlayer", activePlayer);
        EventManager.TriggerEvent("ActivePlayer", data);
    }

    public void CreateUnit(int playerId, int x, int y, UnitS.unitType utype)
    {
        UnitS temp;
        GameObject newGo;
        GridNodeS node;
        if(utype == UnitS.unitType.pawn)
        {
            newGo = Instantiate(playerPawnPrefab, new Vector3(0,0,0), Quaternion.identity);
        }
        else
        {
            newGo = Instantiate(playerKingPrefab, new Vector3(0,0,0), Quaternion.identity);
        }

        temp = newGo.transform.GetComponent<UnitS>();
        //Debug.Log( "x : "+ x + ", y:" + y );
        node = GridManagerS.GetNodeS(x,y);
        node.occupyingUnit = temp;
        temp.node = node;
        temp.unitObj.transform.position = node.GetPosition() + new Vector3(0, 0.5f, 0);
        

        if(playerId == 1)
        {
            newGo.GetComponent<Renderer>().material.color = Color.red;
            temp.ptype = UnitS.player.p1;
            player1Units.Add(temp);
        }
        else
        {
            newGo.GetComponent<Renderer>().material.color = Color.blue;
            temp.ptype = UnitS.player.p2;
            player2Units.Add(temp);
        }
        
    }

    public void OnEndPlayerMovement(string eventName, ActionParams data)
    {
        int receivedPlayer = data.Get<int>("activePlayer");
        GridNodeS receivedNode = data.Get<GridNodeS>("lastSelectedNode");
        //Debug.Log("received node :" + receivedNode.GetXZ());
        Debug.Log("in OnEndPlayerMovement, active player received: " + receivedPlayer);

        if(CheckForGameEnd(receivedNode))
        {
            Debug.Log("GameOver");
            ActionParams temp = new ActionParams();
            temp.Put("activePlayer", activePlayer);
            EventManager.TriggerEvent("GameOver", temp);
        }
        else{
            if(receivedPlayer == 1)
            {
                activePlayer = 2;
            }
            if(receivedPlayer == 2)
            {
                activePlayer = 1;
            }

            if(aiPlayer && activePlayer == 2)
            {
                ActionParams ai = new ActionParams();
                EventManager.TriggerEvent("AITurn", ai);
            }
            else{
                ActionParams temp = new ActionParams();
                temp.Put("activePlayer", activePlayer);
                EventManager.TriggerEvent("ActivePlayer", temp);
            }
        }
        
    }

    public bool CheckForGameEnd(GridNodeS node)
    {
        if(p1KingCaptured || p2KingCaptured)
        {
            return true;
        }

        if(node == p1GoalNode)
        {
            //Debug.Log("got into matching goal node");
            if(p1GoalNode.occupyingUnit.usType == UnitS.unitType.king && p1GoalNode.occupyingUnit.ptype == UnitS.player.p1)
            {
                return true;
            }
            
        }
        if(node == p2GoalNode)
        {
            
            if(p2GoalNode.occupyingUnit.usType == UnitS.unitType.king && p2GoalNode.occupyingUnit.ptype == UnitS.player.p2)
            {
                return true;
            }
            
        }

        return false;
    }

    public static void RemoveUnitAtNode(GridNodeS node)
    {
        if(node.occupyingUnit != null)
        {
            UnitS temp = node.occupyingUnit;
            //check if that unit was the king, for end game conditions
            if(temp.usType == UnitS.unitType.king)
            {
                if(temp.ptype == UnitS.player.p1)
                {
                    instance.p1KingCaptured = true;
                }
                if(temp.ptype == UnitS.player.p2)
                {
                    instance.p2KingCaptured = true;
                }
            }
            GameManager.RemoveUnitFromPlayerList(temp);
            //Destroy(node.occupyingUnit);
            node.occupyingUnit = null;
            temp.Destroy();
        }
    }

    public static void RemoveUnitFromPlayerList(UnitS unit)
    {
        if(unit.ptype == UnitS.player.p1)
        {
            for(int i=0;i<instance.player1Units.Count;i++)
            {
                if(instance.player1Units[i] == unit)
                {
                    instance.player1Units.RemoveAt(i);
                }
            }
        }
        else
        {
            for(int i=0;i<instance.player2Units.Count;i++)
            {
                if(instance.player2Units[i] == unit)
                {
                    instance.player2Units.RemoveAt(i);
                }
            }
        }
    }

    public static List<GridNodeS> BuildUnitMoveList(OnitamaCard card, GridNodeS node)
    {
        List<CardMove> unitmoves = card.cardinfo.moveset;
        int x = node.x;
        int z = node.z;
        List<GridNodeS> rList = new List<GridNodeS>();

        if(card.playerId == 1)
        {
            rList = instance.LegalUnitMoves(unitmoves, x,z,1);
        }
        else{
            //need to reverse the coordinates because they are moving the opposite way on the grid
            rList = instance.LegalUnitMoves(unitmoves,x,z, -1);
        }
        return rList;
    }

    private List<GridNodeS> LegalUnitMoves(List<CardMove> moves, int x, int z, int mult)
    {
        GridNodeS temp;
        List<GridNodeS> rList = new List<GridNodeS>();
        for(int i=0;i< moves.Count;i++)
        {
            temp = GridManagerS.GetNodeS(moves[i].x * mult + x,moves[i].z * mult + z);
            if(temp != null)
            {
                rList.Add(temp);
            }  
        }
        return rList;
    }

    public static List<UnitS> GetPlayerUnitList( int x)
    {
        if(x == 1)
        {
            return instance.player1Units;
        }
        else{
            return instance.player2Units;
        }
    }

    public void CleanUpPlayerUnits()
    {
        foreach(UnitS item in player1Units)
        {
            item.Destroy();
        }
        foreach(UnitS item in player2Units)
        {
            item.Destroy();
        }
        player1Units.Clear();
        player2Units.Clear();
    }

    public static void RestartGame()
    {
        CardManager.RestartGame();
        instance.CleanUpPlayerUnits();
        instance.InitPlayers();

        instance.p1KingCaptured = false;
        instance.p2KingCaptured = false;
        instance.RunGame();

    }
}
