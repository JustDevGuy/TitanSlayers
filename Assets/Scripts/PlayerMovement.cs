using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

	//Camera Movement//
	public float mouseX, mouseY;
	private float refX, refY;
	private float smoothMouseX, smoothMouseY;

	//Player Movement//
	private float timeInAir;

	//DMT Movement//
	public GameObject hook;
	public Texture sightWhite, sightRed;
	private bool inRange;
	private int dmtNumber;
	private RaycastHit hit;
	private GameObject hookInstance;

	//Flare Gun//
	public GameObject flare;
	private GameObject flareInstance;

	//Initialize some variables and stuffs//
	void Start () {
		gameObject.name = "Player " + netId;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Camera.main.GetComponent<SwayScript> ().startObject (Camera.main.transform.GetChild (0));
	}

	//All the functions working down here//
	void Update () {
		if (!isLocalPlayer) {
			return;
		}

		if (transform.position.y <= -50) {
			transform.position = new Vector3 (transform.position.x, 30, transform.position.z);
		}

		if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit, 80)) {
			inRange = true;
		} else {
			inRange = false;
		}

		moveCamera (180);
		movePlayer (15, 14);

		if (Input.GetKeyDown (KeyCode.E) && inRange) {
			CmdSpawnHook (netId, hit.point);
		}

		if (Input.GetKeyUp (KeyCode.E)) {
			CmdDestroyHook ();
		}

		if (Input.GetMouseButtonDown (0) && Camera.main.GetComponent<DynamicInventory> ().atualItem == 2) {
			CmdSpawnFlare (netId, hit.point);
		}

		Camera.main.GetComponent<SwayScript> ().swayObject (Camera.main.transform.GetChild (0));
		Camera.main.GetComponent<AttackSystem> ().UseSwords (Camera.main.transform.GetChild(1).GetChild(0).gameObject,Camera.main.transform.GetChild(2).GetChild(0).gameObject);
	}

	//Spawn and show the hook for everyone in the server//
	[Command]
	void CmdSpawnHook(NetworkInstanceId playerID, Vector3 hitPoint){
		hookInstance = Instantiate (hook, transform.position, Quaternion.LookRotation(-(transform.position - hitPoint))) as GameObject;
		hookInstance.GetComponent<HookManager> ().player = GameObject.Find ("Player " + playerID).gameObject;
		NetworkServer.Spawn (hookInstance);
	}

	//Spawn and show the flare bullet for everyone in the server//
	[Command]
	void CmdSpawnFlare(NetworkInstanceId playerID, Vector3 hitPoint){
		flareInstance = Instantiate (flare, transform.position, Quaternion.LookRotation(-(transform.position - hitPoint))) as GameObject;
		flareInstance.transform.LookAt (hit.point);
		flareInstance.GetComponent<Rigidbody> ().AddForce (Camera.main.transform.forward * 30, ForceMode.Impulse);
		NetworkServer.Spawn (flareInstance);
		Destroy (flareInstance, 15);
	}

	//Destroy Hook
	[Command]
	void CmdDestroyHook () {
		hookInstance.GetComponent<HookManager> ().shoot = false;
		hookInstance = null;
	}

	//Move the player with the rigidbody//
	void movePlayer(float velocity, float maxVelocity){
		Vector3 movement = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		movement = transform.TransformDirection (movement);
		movement *= velocity;

		Vector3 velocityChange = (movement - GetComponent<Rigidbody>().velocity);
		velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocity, maxVelocity);
		velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocity, maxVelocity);
		velocityChange.y = 0;
		GetComponent<Rigidbody> ().AddForce (velocityChange, ForceMode.Force);

		if (Physics.Raycast (transform.position, -transform.up, 1.5f)) {
			if (!Input.GetKey (KeyCode.W) && !Input.GetKey (KeyCode.A) && !Input.GetKey (KeyCode.S) && !Input.GetKey (KeyCode.D)) {
				GetComponent<Rigidbody> ().velocity = Vector3.Slerp (GetComponent<Rigidbody> ().velocity, new Vector3(0,GetComponent<Rigidbody> ().velocity.y,0), 8 * Time.deltaTime);
			} 

			timeInAir = 0;
			if (Input.GetKeyDown (KeyCode.Space)) {
				GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 5, 0), ForceMode.Impulse);
			}
		} else {
			if (Input.GetKey (KeyCode.Space) && timeInAir <= 2) {
				timeInAir += Time.deltaTime;
				GetComponent<Rigidbody> ().AddForce (new Vector3(0, 0.2f, 0), ForceMode.Impulse);
				CmdUseSteamParticle ();
			}
		}
	}

	//Move the camera//
	void moveCamera(float sensivity){
		mouseX -= Input.GetAxis ("Mouse Y") * sensivity * Time.deltaTime;
		mouseY += Input.GetAxis ("Mouse X") * sensivity * Time.deltaTime;
		mouseX = Mathf.Clamp (mouseX, -90, 90);

		smoothMouseX = Mathf.SmoothDamp (smoothMouseX, mouseX, ref refX, 3 * Time.deltaTime);
		smoothMouseY = Mathf.SmoothDamp (smoothMouseY, mouseY, ref refY, 3 * Time.deltaTime);

		Camera.main.transform.position = new Vector3 (transform.position.x, transform.position.y + 0.8f, transform.position.z);
		transform.rotation = Quaternion.Euler (0, smoothMouseY, 0);
		Camera.main.transform.rotation = Quaternion.Euler (smoothMouseX, smoothMouseY, -Input.GetAxis("Mouse X"));
	}
		
	//Show the sight system for the player//
	void OnGUI(){
		if (!isLocalPlayer) {
			return;
		}

		if (inRange) {
			GUI.DrawTexture (new Rect (Screen.width / 2, Screen.height / 2, 11, 11), sightWhite);
		} else {
			GUI.DrawTexture (new Rect (Screen.width / 2, Screen.height / 2, 11, 11), sightRed);
		}
	}

	//Show the steam particle for everyone in the server//
	[Command]
	public void CmdUseSteamParticle(){
		GetComponent<ParticleSystem> ().Emit (1);
	}
}
