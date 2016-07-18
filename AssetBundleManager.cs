using System;
using UnityEngine;
using System.Collections.Generic;
using Parkitect.UI;

namespace RollercoasterEdit
{
	public class AssetBundleManager
	{
        public Material MaterialPlane;


        private Main main;
		public GameObject nodeGo;
        public GameObject nodeRotateGo;
        public GameObject UiWindowGo;

		public AssetBundleManager (Main main)
		{
			this.main = main;

            nodeRotateGo = LoadAsset<GameObject> ("Node_Rotate");
			nodeGo = LoadAsset<GameObject> ("Node");
            UiWindowGo = LoadAsset<GameObject> ("UITrackRide");
            UiWindowGo.AddComponent<TrackEditUI> ();

            MaterialPlane = new Material(Shader.Find("Particles/Additive"));
            MaterialPlane.SetColor ("_TintColor", new Color (255, 255, 255, 100));
            MaterialPlane.SetTexture("_MainTex",AssetManager.Instance.terrainGridProjectorGO.GetComponent<Light>().cookie);
            MaterialPlane.SetTextureScale ("_MainTex", new Vector2 (1.0f, 1.0f));
            MaterialPlane.SetTextureOffset("_MainTex",new Vector2(0f,.5f));
		}



		private T  LoadAsset<T>(string prefabName) where T : UnityEngine.Object
		{
			try
			{
				T asset;

				char dsc = System.IO.Path.DirectorySeparatorChar;
				using (WWW www = new WWW("file://" + main.Path + dsc + "assetbundle" + dsc + "TrackEdit"))
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

