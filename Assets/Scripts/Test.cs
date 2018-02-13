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
	string url = @"http://192.168.203.79/Unity5.6.4.rar";
	string savePath;
	
	void Start () {
		savePath = Application.streamingAssetsPath;
		http = new HttpDownLoad();
		http.DownLoad(url, savePath, LoadLevel);
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
			isDone = false;
			string url = @"file://" + Application.streamingAssetsPath + "/test";
			StartCoroutine(LoadScene(url));
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
