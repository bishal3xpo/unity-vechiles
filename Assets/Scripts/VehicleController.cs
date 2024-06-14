using System;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public Color materialColor;
    private Rigidbody m_rigidbody;

    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string jumpButton = "Jump";

    private float inputHorizontal;
    private float inputVertical;

    [SerializeField] float horizontalSpeed;
   

    void Update()
    {
        inputHorizontal = SimpleInput.GetAxis(horizontalAxis);
        inputVertical = SimpleInput.GetAxis(verticalAxis);

        transform.Rotate(0f, inputHorizontal * 0.1f, 0f, Space.World);
    }

    void LateUpdate()
    {
        transform.Translate(new Vector3(0f, 0f, inputVertical) * horizontalSpeed * Time.deltaTime);
    }


    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.75f);
    }
}
