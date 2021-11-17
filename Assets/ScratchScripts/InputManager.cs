using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private GridNodeS lastSelectedNode = null;
    private OnitamaCard lastSelectedCard = null;
    private bool inSelectionMode = false;

    private int layerMask = 1 << 9; //these are all the selectable layers bitshifted as thats how unity does stuff i guess

    private List<GridNodeS> moveList = new List<GridNodeS>();

    private int activePlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        layerMask = ~layerMask; //we invert the bitmask so that only layer 9 is ignored, which are the player pieces(we want to select grid tiles, not pieces currently)
        EventManager.StartListening("ActivePlayer", OnActivePlayerChange);
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetMouseButtonDown (0)){ 
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            if ( Physics.Raycast (ray,out hit,100.0f,layerMask)) {
                //StartCoroutine(ScaleMe(hit.transform));
                //GridNodeS tempNode = GridManagerS.GetNodeS(hit.transform.gameObject);

                GameObject tempHit = hit.transform.gameObject;

                //Debug.Log(tempHit);

                //GridNodeS tempNode = hit.transform.GetComponent<GridNodeS>();
                //Debug.Log(tempNode);
                if(tempHit.tag == "GridNode")
                {
                    GridNodeS tempNode = GridManagerS.GetNodeS(hit.transform.gameObject);
                    //if we click another unit after already selecting one, turn off highlights of old unit
                    if(tempNode.occupyingUnit != null && lastSelectedNode != null)
                    {
                        if(lastSelectedNode.occupyingUnit != null)
                        {
                            if(tempNode.occupyingUnit.ptype == lastSelectedNode.occupyingUnit.ptype)
                            {
                                inSelectionMode = false;
                                lastSelectedNode.TurnOffHighlight();
                            }
                        }
                        
                    }

                    if(inSelectionMode)
                    {
                        if(tempNode.occupyingUnit != null)
                        {
                            if(tempNode.occupyingUnit.ptype != lastSelectedNode.occupyingUnit.ptype && CheckIfInMoveList(tempNode))
                            {
                                GameManager.RemoveUnitAtNode(tempNode);
                                lastSelectedNode.occupyingUnit.MoveToGridNode(tempNode);
                                lastSelectedNode.TurnOffHighlight();
                                lastSelectedNode = tempNode;
                                ResetMoveList();
                                EndPlayerMovement();
                            }
                        }
                        else
                        {
                            if(tempNode == lastSelectedNode) //added some stuff above so won't be entering this, will leave here just in case
                            {
                                lastSelectedNode.TurnOffHighlight();
                                lastSelectedNode = null;
                            }
                            else
                            {
                                if(CheckIfInMoveList(tempNode))
                                {
                                    lastSelectedNode.occupyingUnit.MoveToGridNode(tempNode);
                                    lastSelectedNode.TurnOffHighlight();
                                    lastSelectedNode = tempNode;
                                    ResetMoveList();
                                    EndPlayerMovement();
                                }
                                
                            }
                        }
                        
                        inSelectionMode = false;
                    }
                    else
                    {
                        if(tempNode.occupyingUnit != null)
                        {
                            //Debug.Log("player unit check " +tempNode.occupyingUnit.ptype);
                            if(playerUnitCheck(tempNode.occupyingUnit.ptype))
                            {
                                inSelectionMode = true;
                                lastSelectedNode = tempNode;
                                lastSelectedNode.TurnOnHighlight();
                                SetupMoveList(tempNode);
                            } 
                        }
                    }

                    List<int> tempList = tempNode.GetXZ();
                    //Debug.Log("You selected the " + tempList[0] + ","+tempList[1]); // ensure you picked right object   
                }

                //need to handle left clicking on a card
                if(tempHit.tag == "OnitamaCard")
                {
                    OnitamaCard selectedCard = tempHit.GetComponentInChildren<OnitamaCard>();
                    if(playerCardCheck(selectedCard))
                    {
                        if(inSelectionMode)//if we have a unit selected, need to redraw it's movement
                        {
                            lastSelectedCard.TurnOffHighlight();
                            lastSelectedCard = selectedCard;
                            lastSelectedCard.TurnOnHighlight();
                            ResetMoveList();
                            SetupMoveList(lastSelectedNode);
                        }
                        else{
                            lastSelectedCard.TurnOffHighlight();
                            lastSelectedCard = selectedCard;
                            lastSelectedCard.TurnOnHighlight();
                        }
                    }
                    
                    //Debug.Log(lastSelectedCard);
                }
            }
        }
        
    }

    private bool CheckIfInMoveList(GridNodeS nodeS)
    {
        foreach(GridNodeS item in moveList)
        {
            if(nodeS == item)
            {
                return true;
            }
        }
        return false;

    }

    private void SetupMoveList(GridNodeS nodes)
    {
        ResetMoveList();
        moveList = GameManager.BuildUnitMoveList(lastSelectedCard ,nodes);
        foreach(GridNodeS item in moveList)
        {
            item.TurnOnMoveHighlight();
        }
    }

    private void ResetMoveList()
    {
        // if(moveList.Count > 0)
        // {
            foreach(GridNodeS item in moveList)
            {
                item.TurnOffHighlight();
            }
        // }
    }

    public void OnActivePlayerChange(string eventName, ActionParams data)
    {
        activePlayer = data.Get<int>("activePlayer");
        lastSelectedCard = CardManager.selectRandomPlayerCard(activePlayer);
        lastSelectedCard.TurnOnHighlight();
        //Debug.Log("got activeplayer event, active player is now: " + activePlayer);
    }

    private bool playerUnitCheck(UnitS.player id)
    {
        if(id == UnitS.player.p1 && activePlayer == 1)
        {
            return true;
        }
        if(id == UnitS.player.p2 && activePlayer == 2)
        {
            return true;
        }
        return false;
    }

    private bool playerCardCheck( OnitamaCard card)
    {
        if(card.playerId == 1 && activePlayer == 1)
        {
            return true;
        }
        if(card.playerId == 2 && activePlayer == 2)
        {
            return true;
        }
        return false;
    }

    public void EndPlayerMovement()
    {
        lastSelectedCard.TurnOffHighlight();
        ActionParams data = new ActionParams();
        data.Put("activePlayer", activePlayer);
        data.Put("lastSelectedCard", lastSelectedCard);
        data.Put("lastSelectedNode", lastSelectedNode);
        EventManager.TriggerEvent("EndPlayerMovement", data);
    }



}
