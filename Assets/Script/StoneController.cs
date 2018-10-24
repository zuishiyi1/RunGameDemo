using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneController : MonoBehaviour {

	private bool hitted = false;

	private float ySpeed = 0;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (hitted) {
			ySpeed += Time.deltaTime * 0.4f;
			this.transform.position = this.transform.position + new Vector3(0,ySpeed,-0.5f);
			this.transform.Rotate(15,15,15,Space.Self);
		}
	}

	public void Hitted()
	{
		hitted = true;
	}

	public bool GetHitted()
	{
		return hitted;
	}
}
