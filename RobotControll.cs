using UnityEngine;
using System;
using System.Collections;

public class RobotControll : MonoBehaviour
{

	private string text = "Run";
	private string textFire = "Start Fire";

	public Color teamColor = Color.red;

	public Transform gunFire1;
	public Transform gunFire2;

	public AnimationClip chasisIdle;
	public AnimationClip bodyIdle;
	public AnimationClip chasisRun;
	public AnimationClip bodyRun;

	public GameObject ChildToControll;

	private float nowmoveSpeed = 3.0f;
	//正式为6.0

	private bool normalrun = true;
	private bool backtorun = false;
	private bool hitbystone = false;

	private float fallPos;
	private float upPos;

	private int hitTimes = 0;

	private GameObject mainCamera;

	private GameObject[] players;
	private GameObject farestPlayer;

	private bool win;



	// Use this for initialization
	void Start ()
	{
		//SetTeamColor(transform);
		mainCamera = GameObject.Find ("Main Camera");
		if (mainCamera) {
			Invoke ("Shoot", UnityEngine.Random.Range (2.0f, 3.5f));
		}

		players = GameObject.FindGameObjectsWithTag ("Player");

		foreach (GameObject p in players) {
			p.SendMessage ("GetBoss", this.gameObject);
			farestPlayer = p;
		}

		win = false;
	}

	private void SetTeamColor (Transform trans)
	{
		if (trans.GetComponent<Renderer> () != null) {
			trans.GetComponent<Renderer> ().material.SetColor ("_DyeColor", teamColor);
		}

		for (int i = 0; i < trans.transform.childCount; i++) {

			if (trans.GetChild (i).GetComponent<Renderer> () != null) {
				trans.GetChild (i).GetComponent<Renderer> ().material.SetColor ("_DyeColor", teamColor);
			}

			if (trans.GetChild (i).childCount > 0) {
				SetTeamColor (trans.GetChild (i));
			}
		}
	}

	void Awake ()
	{


	}

	// Update is called once per frame
	void Update ()
	{
		if (this.transform.localScale.x <= 0.3f) {
			//结束游戏
			if (!win) {
				mainCamera.GetComponent<MainCameraController> ().Win ();
				win = true;
			}
		}
		if (hitTimes >= 3) {
			this.transform.localScale = Vector3.Lerp (this.transform.localScale, Vector3.zero, Time.deltaTime);
			this.transform.position = this.transform.position + Vector3.back * Time.deltaTime * 0.1f;
		} else {
			if (hitbystone) {
				this.transform.position = Vector3.Lerp (this.transform.position, new Vector3 (this.transform.position.x, fallPos - 3.0f, this.transform.position.z), Time.deltaTime * 2.0f);
			} else {
				if (backtorun) {
					this.transform.position = Vector3.Lerp (this.transform.position, new Vector3 (this.transform.position.x, upPos + 3.0f, this.transform.position.z), Time.deltaTime * 2.0f);
					if (Mathf.Abs (fallPos - this.transform.position.y) <= 0.2f) {
						normalrun = true;
						backtorun = false;
						GetComponent<Animation> ().Play (chasisRun.name);
						foreach (GameObject g in players) {
							if (g.GetComponent<PlayerController> ().nowmoveSpeed > farestPlayer.GetComponent<PlayerController> ().nowmoveSpeed) {
								farestPlayer = g;
							}
						}
						nowmoveSpeed += 4.0f;
					}
				}
				if (normalrun) {
					if (farestPlayer) {
						if (farestPlayer.transform.position.z - this.transform.position.z <= 13.0f) {
							if (farestPlayer.GetComponent<PlayerController> ().dead == false) {
								if (nowmoveSpeed > 4.0f) {
									this.nowmoveSpeed /= 2.0f;
								}
							}
						
						} else if (farestPlayer.transform.position.z - this.transform.position.z >= 30.0f) {
							this.nowmoveSpeed += 10.0f;
						}
						this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z + Time.deltaTime * nowmoveSpeed);
					}
				}
			}
		}
	}

	void PlayFxBundle0 ()
	{

	}

	void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.layer == 11 && (col.gameObject.GetComponentInParent<StoneController> ().GetHitted ())) {
			if (hitbystone) {
				//Invoke ("BackToRun", 8.0f);
			} else {
				Invoke ("BackToRun", 4.0f);
				fallPos = this.transform.position.y;
			}
			hitbystone = true;
			normalrun = false;
			gunFire1.gameObject.SetActive (false);
			gunFire2.gameObject.SetActive (false);
			hitTimes++;
			GetComponent<Animation> ().Play (chasisIdle.name);
			GameObject g = Instantiate (Resources.Load ("3D_Hit_02") as GameObject);
			g.transform.position = col.transform.position;
			g.GetComponent<ParticleSystem> ().Play ();
			Destroy (col.gameObject);
		}
	}

	void BackToRun ()
	{
		hitbystone = false;
		backtorun = true;
		normalrun = false;
		upPos = this.transform.position.y;
		//GetComponent<Animation>().Play(chasisRun.name);
	}

	void Shoot ()
	{
		if (hitTimes >= 3) {
			return;
		}
		if (!normalrun) {

		} else {
			if (gunFire1.gameObject.activeSelf) {
				gunFire1.gameObject.SetActive (false);
				gunFire2.gameObject.SetActive (false);		
			} else {
				gunFire1.gameObject.SetActive (true);
				gunFire2.gameObject.SetActive (true);

				//生成陨石效果
				mainCamera.SendMessage ("SetgroundeffectX");
				mainCamera.SendMessage ("CmdShootGroundEffect");
			}
		}
		Invoke ("Shoot", UnityEngine.Random.Range (2.0f, 3.5f));
	}

	public void GoFaster ()
	{
		foreach (GameObject g in players) {
			if (g.GetComponent<PlayerController> ().nowmoveSpeed > farestPlayer.GetComponent<PlayerController> ().nowmoveSpeed) {
				farestPlayer = g;
			}
		}
		this.nowmoveSpeed += 4.0f;

	}


}
