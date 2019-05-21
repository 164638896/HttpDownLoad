using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;

public class Test : MonoBehaviour {

	Slider slider;
	Text text;
    HttpDownLoad http;

    private List<HttpDownLoad.ThreadParam> mDownLoadList = new List<HttpDownLoad.ThreadParam>();

    void Awake()
	{
		slider = GameObject.Find("Slider").GetComponent<Slider>();
		text = GameObject.Find("Text").GetComponent<Text>();
	}

	void Start ()
    {
		http = new HttpDownLoad();

        HttpDownLoad.ThreadParam param = new HttpDownLoad.ThreadParam();
        param.m_nID = 1;
        param.m_strURL = @"http://127.0.0.1/SharpZipLib_0860_Bin.zip";
        param.m_strSavePath = Application.streamingAssetsPath;
        param.callBack = ThreadCallBack;
        bool ret = ThreadPool.QueueUserWorkItem(http.DownLoad, param);
        if (!ret)
        {
            UnityEngine.Debug.LogError("");
        }

        HttpDownLoad.ThreadParam param1 = new HttpDownLoad.ThreadParam();
        param1.m_nID = 2;
        param1.m_strURL = @"http://127.0.0.1/main.6.com.top1game.rotdgp.zip";
        param1.m_strSavePath = Application.streamingAssetsPath;
        param1.callBack = ThreadCallBack;

        bool ret1 = ThreadPool.QueueUserWorkItem(http.DownLoad, param1);
        if (!ret1)
        {
            UnityEngine.Debug.LogError("");
        }
    }

	void OnDisable()
	{
		print ("OnDisable");
		http.Close();
	}

	void ThreadCallBack(HttpDownLoad.ThreadParam param)
	{
        lock (mDownLoadList)
        {
            mDownLoadList.Add(param);
        }
    }

	void Update()
	{
		slider.value = http.progress;
		text.text = "资源加载中" + (slider.value * 100).ToString("0.00") + "%";
        lock (mDownLoadList)
        {
            foreach(HttpDownLoad.ThreadParam param in mDownLoadList)
            {
                string fileName = Path.GetFileName(param.m_strURL);
                HttpDownLoad.UnZipFile(param.m_strSavePath + "/" + fileName, param.m_strSavePath);
            }

            mDownLoadList.Clear();
        }
	}

	//IEnumerator LoadScene(string url)
	//{
	//	WWW www = new WWW(url);
	//	yield return www;
	//	//AssetBundle ab = www.assetBundle;
	//	//SceneManager.LoadScene("Demo2_towers");
	//}
}
