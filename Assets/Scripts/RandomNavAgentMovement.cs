// ClickToMove.cs
using UnityEngine;

[RequireComponent (typeof (NavMeshAgent))]
public class RandomNavAgentMovement : MonoBehaviour {
	NavMeshAgent agent;
	public float roamRadius;

	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}

	void Update () {
		freeRoam (agent);
	}

	public void freeRoam(NavMeshAgent _nav)
	{
		if (_nav.enabled == true) {
			Vector3 startPosition = _nav.transform.position;
			Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
			Vector3 finalPosition;
			randomDirection += startPosition;
			NavMeshHit hit;
			NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
			finalPosition = hit.position; 
			if (_nav.remainingDistance < 2f) {
				_nav.destination = finalPosition;
			}
		}
	}
}
