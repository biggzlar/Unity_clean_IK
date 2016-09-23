using UnityEngine;
using System.Collections;

public class Breathe : MonoBehaviour {
	Vector3 endPos, newPos;
	
	public float amplitude;
	public float period;
	public float smooth;

	private float distance;
	float theta;

	public Vector3 moveDir;
	Vector3 heightOffset;
	public Transform target;
	
	void Awake() {
		heightOffset = new Vector3 (0, 0);
	}
	
	void FixedUpdate() {
		endPos = target.position + heightOffset;
		//Vector3 movement = GameObject.FindWithTag ("Player").GetComponent<PlayerMovement> ().movement;
		theta = Time.deltaTime / period;
		distance = amplitude * Mathf.Sin(theta);
		moveDir = endPos - transform.position;
		/*newPos = Vector3.Lerp (transform.position, endPos, smooth * Time.deltaTime);
		transform.position = newPos;*/
		//if (moveDir.magnitude >= 0.01f && GameObject.FindWithTag ("Player").GetComponent<PlayerMovement> ().orbit) {
			transform.position += moveDir * distance;
		/*} else {
			newPos = Vector3.Lerp (transform.position, endPos, smooth * Time.deltaTime);
			transform.position = newPos;
		}
		if (moveDir.magnitude <= 0.01f) {
			transform.position = endPos;
		}*/
	}
}