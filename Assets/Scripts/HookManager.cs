using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HookManager : NetworkBehaviour {

	[SyncVar]
	public GameObject player;
	[SyncVar]
	public bool shoot;

	private bool collided;
	private float distance;
	private bool oneTime = true;
	public ParticleSystem steamParticle;

	public void Start () {
		transform.parent = null;
		shoot = true;
		collided = false;
	}

	void Update () {
		if (player == null) {
			DestroyThis ();
		}

		if (shoot) {
			UseHook ();
		} else {
			RecoilHook ();
		}
	
		UpdateHook ();
	}
		
	void UpdateHook(){
		GetComponent<LineRenderer> ().SetPosition (0, player.transform.position);
		GetComponent<LineRenderer> ().SetPosition (1, transform.position);
		distance = Vector3.Distance (transform.position, player.transform.position);
	}
		
	void UseHook(){
		if (!collided) {
			transform.Translate (0, 0, 65 * Time.deltaTime);
		} else {
			if (oneTime) {
				oneTime = false;
			}
			player.GetComponent<SpringJoint> ().spring = 5;
			player.GetComponent<SpringJoint> ().damper = 0.1f;
			player.GetComponent<SpringJoint> ().maxDistance = 4;
			player.GetComponent<SpringJoint> ().tolerance = 0.025f;
			player.GetComponent<SpringJoint> ().connectedBody = GetComponent<Rigidbody> ();

			if (player.GetComponent<NetworkIdentity> ().isLocalPlayer) {
				if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.D)) {
					player.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 0.1f, 0), ForceMode.Impulse);
					player.GetComponent<PlayerMovement> ().CmdUseSteamParticle ();
				}
			}
		}
	}
		
	void RecoilHook(){
		transform.position = Vector3.MoveTowards (transform.position, player.transform.position, 45 * Time.deltaTime);

		if (distance <= 2) {
			player.GetComponent<SpringJoint> ().spring = 0;
			player.GetComponent<SpringJoint> ().damper = 0;
			player.GetComponent<SpringJoint> ().maxDistance = 0;
			player.GetComponent<SpringJoint> ().tolerance = 0;
			player.GetComponent<SpringJoint> ().connectedBody = null;
			DestroyThis ();
		}
	}
		
	void DestroyThis(){
		Destroy (gameObject);
	}

	void OnTriggerEnter(Collider coll){
		if (coll.tag != "Player" && coll.tag != "Hook") {
			collided = true;
		}
	}
}
