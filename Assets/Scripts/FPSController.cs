using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{

    // Variaveis de Transform
    private Transform FPSView;  //Manipulação do personagem

    private Transform FPSCamera; //Manipulação da Câmera

    private Vector3 FPSRotation = Vector3.zero; //Rotacionar a Câmera

    // Variaveis de Velocidade
    public float speedWalking = 5.0f;

    public float speedRunning = 10.0f;

    public float speedJump = 8.0f;

    public float gravity = 20f;

    public float speed;

    // Inputs e Teclas
    private float inputX;

    private float inputY;

    private float inputXSet;

    private float inputYSet;

    private float InputFactor;

    private bool limitSpeedDiagonal;

    private float antiBump = 0.75f;

    // Variaveis Lógicas

    private bool inFloor;

    private bool moving;

    // outras
    private CharacterController charController;

    private Vector3 directionMovement = Vector3.zero;

    // Agachar, Pular & Correr

    public LayerMask FloorLayer;

    private float rayDistance;

    private float heightDefault;

    private Vector3 positionDefaultCam;

    private float heightCam;

    private bool isCrouched;

    private float speedCrouched = 3.5f;

    private Animator animator;
    //Sound
    private bool Executing;


    // Start is called before the first frame update
    void Start()
    {
        FPSView = transform.Find("Visao").transform;
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        speed = speedWalking;
        moving = false;

        rayDistance = charController.height * 0.5f + charController.radius;
        heightDefault = charController.height;
        positionDefaultCam = FPSView.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        processYMovement();
        processXMovement();

        if (inputYSet != 0f || inputXSet != 0f)
        {
            animator.SetBool("walkingForward", true);
        }
        else
        {
            animator.SetBool("walkingForward", false);
        }

        inputY = Mathf.Lerp(inputY, inputYSet, Time.deltaTime * 20f);
        inputX = Mathf.Lerp(inputX, inputXSet, Time.deltaTime * 20f);

        InputFactor = Mathf.Lerp(
            InputFactor,
            (inputYSet != 0 && inputXSet != 0 && limitSpeedDiagonal) ? 0.75f : 1.0f, 
            Time.deltaTime * 20f
            );

        //FPSRotation = Vector3.Lerp(FPSRotation, Vector3.zero, Time.deltaTime * 5f);
        //FPSView.localEulerAngles = FPSRotation;
        if (inFloor)
        {
            crounchesAndRuns();
            directionMovement = new Vector3(inputX * InputFactor, -antiBump , inputY * InputFactor);
            directionMovement = transform.TransformDirection(directionMovement) * speed;
            Jump();
        }

        directionMovement.y -= gravity * Time.deltaTime;

        inFloor = (charController.Move(directionMovement * Time.deltaTime) & CollisionFlags.Below) != 0;
        moving = (charController.velocity.magnitude > 0.15f);
        if (moving)
        {
            PlayMovementSound();
        }
    }

    void processYMovement()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
            {
                inputYSet = 1f;
                return;
            }
            inputYSet = -1f;
            return;
        }
        inputYSet = 0f;
        return;
    }

    void processXMovement()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.D))
            {
                inputXSet = 1f;
                return;
            }
            inputXSet = -1f;
            return;
        }
        inputXSet = 0f;
    }

    void crounchesAndRuns()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!isCrouched)
            {
                isCrouched = true;
            }
            else
            {
                if (canStandUp())
                {
                    isCrouched = false;
                }
            }
        }

        StopCoroutine(MoveCamCrouched());
        StartCoroutine(MoveCamCrouched());

        if (isCrouched)
        {
            speed = speedCrouched;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                animator.SetBool("running", true);
                speed = speedRunning;
            }
            else
            {
                speed = speedWalking;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                animator.SetBool("running", false);
            }
        }
    }

    bool canStandUp()
    {
        Ray rayTop = new Ray(transform.position, transform.up);
        RaycastHit rayTopHit;

        if (Physics.SphereCast(rayTop, charController.radius + 0.05f, out rayTopHit, rayDistance, FloorLayer))
        {
            if (Vector3.Distance(transform.position, rayTopHit.point) < 2.3f)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator MoveCamCrouched()
    {
        charController.height = isCrouched ? heightDefault / 1.5f : heightDefault;
        charController.center = new Vector3(0f, charController.height/2f, 0f);
        heightCam = isCrouched ? positionDefaultCam.y / 1.5f : positionDefaultCam.y;

        while (Math.Abs(heightCam - FPSView.localPosition.y) > 0.01f)
        {
            FPSView.localPosition = Vector3.Lerp(
                FPSView.localPosition, 
                new Vector3(positionDefaultCam.x, heightCam, positionDefaultCam.z), Time.deltaTime*11f);
        }

        yield return null;
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (isCrouched)
            {
                if (canStandUp())
                {
                    isCrouched = false;
                    StopCoroutine(MoveCamCrouched());
                    StartCoroutine(MoveCamCrouched());
                }
            }
            else
            {
                animator.SetBool("jumping", true);
                directionMovement.y = speedJump;
            }

            return;
        }
        animator.SetBool("jumping", false);
    }

    void PlayMovementSound()
    {
        if (!Executing)
        {
            var audios = GetComponents<AudioSource>();
            Executing = true;
            foreach (var audio in audios)
            {
                // audio.Play();
            }
            Executing = false;

        }
    }
}
