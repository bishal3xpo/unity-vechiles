using UnityEngine;

public class PlayerControllerWithRigidBody : MonoBehaviour
{
	public Color materialColor;
	private Rigidbody vechileRigidBody;
	private Rigidbody targetForVCamRigidBody;

	public string horizontalAxis = "Horizontal";
	public string verticalAxis = "Vertical";
	public string jumpButton = "Jump";

	private float inputHorizontal;
	private float inputVertical;



	void Awake()
	{
		GetComponent<Renderer>().material.color = materialColor;
		vechileRigidBody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		inputHorizontal = SimpleInput.GetAxis(horizontalAxis);
		inputVertical = SimpleInput.GetAxis(verticalAxis);

		transform.Rotate(0f, inputHorizontal * 5f, 0f, Space.World);

		if (SimpleInput.GetButtonDown(jumpButton) && IsGrounded())
			vechileRigidBody.AddForce(0f, 10f, 0f, ForceMode.Impulse);
	}

	void FixedUpdate()
	{
		vechileRigidBody.AddRelativeForce( new Vector3( 0f, 0f, inputVertical ) * 20f );

	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
			vechileRigidBody.AddForce(collision.contacts[0].normal * 10f, ForceMode.Impulse);
	}

	bool IsGrounded()
	{
		return Physics.Raycast(transform.position, Vector3.down, 1.75f);
	}
}