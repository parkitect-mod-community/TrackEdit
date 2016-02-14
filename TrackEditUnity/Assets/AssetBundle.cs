using UnityEngine;
using System.Collections;
using UnityEditor;

public class AssetBundle : MonoBehaviour {

	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles()
	{
		BuildPipeline.BuildAssetBundles("./../assetbundle/" );
	}
}
