using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicInventory : MonoBehaviour {

	public GameObject[] inventory;
	private bool changing;
	private float timeToBack;
	public int atualItem;

	//ID 1 > Blades
	//ID 2 > FlareGun

	void Update () {	
		if (changing == false) {	
			if (Input.GetKeyDown ("1")) {
				atualItem = 1;
				changing = true;
			} else if (Input.GetKeyDown ("2")) {
				atualItem = 2;
				changing = true;
			}
		} else {
			ChangingAnimation (atualItem);
		}
	}

	void ChangeItem (int inventoryID) {
		for (int i = 0; i < inventory.Length; i++) {
			inventory [i].SetActive (false);
		}

		if (inventoryID == 1) { //Default Blades
			inventory [0].SetActive (true);
			inventory [1].SetActive (true);
		} else if (inventoryID == 2) { //Flare Gun
			inventory [2].SetActive (true);
		}
	}

	void ChangingAnimation (int itemID) {
		timeToBack += Time.deltaTime;
		transform.GetChild (0).transform.localPosition = Vector3.Slerp (transform.GetChild (0).transform.localPosition, new Vector3 (0, 0, 0), 7 * Time.deltaTime);

		if (timeToBack >= 0.2f && timeToBack < 0.5f) {
			transform.GetChild (0).transform.localPosition = Vector3.Slerp (transform.GetChild (0).transform.localPosition, new Vector3 (0, 1, 0), 7 * Time.deltaTime);
		} else if (timeToBack >= 0.5f && timeToBack < 0.8f) {
			ChangeItem (itemID);
		} else if (timeToBack >= 0.8f) {
			changing = false;
			timeToBack = 0;
		}
	}
}
