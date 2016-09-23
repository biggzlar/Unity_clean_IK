using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CleanIK : MonoBehaviour {

	protected Animator animator;

	public bool ikActive = false;
	public bool rotation = false;

	public Transform LeftFoot = null;
	public Transform RightFoot = null;
	public float footOffset;

	//turn on gizmos in playmode to check the linecasts
	public bool showMarkers = false;
	//length of the linecast
	float legDistance;

	int layerMask = 1 << 8;
	CharacterController controller;
	NavMeshAgent agent;

	float LeftFootY, RightFootY;
	float colliderCenterY;

	void Start () 
	{
		animator = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();
		agent = GetComponent<NavMeshAgent> ();

		//hit all layers but the players layer
		layerMask = ~layerMask;
		colliderCenterY = controller.center.y;
	}

	void Update()
	{
		handleColliderOffset();

		if (showMarkers) {
			Debug.DrawLine (checkOrigin (LeftFoot.position), checkTarget (LeftFoot.position), Color.green, 1f);
			Debug.DrawLine (checkOrigin (RightFoot.position), checkTarget (RightFoot.position), Color.green, 1f);
		}
	}
		
	void OnAnimatorIK()
	{
		if(animator) {

			if(ikActive) {
				
				if(LeftFoot != null) {
					solveIK (LeftFoot);
				}

				if(RightFoot != null) {
					solveIK (RightFoot);
				}
			}
		}
	}

	private void solveIK(Transform foot)
	{
		String footName = foot.name;
		RaycastHit floorHit = new RaycastHit ();

		Vector3 newPosition = new Vector3 ();
		Quaternion newRotation = Quaternion.identity;

		if (Physics.Linecast (checkOrigin (foot.position), checkTarget (foot.position), out floorHit, layerMask)) {
			newPosition = footPosition (foot, floorHit);
			newRotation = footRotation (foot, floorHit);

			if(String.Equals(footName, "LeftFoot")) {
				animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f);
				animator.SetIKPosition(AvatarIKGoal.LeftFoot, newPosition);

				LeftFootY = newPosition.y;

				if (rotation) {
					animator.SetIKRotationWeight (AvatarIKGoal.LeftFoot, 1f);
					animator.SetIKRotation (AvatarIKGoal.LeftFoot, newRotation);
				}
			}

			if(String.Equals(footName, "RightFoot")) {
				animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f);
				animator.SetIKPosition(AvatarIKGoal.RightFoot, newPosition);

				RightFootY = newPosition.y;

				if (rotation) {
					animator.SetIKRotationWeight (AvatarIKGoal.RightFoot, 1f);
					animator.SetIKRotation (AvatarIKGoal.RightFoot, newRotation);
				}
			}
		}
	}

	private void handleColliderOffset()
	{
		//this will change the length of the linecast based on the agents speed
		stateBasedLegDistance ();

		if (planeSpeed (controller) < 0.1f) {
			float delta = Mathf.Abs (LeftFootY - RightFootY);
			controller.center = new Vector3 (0, colliderCenterY + delta, 0);
		} else {
			controller.center = new Vector3 (0, colliderCenterY, 0);
		}
	}

	private void stateBasedLegDistance()
	{
		if (agent) {
			legDistance = (1 / (planeSpeed (agent) + 0.1f));
		} else {
			legDistance = (1 / (planeSpeed (controller) + 0.1f));
		}
	}


	private float planeSpeed(CharacterController characterController)
	{
		Vector3 planeSpeed = new Vector3 (characterController.velocity.x, 0, characterController.velocity.z);
		return planeSpeed.magnitude;
	}

	private float planeSpeed(NavMeshAgent navAgent) 
	{
		Vector3 planeSpeed = new Vector3 (navAgent.velocity.x, 0, navAgent.velocity.z);
		return planeSpeed.magnitude;
	}

	private Quaternion footRotation(Transform foot, RaycastHit hit)
	{
		Quaternion footRotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal );
		return footRotation;
	}

	private Vector3 footPosition(Transform foot, RaycastHit hit)
	{
		Vector3 displacement = hit.point;
		displacement.y += footOffset; 
		return displacement;
	}

	private Vector3 checkOrigin(Vector3 footPosition)
	{
		Vector3 origin = footPosition + ((legDistance + 0.25f) * Vector3.up);
		return origin;
	}

	private Vector3 checkTarget(Vector3 footPosition)
	{
		Vector3 target = footPosition - ((legDistance / 2f) * Vector3.up);
		return target;
	}
}