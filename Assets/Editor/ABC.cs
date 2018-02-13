using UnityEngine;
using System.Collections;
using UnityEditor;

public class AssetBundleCreate : MonoBehaviour {

	[MenuItem("MS/AssetBundleCreate")]
	public static void Build()
	{
		//将资源打包到StreamingAssets文件夹下
		//BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath);
	}

}
