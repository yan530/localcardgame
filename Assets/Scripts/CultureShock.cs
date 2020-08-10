using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultureShock : MonoBehaviour
{

    public GameObject panel;

    private void WaitForMe()
    {
        panel.SetActive(false);
    }

    public void OnClick()
    {
        panel.SetActive(true);
        Invoke("WaitForMe", 2f);
    }
}
