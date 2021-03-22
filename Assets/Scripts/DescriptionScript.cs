using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionScript : MonoBehaviour
{
    public GameObject panel;

    private Text description;

    // Start is called before the first frame update
    void Start()
    {
       
        description = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(description.text.Length > 0)
        {
            Debug.Log("Ativa a descrição!");
            if (!panel.activeSelf)
            {
                panel.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Desativa a descrição!");
            if (panel.activeSelf)
            {
                panel.SetActive(false);
            }
        }
    }
}
