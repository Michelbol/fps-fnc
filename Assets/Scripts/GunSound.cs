using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSound : MonoBehaviour
{

    public AudioSource Shoot;

    public AudioSource EjectMagazine;

    public AudioSource Reload;

    public AudioSource Empty;
    // Start is called before the first frame update
    void Start()
    {
        var audios = GetComponents<AudioSource>();
        Shoot = audios[0];
        EjectMagazine = audios[1];
        Reload = audios[2];
        Empty = audios[3];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
