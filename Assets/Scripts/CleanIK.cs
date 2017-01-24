using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CleanIK : MonoBehaviour {

	protected Animator animator;

	public bool ikActive = false;
	public bool rotation = false;
	//turn on gizmos in playmode to check the linecasts
	public bool showMarkers = false;

	public Transform LeftFoot = null;
	public Transform RightFoot = null;
	public float footOffset;

	//length of the linecast
	float legDistance;
	//
	int layerMask = 1 << 8;
	CharacterController controller;
	NavMeshAgent agent;

	float LeftFootY, RightFootY;
	private float colliderHeight, controllerBoundsBottom;
	public float smooth = 10f;
	public float deltaAmplifier = 1f;

	void Start ()
	{
		animator = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();
		agent = GetComponent<NavMeshAgent> ();

		//hit all layers but the players layer
		layerMask = ~layerMask;
		colliderHeight = controller.height;
		//controllerBoundsBottom = controller.bounds.extents.y;
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
					solveIK (ref LeftFoot);
				}

				if(RightFoot != null) {
					solveIK (ref RightFoot);
				}
			}
		}
	}

	private void solveIK(ref Transform foot)
	{
		String footName = foot.name;
		RaycastHit floorHit = new RaycastHit ();

		Vector3 newPosition = new Vector3 ();
		Quaternion newRotation = Quaternion.identity;

		if (Physics.Linecast (checkOrigin (foot.position), checkTarget (foot.position), out floorHit, layerMask)) {
			newPosition = footPosition (floorHit);
			newRotation = footRotation (foot, floorHit);

			if(String.Equals(footName, LeftFoot.name)) {
				animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f);
				animator.SetIKPosition(AvatarIKGoal.LeftFoot, newPosition);

				LeftFootY = newPosition.y;

				if (rotation) {
					animator.SetIKRotationWeight (AvatarIKGoal.LeftFoot, 1f);
					animator.SetIKRotation (AvatarIKGoal.LeftFoot, newRotation);
				}
			}

			if(String.Equals(footName, RightFoot.name)) {
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

		if (planeSpeed (ref controller) < 0.1f) {
			float delta = Mathf.Abs (LeftFootY - RightFootY);
			controller.height = colliderHeight - delta * deltaAmplifier;
			//controller.center = new Vector3(0, Mathf.Lerp(controller.center.y, colliderCenterY + delta, Time.deltaTime * smooth), 0);//new Vector3 (0, colliderCenterY + delta, 0);
		} else {
			controller.height = colliderHeight;
			//controller.center = new Vector3 (0, colliderCenterY, 0);
		}
	}

	private void stateBasedLegDistance()
	{
		if (controller) {
			legDistance = (1 / (planeSpeed (ref controller) + 0.8f));
		} else {
			legDistance = (1 / (planeSpeed (ref agent) + 0.8f));
		}
	}


	private float planeSpeed(ref CharacterController characterController)
	{
		Vector3 planeSpeed = new Vector3 (characterController.velocity.x, 0, characterController.velocity.z);
		return planeSpeed.magnitude;
	}

	private float planeSpeed(ref NavMeshAgent navAgent)
	{
		Vector3 planeSpeed = new Vector3 (navAgent.velocity.x, 0, navAgent.velocity.z);
		return planeSpeed.magnitude;
	}

	private Quaternion footRotation(Transform foot, RaycastHit hit)
	{
		Quaternion footRotation = Quaternion.LookRotation( Vector3.ProjectOnPlane(transform.forward, hit.normal), hit.normal );
		return footRotation;
	}

	private Vector3 footPosition(RaycastHit hit)
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
