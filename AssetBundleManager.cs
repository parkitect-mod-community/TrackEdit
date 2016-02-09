using System;
using UnityEngine;
using System.Collections.Generic;

namespace HelloMod
{
	public class AssetBundleManager
	{
		public Main Main {get;set;}
		public GameObject Node;

		public AssetBundleManager (Main main)
		{
			this.Main = main;
			Node = LoadAsset<GameObject> ("Node");
		}



		public T  LoadAsset<T>(string prefabName) where T : UnityEngine.Object
		{
			try
			{
				T asset;

				char dsc = System.IO.Path.DirectorySeparatorChar;
				using (WWW www = new WWW("file://" + Main.Path + dsc + "assetbundle" + dsc + "TrackEdit"))
				{

					if (www.error != null)
					{
						Debug.Log("Loading had an error:" + www.error);
						throw new Exception("Loading had an error:" + www.error);
					}
					if(www.assetBundle == null)
					{
						Debug.Log("Loading had an error:" + www.error);
						throw new Exception("assetBundle is null");

					}
					AssetBundle bundle = www.assetBundle;


					try
					{
						asset = bundle.LoadAsset<T>(prefabName);
						bundle.Unload(false);

						return asset;
					}
					catch (Exception e)
					{
						UnityEngine.Debug.LogException(e);
						bundle.Unload(false);
						return null;
					}
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogException(e);
				return null;
			}
		}
	}
}

