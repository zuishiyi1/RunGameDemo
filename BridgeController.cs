using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour {

	public int tid;
	public GameObject tubex;
	public GameObject tubey;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Player") {
			if (this.tid == 2) {
				
			}
		}
	}
}
