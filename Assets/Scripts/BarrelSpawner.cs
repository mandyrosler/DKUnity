using UnityEngine;
using System.Collections;

public class BarrelSpawner : MonoBehaviour {

	Animator anim;
	Transform barrelSpawnPos;

	void Start () {

		anim = gameObject.GetComponent<Animator>();
		barrelSpawnPos = gameObject.transform.FindChild("BarrelSpawnPos");
		StartCoroutine(SpawnBarrel(1.0f));
	}
	
	IEnumerator SpawnBarrel(float delay) {

		yield return new WaitForSeconds(delay);

		anim.SetTrigger("ThrowBarrel");
		StartCoroutine(SpawnBarrel(6.0f));
	}

	public void ReleaseBarrel() {

		GameObject barrel = Instantiate(Resources.Load ("BarrelPrefab")) as GameObject;
		barrel.transform.parent = transform;
		barrel.transform.localPosition = barrelSpawnPos.localPosition;
		barrel.name = "Barrel";
	}

	public void Stop() {
		anim.StopPlayback();
		Destroy (this);
	}
}
