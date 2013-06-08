using UnityEngine;
using System.Collections;

public class playerControl : MonoBehaviour {
	
	//Objects
	public GameObject cube;
	public Camera camera1;
	public GameObject bulb;
	private GameObject clone;
	
	#region Camera Control
		public float cameraSpeed;//How fast does the cam rotate
		public float minCameraX;//Max camera vertical rotation
		public float maxCameraX;//Min camera vertical rotation
		private float cameraRotationX = 0;//What is the camera's rotation right now (relative to 0)
		private float yRot;
        private float zRot;
	#endregion
	
	#region Character Control
		//Movement
		public int moveSpeed;
		public int rotateSpeed;
		private Vector3 moveDirection;
	
		//Jumping
		public int jumpSpeed;
		public int gravity;
		private bool isGrounded;
	
		//Throwing
		public int heldBulbs;
		public float throwSpeed;
		private bool isAiming = false;
	#endregion
	
	// Use this for initialization
	void Start () 
	{
		Physics.IgnoreCollision(clone.collider, this.collider);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		CameraControl();
		PlayerControl();
		if (isGrounded) 
			Grounded();
		else
			moveDirection.y -= gravity * Time.deltaTime;
		transform.Translate(moveDirection*Time.deltaTime);
	}
	
	void CameraControl()//Controls camera view
	{
		yRot = Input.GetAxis("Mouse X");
		
		//Rotate Player Controller
		transform.Rotate(new Vector3(0,Time.deltaTime*yRot*rotateSpeed,0));
		transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
		
		//Rotate Player Model
		cube.transform.Rotate(new Vector3(0,Time.deltaTime*yRot*rotateSpeed,0));
		cube.transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
		
		CameraMinMax();//Vertical axis controls
	}
	
	void CameraMinMax()//Correct vertical camera to stay within bounds
	{
		if(Input.GetAxis("Mouse Y")>.5)
			zRot=.5f;
		else if(Input.GetAxis("Mouse Y")<-.5)
			zRot=-.5f;
		else
			zRot = Input.GetAxis("Mouse Y");
		
		if(cameraRotationX >= minCameraX && cameraRotationX <= maxCameraX)
			{
				cameraRotationX += zRot;
				camera1.transform.Rotate(new Vector3(Time.deltaTime*zRot*-rotateSpeed,0,0));
			}
		if(cameraRotationX < minCameraX)//if lower than min rotation, correct
			{
				camera1.transform.Rotate(new Vector3(Time.deltaTime*.6f*-rotateSpeed,0,0));
				cameraRotationX += Time.deltaTime*-.3f*-rotateSpeed;
			}
		if(cameraRotationX > maxCameraX)//if higher than max rotation, correct
			{
				cameraRotationX += Time.deltaTime*.3f*-rotateSpeed;
				camera1.transform.Rotate(new Vector3(Time.deltaTime*-.6f*-rotateSpeed,0,0));
			}	
	}
	
	void PlayerControl()//Control Player
	{
		if(!isAiming)
		{
			#region WASD
			if(Input.GetKey(KeyCode.W))
				transform.Translate(new Vector3(moveSpeed,0,0)*Time.deltaTime);
			
			if(Input.GetKey(KeyCode.S))
				transform.Translate(new Vector3(-moveSpeed*.8f,0,0)*Time.deltaTime);
			
			if(Input.GetKey(KeyCode.A))
				transform.Translate(new Vector3(0,0,moveSpeed*.7f)*Time.deltaTime);	
			
			if(Input.GetKey(KeyCode.D))
				transform.Translate(new Vector3(0,0,-moveSpeed*.7f)*Time.deltaTime);
			#endregion
		}
		
		#region Mouse
		if(Input.GetMouseButton(0))
		{
			if(heldBulbs > 0)
			{
				//Stop moving
				isAiming = true;
				//Draw decal	
			}
			else
			{
				//Display message saying no plants held
				//Play error sound
			}
		}
		
		if(Input.GetMouseButtonUp(0))
		{
			if(heldBulbs > 0)
			{
				isAiming = false;
				//Shoot Projectile
				ThrowBulb();
				
				//Start Moving Again
				
				//Play soun
			}
		}
		#endregion
	}
	
	void ThrowBulb()
	{
		clone = Instantiate(bulb,transform.position + Vector3.forward,transform.rotation) as GameObject;
		clone.rigidbody.velocity = transform.TransformDirection(Vector3.right*throwSpeed);
	}
	
	void Grounded() //When on the ground
	{
		print("Is Grounded");
        moveDirection = new Vector3(0, 0, 0);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= moveSpeed;
        if (Input.GetKey(KeyCode.Space))
            moveDirection.y = jumpSpeed;
	}
	
	#region COLLISIONS
	void OnTriggerEnter(Collider other)//On collision with stuff
	{
		if(other.gameObject.tag == "Ground")//if the ground
			isGrounded = true;
		
		//Wall Collisions
		//Object Collisions
	}
	
	void OnTriggerExit(Collider other)//If leaving collision with stuff
	{
		if(other.gameObject.tag == "Ground")
			isGrounded = false;
	}	
	#endregion
}
