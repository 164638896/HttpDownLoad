using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Net;
using System.IO;

public class Test : MonoBehaviour {

	bool isDone;
	Slider slider;
	Text text;

	void Awake()
	{
		slider = GameObject.Find("Slider").GetComponent<Slider>();
		text = GameObject.Find("Text").GetComponent<Text>();
	}

	HttpDownLoad http;
	string testURL = @"http://127.0.0.1/SharpZipLib_0860_Bin.zip";
	string savePath;
	
	void Start ()
    {
		savePath = Application.streamingAssetsPath;
		http = new HttpDownLoad();
		http.DownLoad(testURL, savePath, LoadLevel);
    }

	void OnDisable()
	{
		print ("OnDisable");
		http.Close();
	}

	void LoadLevel()
	{
		isDone = true;
	}

	void Update()
	{
		slider.value = http.progress;
		text.text = "资源加载中" + (slider.value * 100).ToString("0.00") + "%"; 
		if(isDone)
		{
            UnityEngine.Debug.Log("加载完成");
            isDone = false;
            HttpDownLoad.UnZipFile(savePath + "/SharpZipLib_0860_Bin.zip", savePath);
            StartCoroutine(LoadScene(@"file://" + Application.streamingAssetsPath + "/" + testURL.Substring(testURL.LastIndexOf('/') + 1)));
		}
	}

	IEnumerator LoadScene(string url)
	{
		WWW www = new WWW(url);
		yield return www;
		//AssetBundle ab = www.assetBundle;
		//SceneManager.LoadScene("Demo2_towers");
	}
}
