using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayScript : MonoBehaviour {

	private float xSwayAmount = 0.1f;
	private float ySwayAmount = 0.05f;
	private float maxXAmount = 0.35f;
	private float maxYAmount = 0.2f;
	private Vector3 amount = Vector3.zero;

	public void startObject(Transform cameraObject){
		amount = cameraObject.transform.localPosition;
	}

	public void swayObject(Transform cameraObject){
		float fx = Input.GetAxis ("Mouse X") * xSwayAmount;
		float fy = Input.GetAxis ("Mouse Y") * ySwayAmount;

		if (fx > maxXAmount)
			fx = maxXAmount;
		if (fx < -maxXAmount)
			fx = -maxXAmount;
		if (fy > maxYAmount)
			fy = maxYAmount;
		if (fy < -maxYAmount)
			fy = -maxYAmount;

		Vector3 newAmount = new Vector3 (amount.x + fx, amount.y + fy, amount.z);
		cameraObject.localPosition = Vector3.Lerp (cameraObject.localPosition, newAmount, 3 * Time.deltaTime);
	}

}
