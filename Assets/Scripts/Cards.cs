using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

//[CreateAssetMenu(fileName = "Cards", menuName = "Cards", order = 51)]
public class Cards : ScriptableObject, IComparable<Cards>
{
    private string type;
    private string label;
    private string description;
    private int landNum;
    //private GameObject AR;
    //private string RoomID;
    private int CardID;
    private GameObject attachment;

    public Button playButton;

    public void Init(int id, string type, string label, string description)
    {
        CardID = id;
        this.type = type;
        this.label = label;
        this.description = description;
    }

    public static Cards CreateInstance(int id, string type, string label, string description)
    {
        var data = ScriptableObject.CreateInstance<Cards>();
        data.Init(id, type, label, description);
        return data;
    }

    public string GetCardType()
    {
        return type;
    }

    public string GetCardDescription()
    {
        return description;
    }

    public int GetCardID()
    {
        return CardID;
    }

    public GameObject GetAttachment()
    {
        return attachment;
    }

    public void SetAttachment(GameObject attach)
    {
        attachment = attach;
    }

    public int CompareTo(Cards other)
    {
        return CardID - other.GetCardID();
    }
}