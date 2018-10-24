using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {


	private ParticleSystem ps;


	// Use this for initialization
	void Start () {
		ps = this.GetComponent<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ps) {
			if (ps.isStopped) {
				if (this.gameObject.name != "GroundLight") {
					Destroy (this.gameObject);
				}
			}
		}
	}

	public void DestoryThis()
	{
		Destroy (this.gameObject);
	}
}
