/**
* Copyright 2019 Michael Pollind
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using UnityEngine;

namespace TrackEdit
{
    public class AssetBundleManager
    {
        private readonly Main _main;
        public readonly Material MaterialPlane;
        public readonly GameObject NodeGo;
        public readonly GameObject NodeRotateGo;

        public AssetBundleManager(Main main)
        {
            _main = main;
            var dsc = System.IO.Path.DirectorySeparatorChar;
            var assetBundle = AssetBundle.LoadFromFile(_main.Path + dsc + "assetbundle" + dsc + "trackedit");


            NodeRotateGo = assetBundle.LoadAsset<GameObject>("Node_Rotate");
            NodeGo = assetBundle.LoadAsset<GameObject>("Node");
            NodeGo.layer = LayerMasks.COASTER_TRACKS;

            MaterialPlane = new Material(Shader.Find("Particles/Additive"));
            MaterialPlane.SetColor("_TintColor", new Color(255, 255, 255, 100));
            MaterialPlane.SetTexture("_MainTex",
                AssetManager.Instance.terrainGridProjectorGO.GetComponent<Light>().cookie);
            MaterialPlane.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));
            MaterialPlane.SetTextureOffset("_MainTex", new Vector2(0f, .5f));
            assetBundle.Unload(false);
        }

    }
}