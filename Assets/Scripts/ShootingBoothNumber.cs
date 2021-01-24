using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingBoothNumber : MonoBehaviour
{
    public TextMesh TextComponent;

    public string TextNumber = "#1";
    // Start is called before the first frame update
    void Start()
    {
        TextComponent.text = TextNumber;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
