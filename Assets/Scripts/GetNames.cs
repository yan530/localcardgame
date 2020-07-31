using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class GetNames : MonoBehaviour
{
    public TMP_InputField player1;
    public Image image1;
    public TMP_InputField player2;
    public Image image2;
    public TMP_InputField player3;
    public Image image3;
    public TMP_InputField player4;
    public Image image4;
    private List<string> names;
    private GameManager gameManager;

    void Start()
    {
        names = new List<string>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player1.text != "") { image1.gameObject.SetActive(true); } else { image1.gameObject.SetActive(false); }
        if (player2.text != "") { image2.gameObject.SetActive(true); } else { image2.gameObject.SetActive(false); }
        if (player3.text != "") { image3.gameObject.SetActive(true); } else { image3.gameObject.SetActive(false); }
        if (player4.text != "") { image4.gameObject.SetActive(true); } else { image4.gameObject.SetActive(false); }
    }

    public void StoreName()
    {
        if (player1.text != "") { names.Add(player1.text); }
        if (player2.text != "") { names.Add(player2.text); }
        if (player3.text != "") { names.Add(player3.text); }
        if (player4.text != "") { names.Add(player4.text); }

        gameManager.StartGame(names);
    }
}
