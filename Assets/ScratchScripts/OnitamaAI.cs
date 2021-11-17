using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnitamaAI : MonoBehaviour
{
    OnitamaCard moveCard;
    GridNodeS aiMoveNode;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("AITurn", OnAiTurn);  
    }

    private void OnAiTurn(string eventName, ActionParams data)
    {
        Debug.Log("Starting AI Turn");
        StartCoroutine(RunAiActions());
    }

    //need to give end of turn actions time to finish
    IEnumerator RunAiActions()
    {
        yield return new WaitForSeconds(2);
        //DoRandomAction();
        DoEasyAction();
        EndAIMovement(); 
    }
    
    struct WeightedAiAction{
        public float score;
        public GridNodeS nodeS;
        public UnitS unitS;
        public OnitamaCard card;
    }

    //The ai will weight moves based on attackable targets, will attack targets if able, otherwise does a random movement
    private void DoEasyAction()
    {
        List<WeightedAiAction> weightedActionsList = new List<WeightedAiAction>();
        List<UnitS> aiUnits = GameManager.GetPlayerUnitList(2);
        List<OnitamaCard> aiCards = CardManager.GetP2Cards();
        GenerateWeightedActions(aiUnits,aiCards[0], ref weightedActionsList);
        GenerateWeightedActions(aiUnits,aiCards[1], ref weightedActionsList);

        // foreach(UnitS unit in aiUnits)
        // {
        //     List<GridNodeS> aiMoves = GameManager.BuildUnitMoveList(aiCards[0], unit.node);
        //     List<GridNodeS> legalMoves = CheckForLegalAIMoves(aiMoves, unit);
        //     if(legalMoves.Count > 0)
        //     {
        //         foreach(GridNodeS node in legalMoves)
        //         {
        //             WeightedAiAction temp;
        //             temp.nodeS = node;
        //             temp.unitS = unit;
        //             temp.score = 0;
        //             if(node.occupyingUnit != null)
        //             {
        //                 if(node.occupyingUnit.usType == UnitS.unitType.king)
        //                 {
        //                     temp.score = 40;
        //                 }
        //                 else{
        //                     temp.score = 20;
        //                 }
        //             }
        //             else{
        //                 temp.score = 1;
        //             }
        //             temp.score += Random.Range(0, 11); //just add a little variance
        //             weightedActionsList.Add(temp);
        //         }
        //     }
        // }
        WeightedAiAction bigScore = weightedActionsList[0];
        for(int i=1;i<weightedActionsList.Count;i++)
        {
            if(weightedActionsList[i].score >= bigScore.score)
            {
                bigScore = weightedActionsList[i];
            }
        }
        Debug.Log("Weighted action score :" + bigScore.score);
        Debug.Log("Weighted action target node :" + bigScore.nodeS.x + "," + bigScore.nodeS.z);
        moveCard = aiCards[0];
        bool successfulAction = DoAction(bigScore.nodeS, bigScore.unitS);
        if(!successfulAction)
        {
            Debug.Log("ai didn't do easy action");
        }
    }

    //passing by reference the list we want to fill as I don't want to generate a seperate list for each card, and instead add onto the same list
    private void GenerateWeightedActions(List<UnitS> aiUnits, OnitamaCard aiCard, ref List<WeightedAiAction> weightedActionsList)
    {
        foreach(UnitS unit in aiUnits)
        {
            List<GridNodeS> aiMoves = GameManager.BuildUnitMoveList(aiCard, unit.node);
            List<GridNodeS> legalMoves = CheckForLegalAIMoves(aiMoves, unit);
            if(legalMoves.Count > 0)
            {
                foreach(GridNodeS node in legalMoves)
                {
                    WeightedAiAction temp;
                    temp.nodeS = node;
                    temp.unitS = unit;
                    temp.card = aiCard;
                    temp.score = 0;
                    if(node.occupyingUnit != null)
                    {
                        if(node.occupyingUnit.usType == UnitS.unitType.king)
                        {
                            temp.score = 40;
                        }
                        else{
                            temp.score = 20;
                        }
                    }
                    else{
                        temp.score = 1;
                    }
                    temp.score += Random.Range(0, 11); //just add a little variance
                    weightedActionsList.Add(temp);
                }
            }
        }
    }

    //Has the ai do a completely random action with no weights, just chooses from available moves and does something
    private void DoRandomAction()
    {
        bool successfulAction = false;
        List<UnitS> aiUnits = GameManager.GetPlayerUnitList(2);
        int activeUnit = Random.Range(0, aiUnits.Count-1);
        moveCard = CardManager.selectRandomPlayerCard(2);
        //Debug.Log(moveCard.cardinfo.name);
        //Debug.Log(moveCard.playerId);
        List<GridNodeS> aiMoves = GameManager.BuildUnitMoveList(moveCard, aiUnits[activeUnit].node);
        List<GridNodeS> legalMoves = CheckForLegalAIMoves(aiMoves, aiUnits[activeUnit]);
        int move = Random.Range(0, legalMoves.Count-1);

        if(legalMoves.Count >= 1) //make sure we can make a legal move
        {
            successfulAction = DoAction(legalMoves[move], aiUnits[activeUnit]);
        }
        else{//we can end up with no moves on the rando unit, so need to go through everyone and find a legal move
            Debug.Log("random chosen unit has no moves, going through all units to look for move");
            for(int i=0; i< aiUnits.Count ;i++)
            {
                aiMoves = GameManager.BuildUnitMoveList(moveCard, aiUnits[i].node);
                legalMoves = CheckForLegalAIMoves(aiMoves, aiUnits[i]);
                if(legalMoves.Count >= 1) //make sure we can make a legal move
                {
                    move = Random.Range(0, legalMoves.Count-1);
                    successfulAction = DoAction(legalMoves[move], aiUnits[activeUnit]);
                    return;
                }
            }
        }
        if(!successfulAction)
        {
            Debug.Log("AI managed to not take an action");
        }
    }


    private List<GridNodeS> CheckForLegalAIMoves(List<GridNodeS> aiMoves, UnitS active)
    {
        List<GridNodeS> legalMoves = new List<GridNodeS>();
        for(int i=0; i< aiMoves.Count;i++)
        {
            if(aiMoves[i].occupyingUnit != null)
            {
                if(aiMoves[i].occupyingUnit.ptype != active.ptype)
                {
                    legalMoves.Add(aiMoves[i]);
                }
            }
            else
            {
                legalMoves.Add(aiMoves[i]);
            }
        }
        return legalMoves;
    }

    private bool DoAction(GridNodeS Move, UnitS aiUnit)
    {
        if(Move.occupyingUnit != null && Move.occupyingUnit.ptype != aiUnit.ptype)
        {
            GameManager.RemoveUnitAtNode(Move);
            aiUnit.MoveToGridNode(Move);
            aiMoveNode = Move;
            Debug.Log("AI attacks " + Move.x + "," + Move.z);
            return true;
            
        }
        else
        {
            if(Move.occupyingUnit == null)
            {
                aiUnit.MoveToGridNode(Move);
                aiMoveNode = Move;
                Debug.Log("AI moves to " + Move.x + "," + Move.z);
                return true;
            }
            Debug.Log("Action Failed");
            return false;
        }  
    }

    private void EndAIMovement()
    {
        ActionParams data = new ActionParams();
        data.Put("activePlayer", 2);
        data.Put("lastSelectedCard", moveCard);
        data.Put("lastSelectedNode", aiMoveNode);
        EventManager.TriggerEvent("EndPlayerMovement", data);
    }

}
