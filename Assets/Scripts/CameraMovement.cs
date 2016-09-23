using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
	public float offsetPlayer = 6.0f; 

	public Transform targetObject;
	PlayerMovement playerMovement;
	private Vector3 offsetComplete, heightOffset;
	public float xSpeed = 1f;
	public float ySpeed = 1f;

	//The position of the cursor on the screen. Used to rotate the camera.
	private float x, y;

	float tY = 0f;
	Vector3 targetPos;
	Quaternion newRotation, wantedRotation;

	Vector3 wallOffset;

	#region setup
	void Awake(){
		playerMovement = GameObject.FindWithTag ("Player").GetComponent<PlayerMovement> ();
	}
	
	void Start()
	{
		offsetComplete = (Vector3.forward * offsetPlayer);
		wallOffset = Vector3.forward * 0.3f;
		heightOffset = new Vector3 (0, 1);
	}

	void Update()
	{
		x = Input.GetAxis("Mouse X") * xSpeed;
		y = Input.GetAxis ("Mouse Y") * ySpeed;

		if (targetObject) {
			this.SmoothLookAt (x, y);
		}
	}

	#endregion

	#region methods
	void SmoothLookAt (float _x, float _y) 
	{
		tY = Mathf.Clamp (tY + _y, -36, 36);

		if (playerMovement!= null) {
			if (playerMovement.orbit) {
				newRotation = Quaternion.LookRotation (targetObject.position - transform.position);
				wantedRotation = Quaternion.Euler (tY, newRotation.eulerAngles.y + _x, 0);
			}
			if (!playerMovement.orbit) {
				wantedRotation = Quaternion.Euler (tY, transform.eulerAngles.y + _x, 0);
			}
		}
		//if (playerMovement.target != null) {
		//	newRotation = Quaternion.LookRotation (playerMovement.target.position - targetObject.position, Vector3.up);
		//	wantedRotation = Quaternion.Euler (Mathf.Clamp(newRotation.eulerAngles.x, 36, 36), newRotation.eulerAngles.y, 0);
		//}
		
		transform.rotation =  wantedRotation;
		targetPos = (targetObject.transform.position) - transform.rotation * offsetComplete + heightOffset;
		CompensateForWalls (targetObject.transform.position + heightOffset, ref targetPos);
		transform.position = targetPos;
	}

	private void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget){
		RaycastHit wallHit = new RaycastHit ();
		int layerMask = 1 << 8;
		layerMask = ~layerMask;
		if (Physics.Linecast (fromObject, toTarget, out wallHit, layerMask)) {
			Debug.DrawRay (wallHit.point, Vector3.left, Color.red);
			toTarget = new Vector3 (wallHit.point.x, wallHit.point.y, wallHit.point.z) + wallOffset;
		}
	}
	#endregion
}