using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MainCameraController : NetworkBehaviour
{

	public Text countdownText;
	public Text warningText;
	private int warningTimes = 6;
	private int countdownNum = 3;

	public GameObject background;
	public GameObject secondCamera;
	public Canvas gameoverUI;
	public Text win;
	public Text lose;

	private GameObject[] players;
	private GameObject localPlayer;
	private bool playersReady = false;

	private Vector3 playerPos;
	// z:26+x=21.65    y=:3.96+x=8.4
	private bool lockCa = false;

	public GameObject[] barriers;
	//y=3.88f z>=-30f
	private List<GameObject> barriersInScene = new List<GameObject> ();
	//如果x轴一样 距离起码要隔开9.0f
	private float[] barriersX = new float[3] { 2.84f, 5.84f, 8.84f };

	public GameObject[] items;
	//y=3.88f z>=-30f
	private List<GameObject> itemsInScene = new List<GameObject> ();
	//如果x轴一样 距离起码要隔开9.0f

	private GameObject groundeffect;
	private float groundeffectX = 0;
	private GameObject shooteffect = null;

	private bool startRun = false;

	private float reflashTime = 13.0f;

	private bool showBoss = false;

	private Vector3 BossCameraRotation = new Vector3 (160, 24, 180);
	private Quaternion targetRotation = Quaternion.Euler (160, 24, 180);
	private Vector3 BossCameraPosition = new Vector3 (5.5f, 8.4f, 13.24f);

	public GameObject Boss;
	public GameObject boss_leftgun;
	public GameObject lEffect;
	public GameObject boss_rightgun;
	public GameObject rEffect;
	private bool sl = false;

	private Vector3 velocity = Vector3.zero;


	private GameObject farestPlayer;

	// Use this for initialization
	void Start ()
	{
		//if(this.net)
		secondCamera.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (!playersReady) {
			if (GameObject.FindGameObjectsWithTag ("Player").Length == 1) {
				players = GameObject.FindGameObjectsWithTag ("Player");
				playersReady = true;
				CountDown ();
				LockCamera ();
				farestPlayer = players [0];
			}
		}

		if (groundeffect != null) {
			if (groundeffect.GetComponent<ParticleSystem> ().isStopped) {
				groundeffectX = groundeffect.transform.position.x;
				RealShoot (groundeffectX);
				Destroy (groundeffect);
				groundeffect = null;
			} else {
				//groundeffect.transform.position = new Vector3 (groundeffect.transform.position.x, 4.1f, localPlayer.transform.position.z);
			}
		}
		if (shooteffect != null) {
			//shooteffect.transform.Translate (new Vector3 (0, -0.3f, -1));
			//shooteffect.transform.position = Vector3.SmoothDamp(shooteffect.transform.position,localPlayer.transform.position+new Vector3(0,-2.0f,2.0f),ref velocity,0.3f);
			shooteffect.transform.position = Vector3.SmoothDamp(shooteffect.transform.position,new Vector3(shooteffect.transform.position.x,Boss.transform.position.y-5.0f,farestPlayer.transform.position.z+6.0f),ref velocity,0.3f);
			if (shooteffect.transform.position.y < 1.1f) {
				Destroy (shooteffect.gameObject);
				shooteffect = null;
			}
		}
			

		if (lockCa) {
			if (!showBoss) {
				if (Input.GetMouseButton (0)) {
				}
				Boss.transform.position = new Vector3 (Boss.transform.position.x, Boss.transform.position.y, localPlayer.transform.position.z - 23);
			}
			if (showBoss) {
				if (Input.GetMouseButton (1)) {
					boss_leftgun.GetComponent<Animation> ().enabled = true;
					boss_rightgun.GetComponent<Animation> ().enabled = true;
					lEffect.SetActive (true);
					rEffect.SetActive (true);
				}
				if (Input.GetMouseButton (2)) {
					boss_leftgun.GetComponent<Animation> ().enabled = false;
					boss_rightgun.GetComponent<Animation> ().enabled = false;
					lEffect.SetActive (false);
					rEffect.SetActive (false);
				}
				if (!sl) {
					this.transform.position = Vector3.Lerp (this.transform.position, localPlayer.transform.position + BossCameraPosition, Time.deltaTime * 7.0f);
					this.transform.rotation = Quaternion.Slerp (this.transform.rotation, targetRotation, Time.deltaTime * 3.0f);
				} else {
					this.transform.position = localPlayer.transform.position + BossCameraPosition;
					this.transform.eulerAngles = BossCameraRotation;
					background.transform.position = this.transform.position + new Vector3 (7.0f, 0, -60.0f);
				}

			} else {
				this.transform.position = localPlayer.transform.position + new Vector3 (0, 4.44f, -4.35f);
				background.transform.position = this.transform.position + new Vector3 (7, 0, 0);
			}
		}
	}

	void CountDown ()
	{
		if (countdownNum == 0) {
			startRun = true;
			CmdInstantiateBarrier ();
			countdownText.text = "";
			foreach (GameObject g in players) {
				g.SendMessage ("StartRun");
			}
			//players [0].SendMessage ("StartRun");
			//players [1].SendMessage ("StartRun");
			return;
		}
		countdownText.text = countdownNum.ToString ();
		countdownNum--;
		Invoke ("CountDown", 1.0f);
	}

	void LockCamera ()
	{
		foreach (GameObject g in players) {
			if (g.GetComponent<PlayerController> ().isLocalPlayer) {
				localPlayer = g;
			}
		}
		lockCa = true;
	}

	[Command]
	void CmdInstantiateBarrier ()
	{
		if (showBoss) {
			return;
		}

		if (barriersInScene.Count >= 2) {
			if (reflashTime >= 6.0f) {
				reflashTime -= 0.5f;
			}
			for (int i = 0; i < barriersInScene.Count; i++) {
				if (!barriersInScene [i]) {
					continue;
				}
				if (localPlayer.transform.position.z > barriersInScene [i].transform.position.z) {
					GameObject linshi = barriersInScene [i].gameObject;
					barriersInScene.Remove (barriersInScene [i].gameObject);
					Destroy (linshi);
				}
			}
		}
		float minposz = 99999.0f;
		float maxposz = -50.0f;
		int insNum = Random.Range (6, 9);
		for (int i = 0; i < insNum; i++) {
			GameObject g = Instantiate (barriers [Random.Range (0, 2)]) as GameObject;
			g.transform.position = new Vector3 (barriersX [Random.Range (0, 3)], 3.88f, Random.Range (localPlayer.transform.position.z + 20.0f, localPlayer.transform.position.z + 50.0f));
			foreach (GameObject gg in barriersInScene) {
				if ((g.transform.position.x == gg.transform.position.x) && (Mathf.Abs (g.transform.position.z - gg.transform.position.z) < 9)) {
					g.transform.position = new Vector3 (g.transform.position.x, g.transform.position.y, gg.transform.position.z + Random.Range (9.0f, 13.0f));
				}
			}
			if (g.transform.position.z < minposz) {
				minposz = g.transform.position.z;
			}
			if (g.transform.position.z > maxposz) {
				maxposz = g.transform.position.z;
			}
			NetworkServer.Spawn (g);
			barriersInScene.Add (g);
		}

		CmdInstantiateItems (minposz, maxposz);
		//barriersInScene.Sort ();
		//Debug.Log (barriersInScene.Count);

		Invoke ("CmdInstantiateBarrier", reflashTime);
	}

	[Command]
	void CmdInstantiateItems (float minz, float maxz)
	{
		int ins = Random.Range (0, 2);
		if (ins == 0) {
			return;
		} else {
			if (!showBoss) {
				GameObject item = Instantiate (items [0]) as GameObject;
				item.transform.position = new Vector3 (barriersX [Random.Range (0, 3)], 5.3f, Random.Range (minz, maxz));
				NetworkServer.Spawn (item);
			}
		}
	}

	[Command]
	void CmdInstantiateItemss ()
	{
		//道具生成在主角的z轴加13
		int ins = Random.Range (1, 3);
		if (ins == 1) {
			GameObject item;
			item = Instantiate (items [Random.Range (0, 2)]) as GameObject;
			//item = Instantiate (items[0]) as GameObject;
			item.transform.position = new Vector3 (barriersX [Random.Range (0, 3)], 5.3f, Random.Range (localPlayer.transform.position.z + 13.0f, localPlayer.transform.position.z + 16.0f));
			NetworkServer.Spawn (item);
			itemsInScene.Add (item);
		} else if (ins == 2) {
			GameObject item1;
			GameObject item2;
			item1 = Instantiate (items [0]) as GameObject;
			item2 = Instantiate (items [1]) as GameObject;
			item1.transform.position = new Vector3 (barriersX [Random.Range (0, 3)], 5.3f, Random.Range (localPlayer.transform.position.z + 13.0f, localPlayer.transform.position.z + 16.0f));
			item2.transform.position = new Vector3 (barriersX [Random.Range (0, 3)], 5.3f, Random.Range (localPlayer.transform.position.z + 13.0f, localPlayer.transform.position.z + 16.0f));

			while (item2.transform.position.x == item1.transform.position.x) {
				item2.transform.position = new Vector3 (barriersX [Random.Range (0, 3)], 5.3f, item2.transform.position.z);
			}

			NetworkServer.Spawn (item1);
			NetworkServer.Spawn (item2);
			itemsInScene.Add (item1);
			itemsInScene.Add (item2);

		}
		Invoke ("CmdInstantiateItemss", 3.5f);
	}

	void StopLerp ()
	{
		sl = true;
	}

	void ShowBoss ()
	{
		showBoss = true;
		Boss.SetActive (true);
		Invoke ("StopLerp", 4.0f);
		for (int i = 0; i < barriersInScene.Count; i++) {
			Destroy (barriersInScene [i]);
		}
		CmdInstantiateItemss ();
	}

	void Warning ()
	{
		if (warningTimes == 0) {
			ShowBoss ();
			localPlayer.GetComponent<PlayerController> ().reverse = true;
			return;
		}

		if (warningText.GetComponent<Text> ().enabled) {
			warningText.GetComponent<Text> ().enabled = false;
		} else {
			warningText.GetComponent<Text> ().enabled = true;
		}

		warningTimes--;
		Invoke ("Warning", 0.5f);
	}

	[Command]
	void CmdShootGroundEffect ()
	{
		if (groundeffect == null) {
			if (groundeffectX == 0) {
			groundeffect = Instantiate (Resources.Load ("GroundLight") as GameObject);
			groundeffect.transform.position = new Vector3 (barriersX [Random.Range (0, 3)], 4.1f, localPlayer.transform.position.z);
			//	NetworkServer.Spawn (groundeffect);
			}
		} else {
				groundeffect.transform.position = new Vector3 (barriersX [Random.Range (0, 3)], 4.1f, localPlayer.transform.position.z);

		}

		Invoke ("CmdShootGroundEffect", 0.5f);
	}

	void SetgroundeffectX()
	{
		groundeffectX = 0;
	}

	//[Command]
	void RealShoot(float xpos)
	{
		foreach (GameObject g in players) {
			if (g.GetComponent<PlayerController> ().isLocalPlayer) {
				farestPlayer = g;
			}
		}
		shooteffect = Instantiate (Resources.Load ("ArcaneSpray") as GameObject);
		//shooteffect.transform.position = new Vector3 (xpos, Boss.transform.position.y+5.0f, Boss.transform.position.z);
		shooteffect.transform.position = new Vector3 (barriersX[0], Boss.transform.position.y+5.0f, Boss.transform.position.z);
		NetworkServer.Spawn (shooteffect);
	}
		


	public void Lose()
	{
		if (gameoverUI.gameObject.activeInHierarchy) {
			lose.GetComponent<CanvasGroup> ().alpha += 0.1f;
		}
		else {
			gameoverUI.gameObject.SetActive (true);
			players [0].SetActive (false);
			players [1].SetActive (false);
		}
		Invoke ("Lose", 0.1f);
	}

	public void Win()
	{
		if (gameoverUI.gameObject.activeInHierarchy) {
			win.GetComponent<CanvasGroup> ().alpha += 0.1f;
		}
		else {
			gameoverUI.gameObject.SetActive (true);
			players [0].SetActive (false);
			players [1].SetActive (false);
		}
		Invoke ("Win", 0.1f);
	}
		
		


}
