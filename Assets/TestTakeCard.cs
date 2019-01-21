using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.CardsScripts;
using Fool_online.Scripts.InRoom;
using UnityEngine;

public class TestTakeCard : MonoBehaviour
{
    public CardRoot cardToGrab;

    public void OnClick()
    {
        EnemyInfo en = FindObjectOfType<EnemyInfo>();
        en.PickCardsFromTable(new List<CardRoot>(new[]{ cardToGrab }), new List<CardRoot>());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
