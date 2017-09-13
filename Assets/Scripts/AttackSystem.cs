using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSystem : MonoBehaviour {

	public GameObject bladeDiscart;
	private bool reloading;
	private float cooldownAttack;
	private float cooldownReloadingTime;
	private bool cooldown = false;
	private bool cooldownReloading;
	private Animator anim;

	void Start () {
		anim = GetComponent<Animator> ();
		cooldownAttack = 1.3f;
		cooldownReloadingTime = 2.0f;
	}

	public bool getSwordReloading(){
		return reloading;
	}

	public void UseSwords (GameObject blade1, GameObject blade2) {
		if (Input.GetKeyDown (KeyCode.R) && cooldown == false && cooldownReloading == false) {
			reloading = true;

			GameObject instance1 = Instantiate (bladeDiscart, Camera.main.transform.GetChild(1).GetChild(0).gameObject.transform.position, Camera.main.transform.GetChild(1).GetChild(0).gameObject.transform.rotation) as GameObject;
			GameObject instance2 = Instantiate (bladeDiscart, Camera.main.transform.GetChild(2).GetChild(0).gameObject.transform.position, Camera.main.transform.GetChild(2).GetChild(0).gameObject.transform.rotation) as GameObject;
			Destroy (instance1, 10);
			Destroy (instance2, 10);

			blade1.SetActive (false);
			blade2.SetActive (false);

			anim.SetBool ("Reloading", true);
			cooldownReloading = true;
		} else {
			reloading = false;
		}

		if(cooldownReloading){
			cooldownReloadingTime -= Time.deltaTime;
			if (cooldownReloadingTime <= 1.3f) {
				anim.SetBool ("Reloading", false);
				blade1.SetActive (true);
				blade2.SetActive (true);
			}

			if(cooldownReloadingTime <= 0){
				cooldownReloadingTime = 2.0f;
				cooldownReloading = false;
			}
		}

		if (Input.GetMouseButtonDown (0) && cooldown == false) {
			cooldown = true;
			anim.SetInteger ("AttackType", Random.Range (1, 4));
			anim.SetBool ("Attack", true);
		}

		if(cooldown){
			cooldownAttack -= Time.deltaTime;
			if (cooldownAttack <= 0.7f) {
				anim.SetBool ("Attack", false);
				anim.SetInteger ("AttackType", 0);
			}

			if(cooldownAttack <= 0){
				cooldownAttack = 1.3f;
				cooldown = false;
			}
		}
	}
}
