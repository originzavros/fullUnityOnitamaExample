using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public List<CardData> cardSOList = new List<CardData>();
    //public List<OnitamaCard> oCardList = new List<OnitamaCard>();
    public List<OnitamaCard> p1Cards = new List<OnitamaCard>();
    public List<OnitamaCard> p2Cards = new List<OnitamaCard>();

    public static List<OnitamaCard> GetP2Cards()
    {
        return instance.p2Cards;
    }

    public OnitamaCard upcomingCard;

    public static CardManager instance;

    public static CardManager GetInstance()
    { 
        if(instance == null)
        {
            instance = (CardManager)FindObjectOfType(typeof(CardManager));
        }
        return instance; 
    }
    public void Awake()
    {
        if(instance == null)
        {
            instance = (CardManager)FindObjectOfType(typeof(CardManager));
            if(instance == null)
            {
                instance = this;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        p1SlotList.Add(p1slot1);
        p1SlotList.Add(p1slot2);
        p1SlotList.Add(p1slot3);
        p2SlotList.Add(p2slot1);
        p2SlotList.Add(p2slot2);
        p2SlotList.Add(p2slot3);


        CreateCards();
        EventManager.StartListening("EndPlayerMovement", OnEndPlayerMovement);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 p1slot1 = new Vector3(-3,.5f,0);
    private Vector3 p1slot2 = new Vector3(-3,.5f,4);
    private Vector3 p1slot3 = new Vector3(-3,.5f,8);
    private Vector3 p2slot1 = new Vector3(11,.5f,0);
    private Vector3 p2slot2 = new Vector3(11,.5f,4);
    private Vector3 p2slot3 = new Vector3(11,.5f,8);
    private List<Vector3> p1SlotList = new List<Vector3>();
    private List<Vector3> p2SlotList = new List<Vector3>();

    public static void CreateCards()
    {
        List<int> rngCardPool = RngUtils.GetNonRepeatingNumberFromRange(0,16,5);
        instance.CreateCard(instance.p1slot1, rngCardPool[0],1);
        instance.CreateCard(instance.p1slot2,rngCardPool[1],1);
        instance.CreateCard(instance.p1slot3,rngCardPool[2],1,true);

        instance.CreateCard(instance.p2slot1, rngCardPool[3],2);
        instance.CreateCard(instance.p2slot2,rngCardPool[4],2);
        //instance.CreateCard(new Vector3(11,.5f,8),rngCardPool[5],2);
    }

    public void CreateCard(Vector3 pos, int type, int playerId, bool upcoming=false)
    {
        //Instantiate(cardPrefab, pos, Quaternion.identity);
        GameObject temp;
        OnitamaCard temp2;
        temp = Instantiate(cardPrefab, pos, Quaternion.Euler(75, 0, 0));//75
        temp2 = temp.GetComponentInChildren<OnitamaCard>();
        temp2.cardinfo = cardSOList[type];
        temp2.playerId = playerId;
        temp2.LoadCardData(cardSOList[type]);
        temp2.TurnOffHighlight(); //the prefab needs the highlight enabled to start for some reason, otherwise doesn't show up at all
        temp2.TurnOffUpcoming();
        //goCardList.Add(temp);
        //oCardList.Add(temp2);
        if(upcoming)
        {
            upcomingCard = temp2;
            upcomingCard.TurnOnUpcoming();
        }

        if(playerId == 1)
        {
            p1Cards.Add(temp2);
        }
        else{
            //RotateCard(temp2);
            temp2.cardHolder.transform.rotation = temp2.cardHolder.transform.rotation * Quaternion.Euler(0,0,180);
            p2Cards.Add(temp2);
            
        }
    }

    public static OnitamaCard selectRandomPlayerCard(int playerId)
    {
        //there's only ever 2 cards for a player to choose from
        int randCard = Random.Range(0,2);
        //Debug.Log("randCard :" + randCard);

        if(playerId == 1)
        {
            return instance.p1Cards[randCard];
        }
        else
        {
            return instance.p2Cards[randCard];
        }
    }

    public void OnEndPlayerMovement(string eventName, ActionParams data)
    {
        int receivedPlayer = data.Get<int>("activePlayer");
        OnitamaCard receivedCard = data.Get<OnitamaCard>("lastSelectedCard");
        upcomingCard.TurnOffUpcoming();
        // Debug.Log("In CardManager, end of player movent event receiver");
        if(receivedPlayer == 1)
        {
            for(int i=0;i < p1Cards.Count;i++)
            {
                // Debug.Log("p1Cards[" + i + "] :" + p1Cards[i].cardinfo.name);
                if(p1Cards[i] == receivedCard)
                {
                    p1Cards[i].playerId = 2;
                    p2Cards.Add(p1Cards[i]);
                    RotateCard(p1Cards[i]);
                    MoveCardToPosition(p2slot3, p1Cards[i]);
                    p1Cards[i].TurnOnUpcoming();
                    upcomingCard = p1Cards[i];
                    p1Cards.RemoveAt(i);
                    break;
                }   
            }

            for(int i=0;i < p1Cards.Count;i++)
            {
                // Debug.Log("p1Cards[" + i + "] :" + p1Cards[i].cardinfo.name);
                MoveCardToPosition(p1SlotList[i],p1Cards[i]);
            }
        }
        else{
            for(int i=0;i < p2Cards.Count;i++)
            {
                // Debug.Log("p2Cards[" + i + "] :" + p2Cards[i].cardinfo.name);
                if(p2Cards[i] == receivedCard)
                {
                    p2Cards[i].playerId = 1;
                    p1Cards.Add(p2Cards[i]);
                    RotateCard(p2Cards[i]);
                    MoveCardToPosition(p1slot3, p2Cards[i]);
                    p2Cards[i].TurnOnUpcoming();
                    upcomingCard = p2Cards[i];
                    p2Cards.RemoveAt(i);
                    break;
                }
            }
            for(int i=0;i < p2Cards.Count;i++)
            {
                // Debug.Log("p2Cards[" + i + "] :" + p2Cards[i].cardinfo.name);
                MoveCardToPosition(p2SlotList[i],p2Cards[i]);
            }

        }
    }

    private void MoveCardToPosition( Vector3 pos, OnitamaCard card)
    {
        StartCoroutine(MoveUtils.SmoothLerp(1f, card.cardHolder.transform.position, pos, card.cardHolder));
        
    }

    public void RotateCard(OnitamaCard card)
    {
        Quaternion targetRotation =  card.cardHolder.transform.rotation * Quaternion.Euler(0,0,180);
        StartCoroutine(MoveUtils.SmoothRotate(2f,targetRotation, card.cardHolder));
    }

    public void CleanUpCards()
    {
        foreach(OnitamaCard card in p1Cards)
        {
            card.Destroy();
        }
        foreach(OnitamaCard card in p2Cards)
        {
            card.Destroy();
        }
        p1Cards.Clear();
        p2Cards.Clear();
    }

    public static void RestartGame()
    {
        instance.CleanUpCards();
        CreateCards();
    }

}
