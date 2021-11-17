using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class CardData : ScriptableObject
{
    public new string name;
    public Sprite gridSprite;
    public List<CardMove> moveset =  new List<CardMove>();

    public CardData GetInstance()
    {
        return Instantiate(this);
    }
}

