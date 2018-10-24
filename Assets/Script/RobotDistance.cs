using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDistance : MonoBehaviour {


	public GameObject robot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player") {
			robot.SendMessage ("GoFaster");
		}
	}
}
