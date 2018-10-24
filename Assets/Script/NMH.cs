﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
namespace UnityEngine.Networking
{
	[EditorBrowsable(EditorBrowsableState.Never), AddComponentMenu("Network/NetworkManagerHUD"), RequireComponent(typeof(NetworkManager))]
	public class NMH : MonoBehaviour
	{
		public NetworkManager manager;
		[SerializeField]
		public bool showGUI = true;
		[SerializeField]
		public int offsetX;
		[SerializeField]
		public int offsetY;
		private bool m_ShowServer;

		public Button createMatch;
		public Image panel1;
		public InputField if1;
		public Text if1Text;
		public Button rightBtn;
		public Button xBtn;

		private void Awake()
		{
			this.manager = base.GetComponent<NetworkManager>();
		}
		private void Update()
		{
			if (this.manager.IsClientConnected ()) {
				if (xBtn) {
					xBtn.GetComponent<CanvasGroup> ().alpha = 0;
				}
			}
			if (!this.showGUI)
			{
				return;
			}
			else {
				
			}

			if (!this.manager.IsClientConnected() && !NetworkServer.active && this.manager.matchMaker == null)
			{
				if (Application.platform != RuntimePlatform.WebGLPlayer)
				{
					if (Input.GetKeyDown(KeyCode.S))
					{
						this.manager.StartServer();
					}
					if (Input.GetKeyDown(KeyCode.H))
					{
						this.manager.StartHost();
					}
				}
				if (Input.GetKeyDown(KeyCode.C))
				{
					this.manager.StartClient();
				}
			}
			if (NetworkServer.active && this.manager.IsClientConnected() && Input.GetKeyDown(KeyCode.X))
			{
				this.manager.StopHost();
			}
		}
		private void OnGUI()
		{
			if (!this.showGUI)
			{
				return;
			}
			int num = 10 + this.offsetX;
			int num2 = 40 + this.offsetY;
			bool flag = this.manager.client == null || this.manager.client.connection == null || this.manager.client.connection.connectionId == -1;
			if (!this.manager.IsClientConnected() && !NetworkServer.active && this.manager.matchMaker == null)
			{
				if (flag)
				{
					if (Application.platform != RuntimePlatform.WebGLPlayer)
					{
						if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "创建房间"))
						{
							this.manager.StartHost();
							showGUI = false;
						}
						num2 += 24;
					}
					if (GUI.Button(new Rect((float)num, (float)num2, 105f, 20f), "输入房间号IP："))
					{
						this.manager.StartClient();
						showGUI = false;
					}
					this.manager.networkAddress = GUI.TextField(new Rect((float)(num + 100), (float)num2, 95f, 20f), this.manager.networkAddress);
					num2 += 24;
					if (Application.platform == RuntimePlatform.WebGLPlayer)
					{
						GUI.Box(new Rect((float)num, (float)num2, 200f, 25f), "(  WebGL cannot be server  )");
						num2 += 24;
					}
					else
					{
//						if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "LAN Server Only(S)"))
//						{
//							this.manager.StartServer();
//						}
						num2 += 24;
					}
				}
				else
				{
					GUI.Label(new Rect((float)num, (float)num2, 200f, 20f), string.Concat(new object[]
						{
							"Connecting to ",
							this.manager.networkAddress,
							":",
							this.manager.networkPort,
							".."
						}));
					num2 += 24;
					if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Cancel Connection Attempt"))
					{
						this.manager.StopClient();
					}
				}
			}
			else
			{
				if (NetworkServer.active)
				{
					string text = "Server: port=" + this.manager.networkPort;
					if (this.manager.useWebSockets)
					{
						text += " (Using WebSockets)";
					}
					GUI.Label(new Rect((float)num, (float)num2, 300f, 20f), text);
					num2 += 24;
				}
				if (this.manager.IsClientConnected())
				{
					GUI.Label(new Rect((float)num, (float)num2, 300f, 20f), string.Concat(new object[]
						{
							"Client: address=",
							this.manager.networkAddress,
							" port=",
							this.manager.networkPort
						}));
					num2 += 24;
				}
			}
			if (this.manager.IsClientConnected() && !ClientScene.ready)
			{
				if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Client Ready"))
				{
					ClientScene.Ready(this.manager.client.connection);
					if (ClientScene.localPlayers.Count == 0)
					{
						ClientScene.AddPlayer(0);
					}
				}
				num2 += 24;
			}
			if (NetworkServer.active || this.manager.IsClientConnected())
			{
				if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Stop (X)"))
				{
					this.manager.StopHost();
				}
				num2 += 24;
			}
			if (!NetworkServer.active && !this.manager.IsClientConnected() && flag)
			{
				num2 += 10;
				if (Application.platform == RuntimePlatform.WebGLPlayer)
				{
					GUI.Box(new Rect((float)(num - 5), (float)num2, 220f, 25f), "(WebGL cannot use Match Maker)");
					return;
				}
				if (this.manager.matchMaker == null)
				{
//					if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Enable Match Maker (M)"))
//					{
//						this.manager.StartMatchMaker();
//					}
					num2 += 24;
				}
				else
				{
					if (this.manager.matchInfo == null)
					{
						if (this.manager.matches == null)
						{
							if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Create Internet Match"))
							{
								this.manager.matchMaker.CreateMatch(this.manager.matchName, this.manager.matchSize, true, string.Empty, string.Empty, string.Empty, 0, 0, new NetworkMatch.DataResponseDelegate<MatchInfo>(this.manager.OnMatchCreate));
							}
							num2 += 24;
							GUI.Label(new Rect((float)num, (float)num2, 100f, 20f), "Room Name:");
							this.manager.matchName = GUI.TextField(new Rect((float)(num + 100), (float)num2, 100f, 20f), this.manager.matchName);
							num2 += 24;
							num2 += 10;
							if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Find Internet Match"))
							{
								this.manager.matchMaker.ListMatches(0, 20, string.Empty, false, 0, 0, new NetworkMatch.DataResponseDelegate<List<MatchInfoSnapshot>>(this.manager.OnMatchList));
							}
							num2 += 24;
						}
						else
						{
							for (int i = 0; i < this.manager.matches.Count; i++)
							{
								MatchInfoSnapshot matchInfoSnapshot = this.manager.matches[i];
								if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Join Match:" + matchInfoSnapshot.name))
								{
									this.manager.matchName = matchInfoSnapshot.name;
									this.manager.matchMaker.JoinMatch(matchInfoSnapshot.networkId, string.Empty, string.Empty, string.Empty, 0, 0, new NetworkMatch.DataResponseDelegate<MatchInfo>(this.manager.OnMatchJoined));
								}
								num2 += 24;
							}
							if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Back to Match Menu"))
							{
								this.manager.matches = null;
							}
							num2 += 24;
						}
					}
					if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Change MM server"))
					{
						this.m_ShowServer = !this.m_ShowServer;
					}
					if (this.m_ShowServer)
					{
						num2 += 24;
						if (GUI.Button(new Rect((float)num, (float)num2, 100f, 20f), "Local"))
						{
							this.manager.SetMatchHost("localhost", 1337, false);
							this.m_ShowServer = false;
						}
						num2 += 24;
						if (GUI.Button(new Rect((float)num, (float)num2, 100f, 20f), "Internet"))
						{
							this.manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
							this.m_ShowServer = false;
						}
						num2 += 24;
						if (GUI.Button(new Rect((float)num, (float)num2, 100f, 20f), "Staging"))
						{
							this.manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
							this.m_ShowServer = false;
						}
					}
					num2 += 24;
					GUI.Label(new Rect((float)num, (float)num2, 300f, 20f), "MM Uri: " + this.manager.matchMaker.baseUri);
					num2 += 24;
					if (GUI.Button(new Rect((float)num, (float)num2, 200f, 20f), "Disable Match Maker"))
					{
						this.manager.StopMatchMaker();
					}
					num2 += 24;
				}
			}
		}

		public void StartGame()
		{
			this.manager.StartHost();
			showGUI = false;

			createMatch.GetComponent<CanvasGroup> ().alpha = 0;
			panel1.GetComponent<CanvasGroup> ().alpha = 0;
			if1.GetComponent<CanvasGroup> ().alpha = 0;
			rightBtn.GetComponent<CanvasGroup> ().alpha = 0;
			xBtn.GetComponent<CanvasGroup> ().alpha = 0;
		}

		public void SearchGame()
		{
			this.manager.StartClient();
			showGUI = false;
			this.manager.networkAddress = if1Text.text;


			createMatch.GetComponent<CanvasGroup> ().alpha = 0;
			panel1.GetComponent<CanvasGroup> ().alpha = 0;
			if1.GetComponent<CanvasGroup> ().alpha = 0;
			//rightBtn.gameObject.SetActive (false);
			rightBtn.GetComponent<CanvasGroup> ().alpha = 0;
		}

		public void BackToMain()
		{
			this.manager.StopClient();
			showGUI = false;

			createMatch.GetComponent<CanvasGroup> ().alpha = 1;
			panel1.GetComponent<CanvasGroup> ().alpha = 1;
			if1.GetComponent<CanvasGroup> ().alpha = 1;
			rightBtn.GetComponent<CanvasGroup> ().alpha = 1;
		}
	}


}