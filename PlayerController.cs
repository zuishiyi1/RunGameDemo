
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerController : NetworkBehaviour
{


	private MainCameraController MainCamera;

	public SkinnedMeshRenderer smr;
	public GameObject runningEffect;
	public ParticleSystem stoneEffect;


	private Vector3 spawnPos1 = new Vector3 (8.5f, 3.96f, -44.43f);
	private Vector3 spawnPos2 = new Vector3 (2.5f, 3.96f, -44.43f);
	private float spawnZ = -44.43f;

	public GameObject[] tubes = new GameObject[4];
	private float tube_length = 35.27f;
	private float tubeMovedis = 1035.0f;

	private Vector3 startPos;
	private Vector3 endPos;
	private float xDis;
	private float yDis;

	private int xPos = -1;
	private float moveStartX;
	private bool moveLeft = false;
	private bool moveRight = false;

	private Animator animator;
	private Rigidbody rigi;
	private NetworkIdentity nid;

	private bool gameStart = false;
	private float maxmoveSpeed = 15.0f;
	public float nowmoveSpeed = 3.0f;

	private bool grounded = true;

	private float groundY;

	[HideInInspector]
	public bool dead = false;

	private GameObject shieldEffect = null;

	public float shieldTime = 0;//4.5秒结束

	private bool showboss = false;

	[HideInInspector]
	public bool reverse;

	private GameObject boss;
	private GameObject p2;

	private float showBossTime = 0;

	// Use this for initialization
	void Start ()
	{
		if (this.transform.position == spawnPos1) {
			this.smr.material = Resources.Load ("player2") as Material;
		}

		animator = this.GetComponent<Animator> ();
		reverse = false;

		tubes [0] = GameObject.Find ("tube1");
		tubes [1] = GameObject.Find ("tube2");
		tubes [2] = GameObject.Find ("tube3");


	}

	void Awake ()
	{
		MainCamera = GameObject.Find ("Main Camera").GetComponent<MainCameraController> ();
		rigi = this.GetComponent<Rigidbody> ();
		//if (isLocalPlayer) {
		if (this.transform.position.x < 6) {
			xPos = 0;
		} else {
			xPos = 2;
		}

		//}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!showboss) {
			if (showBossTime >= 3.0f) {
			//if(Input.GetMouseButton(0)){
				MainCamera.SendMessage ("Warning");
				showboss = true;
			}
		}
		if (shieldEffect) {
			shieldEffect.transform.position = this.transform.position;
			shieldTime += Time.deltaTime;
			if (shieldTime >= 4.5f) {
				Destroy (shieldEffect);
				shieldTime = 0;
				this.gameObject.layer = 8;
			}
		}




		if (isLocalPlayer) {
			
			if (this.transform.position.y <= groundY) {
				grounded = true;
				if (!dead) {
					runningEffect.SetActive (true);
				}
				else {
					runningEffect.SetActive (false);
				}
				animator.SetBool ("grounded", true);
				animator.SetBool ("SideJump", false);
				animator.SetBool ("Jump_b", false);
			}
			else {
				grounded = false;
				animator.SetBool ("grounded", false);
				runningEffect.SetActive (false);
			}

			if (moveLeft) {
				this.transform.position = new Vector3 (this.transform.position.x - Time.deltaTime*5.0f, this.transform.position.y, this.transform.position.z);
				if (this.transform.position.x <= (moveStartX-3)) {
					moveLeft = false;
					moveStartX = this.transform.position.x;
				}
			}
			if (moveRight) {
				this.transform.position = new Vector3 (this.transform.position.x + Time.deltaTime*5.0f, this.transform.position.y, this.transform.position.z);
				if (this.transform.position.x >= (moveStartX+3)) {
					moveRight = false;
					moveStartX = this.transform.position.x;
				}
			}

			//生成桥
			if (this.transform.position.z >= (spawnZ + tube_length*1.5f)) {
				spawnZ += tube_length;
				if (tubes [0] && tubes [1] && tubes [2]) {
					tubes [3] = tubes [0];
					tubes [3].transform.localPosition = new Vector3 (tubes [3].transform.localPosition.x, tubes [3].transform.localPosition.y, tubes [3].transform.localPosition.z + tubeMovedis);
					tubes [0] = null;
					if (nowmoveSpeed <= maxmoveSpeed) {
						nowmoveSpeed += 2.0f;
					}
				} else if (tubes [1] && tubes [2] && tubes [3]) {
					tubes [0] = tubes [1];
					tubes [0].transform.localPosition = new Vector3 (tubes [0].transform.localPosition.x, tubes [0].transform.localPosition.y, tubes [0].transform.localPosition.z + tubeMovedis);
					tubes [1] = null;
				} else if (tubes [2] && tubes [3] && tubes [0]) {
					tubes [1] = tubes [2];
					tubes [1].transform.localPosition = new Vector3 (tubes [1].transform.localPosition.x, tubes [1].transform.localPosition.y, tubes [1].transform.localPosition.z + tubeMovedis);
					tubes [2] = null;
					if (nowmoveSpeed <= maxmoveSpeed) {
						nowmoveSpeed += 1.0f;
					}
				} else if (tubes [3] && tubes [0] && tubes [1]) {
					tubes [2] = tubes [3];
					tubes [2].transform.localPosition = new Vector3 (tubes [2].transform.localPosition.x, tubes [2].transform.localPosition.y, tubes [2].transform.localPosition.z + tubeMovedis);
					tubes [3] = null;
				}
			}
			if (Input.GetKey (KeyCode.Space)) {
				if (grounded) {
					animator.SetBool ("Jump_b", true);
					this.rigi.AddForce (new Vector3 (0, 250.0f, 0.0f));
				}

			}
			//向左向右跳以及boss和障碍物
			//animator.SetBool ("SideJump", true);
			//this.transform.Translate(new Vector3(this.transform.position+3
			if (Input.GetKey (KeyCode.A)) {
				if (grounded) {
				animator.SetBool ("SideJump", true);
				if (xPos == 0) {
					this.rigi.AddForce (new Vector3 (0, 150.0f, 0));
					//this.rigi.MovePosition (new Vector3 (this.transform.position.x + 3.0f, this.transform.position.y, this.transform.position.z));
				} else {
					xPos--;
					this.rigi.AddForce (new Vector3 (0f, 150.0f, 0));
					moveLeft = true;
					moveStartX = this.transform.position.x;
					//this.rigi.MovePosition (new Vector3 (this.transform.position.x - 3.0f, this.transform.position.y, this.transform.position.z));
				}
				}

			}
			if (Input.GetKey (KeyCode.D)) {
				if (grounded) {
				animator.SetBool ("SideJump", true);
				if (xPos == 2) {
					this.rigi.AddForce (new Vector3 (0, 250.0f, 0));
					//this.rigi.MovePosition (new Vector3 (this.transform.position.x + 3.0f, this.transform.position.y, this.transform.position.z));
				} else {
					this.rigi.AddForce (new Vector3 (0, 250.0f, 0));
					xPos++;
					moveRight = true;
					moveStartX = this.transform.position.x;
					//this.rigi.MovePosition (new Vector3 (this.transform.position.x + 3.0f, this.transform.position.y, this.transform.position.z));
				}
				}

			}
			if (grounded) {
				if (Input.touchCount > 0) {
					if (Input.GetTouch (0).phase == TouchPhase.Began) {
						startPos = Input.GetTouch (0).position;
					}
					if (Input.GetTouch (0).phase == TouchPhase.Ended) {
						xDis = Input.GetTouch (0).position.x - startPos.x;
						yDis = Input.GetTouch (0).position.y - startPos.y;

						if (xDis > 0) {
							if (Mathf.Abs (yDis) > xDis) {
								if (yDis > 0) {
									//this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y+3, this.transform.position.z);
									animator.SetBool ("Jump_b", true);
									this.rigi.AddForce (new Vector3 (0, 250.0f, 0.0f));
								} else if (yDis <= 0) {
									//this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y-3, this.transform.position.z);
								}
							} else {
								//this.transform.position = new Vector3 (this.transform.position.x+3, this.transform.position.y, this.transform.position.z);
								animator.SetBool ("SideJump", true);
								if (xPos == 2) {
									if (!reverse) {
										this.rigi.AddForce (new Vector3 (0, 150.0f, 0));
									}
									else {
										xPos--;
										moveLeft = true;
										moveStartX = this.transform.position.x;
									}
									//this.rigi.MovePosition (new Vector3 (this.transform.position.x + 3.0f, this.transform.position.y, this.transform.position.z));
								} else {
									this.rigi.AddForce (new Vector3 (0, 150.0f, 0));
									if (reverse) {
										if (xPos != 0) {
											xPos--;
											moveLeft = true;
											moveStartX = this.transform.position.x;
										}
									}
									else {
										xPos++;
										moveRight = true;
										moveStartX = this.transform.position.x;
									}
									//this.rigi.MovePosition (new Vector3 (this.transform.position.x + 3.0f, this.transform.position.y, this.transform.position.z));
								}
							}
						} else if (xDis <= 0) {
							if (Mathf.Abs (xDis) > Mathf.Abs (yDis)) {
								//this.transform.position = new Vector3 (this.transform.position.x-3, this.transform.position.y, this.transform.position.z);
								animator.SetBool ("SideJump", true);
								if (xPos == 0) {
									if (!reverse) {
										this.rigi.AddForce (new Vector3 (0, 150.0f, 0));
									}
									else {
										xPos++;
										moveRight = true;
										moveStartX = this.transform.position.x;
									}
									//this.rigi.MovePosition (new Vector3 (this.transform.position.x + 3.0f, this.transform.position.y, this.transform.position.z));
								} else {
									this.rigi.AddForce (new Vector3 (0f, 150.0f, 0));
									if (reverse) {
										if (xPos != 2) {
											xPos++;
											moveRight = true;
											moveStartX = this.transform.position.x;
										}
									} 
									else {
										xPos--;
										moveLeft = true;
										moveStartX = this.transform.position.x;
									}
									//this.rigi.MovePosition (new Vector3 (this.transform.position.x - 3.0f, this.transform.position.y, this.transform.position.z));
								}
							} else {
								if (yDis > 0) {
									//this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y+3, this.transform.position.z);
									animator.SetBool ("Jump_b", true);
									this.rigi.AddForce (new Vector3 (0, 250.0f, 0.0f));
								} else if (yDis <= 0) {
									//this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y-3, this.transform.position.z);
								}
							}
						}
						

					}
				}
			}

			if (gameStart) {
				showBossTime += Time.deltaTime;
				this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z + Time.deltaTime * nowmoveSpeed);
			}
		}
			
	}

	public override void OnStartLocalPlayer ()
	{
		//players++;
	}

	void JumpFalse ()
	{
		animator.SetBool ("Jump_b", false);
		animator.SetBool ("SideJump", false);
	}

	void StartRun ()
	{
		if (isLocalPlayer) {
			groundY = this.transform.position.y;
			this.animator.SetFloat ("Speed_f", 1.0f);
			gameStart = true;
		}

	}

	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.tag == "barrier") {
			this.gameStart = false;
			this.animator.SetBool ("Death_b", true);
			this.dead = true;
			this.gameObject.layer = 15;
			GameObject hitEffect = Instantiate (Resources.Load ("ElegantSparks") as GameObject);
			if (hitEffect) {
				hitEffect.transform.position = this.transform.position + new Vector3(0,3.0f,0);
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "potion") {
			this.gameObject.layer = 10;
			CmdUsePotion (col.gameObject);
		}

		if (col.gameObject.layer == 11) {
			col.GetComponentInParent<StoneController> ().Hitted ();
			CmdPlayStoneEffect ();
		}

		if (col.gameObject.layer == 13) {
			this.gameStart = false;
			this.animator.SetBool ("Death_b", true);
			this.dead = true;
			this.gameObject.layer = 15;
			col.GetComponentInParent<ParticleManager> ().DestoryThis ();
		}
	}

	void OnCollisionExit (Collision col)
	{
		//if (isLocalPlayer) {
		//if (col.gameObject.tag == "tube") {
			//grounded = false;
		//	animator.SetBool ("grounded", false);
		//}
		//}
	}

	void IsGrounded()
	{
		grounded = true;
	}

	void NotGrounded()
	{
		grounded = false;
	}

	void EndRunningJump()
	{
		switch(xPos)
		{
		case 0:
			this.transform.position = new Vector3 (2.5f, this.transform.position.y, this.transform.position.z);
			break;
		case 1:
			this.transform.position = new Vector3 (5.5f, this.transform.position.y, this.transform.position.z);
			break;
		case 2:
			this.transform.position = new Vector3 (8.5f, this.transform.position.y, this.transform.position.z);
			break;
		}
	}

	void GetBoss(GameObject b)
	{
		this.boss = b;
	}

	[Command]
	void CmdPlayStoneEffect()
	{
		ParticleSystem ps = Instantiate (stoneEffect);
		ps.transform.position = this.transform.position + new Vector3(0,1.0f,1.0f);
		NetworkServer.Spawn (ps.gameObject);
		ps.Play ();
	}

	[Command]
	void CmdUsePotion(GameObject col)
	{
		GameObject protectEffect = Instantiate (Resources.Load ("HollowSmoke") as GameObject);
		GameObject se = Instantiate (Resources.Load ("EarthDome") as GameObject);
		if (protectEffect) {
			protectEffect.transform.position = this.transform.position + new Vector3(0,2.0f,0);
			if (col != null) {
				Destroy (col.gameObject);
			}
			else {
				return;
			}
		}
		if (se) {
			se.transform.position = this.transform.position;
			shieldEffect = se;
			NetworkServer.Spawn (shieldEffect);
		}
	}



	void Lose()
	{
		MainCamera.Lose ();
	}
		
		
}
