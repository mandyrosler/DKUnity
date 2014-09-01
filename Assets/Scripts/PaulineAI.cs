using UnityEngine;
using System.Collections;

public class PaulineAI : MonoBehaviour {

	private Animator anim;

	void Start () {
	
		anim = GetComponent<Animator>();
		StartCoroutine(PlayAnimation(2.0f));
	}

	IEnumerator PlayAnimation(float delay) {

		yield return new WaitForSeconds(delay);

		anim.SetTrigger("PaulineAnim");

		StartCoroutine(PlayAnimation(Random.Range(1.0f, 3.0f)));
	}
}
