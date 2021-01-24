using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMouse : MonoBehaviour
{
    // Eixo
    public enum RotationAxis { MouseX, MouseY};

    public RotationAxis axis = RotationAxis.MouseY;

    // Mouse

    private float actualSensibilityX = 1f;

    private float actualSensibilityY = 1f;

    private float sensibilityX = 1f;

    private float sensibilityY = 1f;

    private float mouseSensibility = 1f;

    // Rotação

    private float rotationX, rotationY;

    private float minX = -360f;

    private float maxX = 360f;

    private float minY = -60f;

    private float maxY = 60f;

    private bool isKeyEscapePressed = false;

    // Base

    private Quaternion originalRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        originalRotation = transform.rotation;
        Debug.Log("Start FPS Mouse");
    }

    void LateUpdate()
    {
        MouseChange();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !isKeyEscapePressed)
        {
            isKeyEscapePressed = true;
            Cursor.visible = !Cursor.visible;
            Debug.Log($"Apertou! Cursos visivel1: {Cursor.visible}");
        }

        if (Input.GetKeyUp(KeyCode.Escape) && isKeyEscapePressed)
        {
            isKeyEscapePressed = false;
            Debug.Log($"Soltou! Cursos visivel1: {Cursor.visible}");
        }
    }

    float LimitAngle(float angle, float min, float max)
    {
        if (angle < min)
        {
            angle += 360;
        }

        if (angle > max)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }

    void MouseChange()
    {
        if (actualSensibilityX != sensibilityX || actualSensibilityY != sensibilityY)
        {
            actualSensibilityX = sensibilityY = mouseSensibility;
        }

        sensibilityX = actualSensibilityX;
        sensibilityY = actualSensibilityY;

        if (axis == RotationAxis.MouseX)
        {
            rotationX += Input.GetAxis("Mouse X") * sensibilityX;
            rotationX = LimitAngle(rotationX, minX, maxX);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = xQuaternion * originalRotation;
        }

        if (axis == RotationAxis.MouseY)
        {
            rotationY += Input.GetAxis("Mouse Y") * sensibilityY;
            rotationY = LimitAngle(rotationY, minY, maxY);
            Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
            transform.localRotation = yQuaternion * originalRotation;
        }
    }
}
