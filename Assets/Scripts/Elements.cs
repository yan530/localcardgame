using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elements : ScriptableObject
{
    private bool collected;
    private string question;
    private string label;
    private int ElementID;

    public void Init(int id, string label, string question, bool collected)
    {
        ElementID = id;
        this.label = label;
        this.question = question;
        this.collected = collected;
    }

    public static Elements CreateInstance(int id, string label, string question, bool collected)
    {
        var data = ScriptableObject.CreateInstance<Elements>();
        data.Init(id, label, question, false);
        return data;
    }

    public string GetLabel()
    {
        return label;
    }

    public string GetQuestion()
    {
        collected = true;
        return question;
    }

    public bool IsCollected()
    {
        return collected;
    }
}