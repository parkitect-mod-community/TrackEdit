using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TrackEdit
{
    public class AssetBundleManager
    {
        private readonly Main _main;
        public readonly Material MaterialPlane;
        public readonly GameObject NodeGo;
        public readonly GameObject NodeRotateGo;
       // public readonly GameObject UiContainerWindowGo;
        //public readonly GameObject UiHeaderPanelGo;

        private AssetBundle assetBundle;
        public AssetBundleManager(Main main)
        {
            
            _main = main;
            var dsc = System.IO.Path.DirectorySeparatorChar;
            assetBundle = AssetBundle.LoadFromFile( _main.Path + dsc + "assetbundle" + dsc + "trackedit");


            NodeRotateGo = assetBundle.LoadAsset<GameObject>("Node_Rotate");
            NodeGo = assetBundle.LoadAsset<GameObject>("Node");
            NodeGo.layer = LayerMasks.COASTER_TRACKS;

           // UiContainerWindowGo = assetBundle.LoadAsset<GameObject>("TrackEditPanel");
           // UiHeaderPanelGo = assetBundle.LoadAsset<GameObject>("HeaderPanel");

            MaterialPlane = new Material(Shader.Find("Particles/Additive"));
            MaterialPlane.SetColor("_TintColor", new Color(255, 255, 255, 100));
            MaterialPlane.SetTexture("_MainTex",
                AssetManager.Instance.terrainGridProjectorGO.GetComponent<Light>().cookie);
            MaterialPlane.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));
            MaterialPlane.SetTextureOffset("_MainTex", new Vector2(0f, .5f));
            assetBundle.Unload(false);
        }


        /*private T LoadAsset<T>(string prefabName) where T : Object
        {
            try
            {
                T asset;

                var dsc = System.IO.Path.DirectorySeparatorChar;
                using (var www = new WWW("file://" + _main.Path + dsc + "assetbundle" + dsc + "trackedit"))
                {
                    if (www.error != null)
                    {
                        Debug.Log("Loading had an error:" + www.error);
                        throw new Exception("Loading had an error:" + www.error);
                    }

                    if (www.assetBundle == null)
                    {
                        Debug.Log("Loading had an error:" + www.error);
                        throw new Exception("assetBundle is null");
                    }

                    var bundle = www.assetBundle;


                    try
                    {
                        asset = bundle.LoadAsset<T>(prefabName);
                        bundle.Unload(false);

                        return asset;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        bundle.Unload(false);
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }*/
    }
}