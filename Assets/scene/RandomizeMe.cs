using UnityEngine;
using System.Collections;

public class RandomizeMe : MonoBehaviour {
	Color[] colors = new Color[6];

	SkinnedMeshRenderer myRenderer;

	public GameObject Mesh;
	public bool randomize;

	void Awake()
	{
		colors[0] = Color.cyan;
		colors[1] = Color.red;
		colors[2] = Color.green;
		colors[3] = new Color(255, 165, 1, 0.5f);
		colors[4] = Color.yellow;
		colors[5] = Color.magenta;

		myRenderer = Mesh.GetComponent<SkinnedMeshRenderer> ();
		myRenderer.material.color = colors[Random.Range(0, colors.Length)];

		randomize = (Random.value > 0.5f);
	}

	void Update()
	{
		if(randomize)
			myRenderer.material.color = colors[Random.Range(0, colors.Length)];
	}
}
