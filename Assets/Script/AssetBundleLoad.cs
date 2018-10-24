using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleLoad : MonoBehaviour {

	private static AssetBundleManifest manifest = null;

	private static Dictionary<string,AssetBundle> ABDic = new Dictionary<string, AssetBundle>();

	public static AssetBundle LoadAB(string ABpath)
	{
		if (ABDic.ContainsKey (ABpath)) {
			return ABDic [ABpath];
		}

		if (manifest == null) {
			AssetBundle ab = AssetBundle.LoadFromFile (Application.streamingAssetsPath + "/AssetBundles/AssetBundles");
			manifest = (AssetBundleManifest)ab.LoadAsset ("AssetBundlesManifest");
		}

		if (manifest) {
			string[] s = manifest.GetAllDependencies (ABpath);

			for (int i = 0; i < s.Length; i++) {
				LoadAB (s [i]);
			}

			ABDic [ABpath] = AssetBundle.LoadFromFile (Application.streamingAssetsPath + "/AssetBundles/" + ABpath);

			return ABDic [ABpath];

		}

		return null;
	}

	public static object LoadGameObject(string abName)
	{
		//if index = 5 ,length=10,substring(6,10-5-1)

		string abpath = abName + ".unity3d";

		int index = abName.LastIndexOf ('/');
		if (index == -1)
			index = abName.Length;

		string realname = abName.Substring (index + 1, abName.Length - index - 1);
		Debug.Log (abpath);
		Debug.Log (index);
		Debug.Log (realname);

		LoadAB (abpath);

		if (ABDic.ContainsKey (abpath) && ABDic [abpath]) {
			return ABDic [abpath].LoadAsset (realname);
		}

		return null;
	}

	public void onad(string s)
	{
		LoadGameObject (Application.streamingAssetsPath + "/AssetBundles/effect/1");
	}

}
