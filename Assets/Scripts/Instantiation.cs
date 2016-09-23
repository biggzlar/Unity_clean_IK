using UnityEngine;
using System.Collections;

public class Instantiation : MonoBehaviour {

	public GameObject ammo;
	public int numberOfShots;
	public float roamRadius = 20f;

	void Start() {
		for (int i = 0; i < numberOfShots; i++) {
			Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
			NavMeshHit hit;
			NavMesh.SamplePosition (randomDirection, out hit, roamRadius, 1);

			GameObject instance = GameObject.Instantiate (ammo);
			instance.transform.parent = this.transform;
			instance.transform.position = hit.position;
		}
	}
}
