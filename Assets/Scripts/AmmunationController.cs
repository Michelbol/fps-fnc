using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmunationController : MonoBehaviour
{
    [SerializeField] Text AmmunitionText;

    public Image FirstBullet;

    public Image MiddleBullet;

    public Image LastBullet;

    public Camera Camera;

    private float TotalAmmunition = 100f;

    private float AmmunitionLeft = 100f;

    private bool Shooting = false;

    private float FirstBulletPercent = 60f;

    private float MiddleBulletPercent = 30f;

    private RaycastHit Hit;

    private RaycastHit[] ObjectsHits;

    private bool HitSometing;

    private bool CanShoot;

    public GunSound GunSound;

    private DateTime LastShot;

    private DateTime LastPlayEmptySound;

    private int Rate = 100;

    private bool IsReloading = false;

    private DateTime StartReloading;

    private int MiliSecondsDurationReloading = 3000;

    private bool isEscapePress = false;

    // Start is called before the first frame update
    void Start()
    {
        LastShot = DateTime.Now;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isEscapePress)
        {
            isEscapePress = true;
            Cursor.visible = !Cursor.visible;
        }
        if (Input.GetKeyUp(KeyCode.Escape) && isEscapePress)
        {
            isEscapePress = false;
        }

        HitSometing = aimHitSomething();
        CanShoot = canShoot();
        BulletHud();
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }

        if (IsReloading)
        {
            if (IsReloadFinish())
            {
                IsReloading = false;
                Reload();
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shooting = true;
            if (CanShoot)
            {                
                LastShot = DateTime.Now;
                AmmunitionLeft--;
                GunSound.Shoot.Play();
                GunSound.EjectMagazine.Play();
            }
        }

        if (Shooting)
        {
            EmptyGun();
            if (HitSometing && CanShoot)
            {
                if (Hit.collider.gameObject.CompareTag("Target"))
                {
                    Hit.collider.gameObject.GetComponent<TargetMovimentation>().selected = true;
                    Hit.collider.gameObject.SetActive(false);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Shooting = false;
        }
        AmmunitionText.text = $"{TotalAmmunition}/{AmmunitionLeft}";
    }

    bool IsReloadFinish()
    {
        return DateTime.Now.CompareTo(StartReloading.AddMilliseconds(MiliSecondsDurationReloading)) > 0;
    }

    void Reload()
    {
        AmmunitionLeft = TotalAmmunition;
    }

    void EmptyGun()
    {
        if (AmmunitionLeft == 0 && (DateTime.Now.CompareTo(LastPlayEmptySound.AddMilliseconds(Rate)) > 0) && !IsReloading)
        {
            LastPlayEmptySound = DateTime.Now;
            GunSound.Empty.Play();
        }
    }

    void StartReload()
    {
        StartReloading = DateTime.Now;
        IsReloading = true;
        GunSound.Reload.Play();
    }

    bool aimHitSomething()
    {
       return Physics.Raycast(
            Camera.transform.position,
            Camera.transform.forward,
            out Hit,
            100,
            -1,
            QueryTriggerInteraction.Ignore
            );
    }

    void BulletHud()
    {
        var percentAmmunition = AmmunitionLeft/TotalAmmunition*100;
        if (percentAmmunition < FirstBulletPercent)
        {
            FirstBullet.GetComponent<Image>().enabled = false;
        }

        if (percentAmmunition > FirstBulletPercent)
        {
            FirstBullet.GetComponent<Image>().enabled = true;
        }

        if (percentAmmunition < MiddleBulletPercent)
        {
            MiddleBullet.GetComponent<Image>().enabled = false;
        }

        if (percentAmmunition > MiddleBulletPercent)
        {
            MiddleBullet.GetComponent<Image>().enabled = true;
        }

        if (AmmunitionLeft == 0)
        {
            LastBullet.GetComponent<Image>().enabled = false;
        }

        if (AmmunitionLeft != 0)
        {
            LastBullet.GetComponent<Image>().enabled = true;
        }
    }

    bool canShoot()
    {
        return (AmmunitionLeft > 0) && (DateTime.Now.CompareTo(LastShot.AddMilliseconds(Rate)) > 0) && !IsReloading;
    }
}
