using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareBullet : MonoBehaviour {

	private bool use = true;

	void Update(){
		if (use) {
			GetComponent<ParticleSystem> ().Emit (1);
		}
	}

	void OnTriggerEnter(Collider coll){
		if (coll.tag != "Player" && coll.tag != "Hook" && coll.tag != "Flare Bullet") {
			Destroy (GetComponent<Rigidbody> ());
			use = false;
			GetComponent<ParticleSystem> ().Stop ();
		}
	}
}
