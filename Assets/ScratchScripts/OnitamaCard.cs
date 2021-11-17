using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class OnitamaCard : MonoBehaviour
{
    public GameObject cardHolder;
    public MeshCollider cardCollider;
    public TextMeshPro cardName;
    public SpriteRenderer cardBackgroundSprite;
    public SpriteRenderer cardMovesSprite;
    public CardData cardinfo;
    public int playerId;

    [SerializeField]
    private SpriteRenderer highlightSprite;
    [SerializeField]
    private SpriteRenderer overlayShadeSprite;
    [SerializeField]
    private TextMeshPro upcomingText;

    void Start()
    {
        cardinfo = cardinfo.GetInstance();
        cardName.text =  cardinfo.name;
        cardMovesSprite.sprite =  cardinfo.gridSprite;
    }

    public void LoadCardData(CardData cardinfo2)
    {
        cardName.text =  cardinfo2.name;
        cardMovesSprite.sprite =  cardinfo2.gridSprite;
    }

    public void TurnOnHighlight()
    {
        highlightSprite.enabled = true;
        //Debug.Log(highlightSprite);
       // Debug.Log(highlightSprite.enabled);
    }

    public void TurnOffHighlight()
    {
        highlightSprite.enabled = false;
    }

    public void TurnOnUpcoming()
    {
        overlayShadeSprite.enabled = true;
        upcomingText.enabled = true;
        cardCollider.enabled = false;
    }

    public void TurnOffUpcoming()
    {
        overlayShadeSprite.enabled = false;
        upcomingText.enabled = false;
        cardCollider.enabled = true;
    }

    public void Destroy()
    {
        Destroy(this.cardHolder);
        Destroy(this);
    }


}
