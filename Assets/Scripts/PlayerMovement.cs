using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 8.0f;            // The speed that the player will move at
	public float jumpSpeed;
	public float gravity;
	float h,v;
	float cooldownTimer, evadeTimer;

	public float evadeTime; // this tells us how long the evade takes
	public float evadeDistance; // this tells us how far player will evade

	[HideInInspector] public Transform target;
	[SerializeField] public Transform pivotTransform;

	[HideInInspector] public Vector3 targetPos;
	Vector3 movement;

	Vector3 forward, right, evadeDirection;

	Quaternion newRotation, lockOnRotation;

	Animator anim;


	public bool orbit, backwards, evading, walking;
	public bool attacking;

	CharacterController controller;

    void Awake () {

		anim = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();

        // Set up references.
		targetPos = new Vector3 ();
    }

	void Update() 
	{
		// Store the input axes.
		h = Input.GetAxisRaw ("Horizontal");
		v = Input.GetAxisRaw ("Vertical");

		// Create a boolean that is true if either of the input axes is non-zero. For the AnimationController.cs
		walking = h != 0f || v != 0f;
		orbit = h != 0f;

		CalcMovement (h, v);

		ProcessEvasion ();

		anim.SetBool("move", walking);
		anim.SetFloat ("velx", h);
		anim.SetFloat ("vely", v);

		if(!controller.isGrounded){
			movement.y -= gravity;
		}

		controller.Move (movement * Time.deltaTime);
	}

	/*void OnAnimatorMove () {
		// Update postion to agent position
		//		transform.position = agent.nextPosition;

		// Update position based on animation movement using navigation surface height
		Vector3 position = anim.rootPosition;
		//position.y = agent.nextPosition.y;
		transform.position = position;
	}*/

    void CalcMovement (float h, float v) 
	{
		movement = new Vector3 ();

		// Normalise the movement vector and make it proportional to the speed per second.
		// "v *"/"h *" is supposed to make rotation stepless
		if (v != 0 || h != 0) {
			movement = v * pivotForward() + (h/2f) * pivotRight();
			//movement = movement.normalized;
		}

		HandleRotation ();
		movement *= speed;
    }

	void HandleRotation()
	{
		if (movement != Vector3.zero) {			
			if (!orbit && v > 0f) {
				newRotation = Quaternion.LookRotation (movement, Vector3.up);
				transform.rotation = newRotation;
			} else {
				newRotation = Quaternion.LookRotation (pivotForward (), Vector3.up);
				transform.rotation = newRotation;
			}
		}
	}

	void Evade() 
	{
		evadeDirection = -transform.forward.normalized;

		if (walking) {
			evadeDirection = movement.normalized;
		}

		if(!evading && Input.GetButtonDown("Fire2")) {
			evading = true;
			evadeTimer = evadeTime;
		}
	}

	void ProcessEvasion() 
	{
		if(evading) {
			evadeTimer = Mathf.Max(0f, evadeTimer - Time.deltaTime);
			controller.Move (evadeDirection * evadeDistance * Time.deltaTime);

			if(evadeTimer == 0) { 
				evading = false;
			}
		}
	}

	void LockOnEnemy() 
	{	
		//we will ONLY scan for new targets if current target is null
		if (target == null)
		{
			GameObject[] enemyList = GameObject.FindGameObjectsWithTag("LockOnTarget");
			if (enemyList.Length > 0)
			{
				int enemyID = -1; //default this to 'invalid' value (just good practice!)
				float closestEnemyDistanceSqr = 400f; //max lockon distance here (use float.MaxValue unless you have a good reason not to!)
				
				//find closest enemy (only change is I'm looking at squared magnitudes, as they do the job and are more efficient)
				for (int i = 0; i < enemyList.Length; i++)
				{
					float enemyDeltaSqr = (transform.position - enemyList[i].transform.position).sqrMagnitude;
					if (enemyDeltaSqr < closestEnemyDistanceSqr)
					{
						closestEnemyDistanceSqr = enemyDeltaSqr;
						enemyID = i;
					}
				}

				if( enemyID > -1){
					//store the target. we can safely assume enemyID is no longer -1, as we know there is at least 1 enemy in enemyList
					target = enemyList[enemyID].transform;
					Debug.Log("Found: Enemy( " + enemyID + ") at " + (closestEnemyDistanceSqr));
				}
			}
		}
		
		//if check is false (i.e. we didn't go through the above if statement), we clear 'target'.
		else {
			this.target = null;
		}
	}

	void Attacking() 
	{
		attacking = true;
	}

	void AttackDone() 
	{
		attacking = false;
	}

	Vector3 pivotForward() 
	{
		Vector3 forwardVector = pivotTransform.transform.forward;
		forwardVector.y = 0;
		return forwardVector;
	}

	Vector3 pivotRight() 
	{
		Vector3 rightVector = pivotTransform.transform.right;
		rightVector.y = 0;
		return rightVector;
	}

	Vector3 targetPosition() 
	{
		return targetPos;
	}
}