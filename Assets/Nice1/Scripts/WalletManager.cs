using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System;


public class WalletManager : Singleton<WalletManager>
{
    [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CheckLicense(string owner, string author, string category, string license_name, string idata_name);


    [DllImport("Nice1Plugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CheckNice1GenesisKey(string owner, string author, string category, string license_name, string idata_name, int checkNice1GenesisKey);


    public WalletAccount CurrentAccount { get; private set; }

    private WebRequestResults lastRequest;

    public delegate void WalletDelegate();
    public static event WalletDelegate OnNoLicense;


    private void Awake()
    {
        CurrentAccount = new WalletAccount();
    }


    public void SetAccount(string name)
    {
        CurrentAccount.Initialize(name, null, null, null, false, null);
        StartCoroutine(SearchAssetsByOwner(name));
    }

    private IEnumerator SearchAssetsByOwner(string owner)
    {

        string url;
        if (MainMenuManager.Instance.checkNice1GenesisKey == 1)
        {
            url = "https://jungle3.api.simpleassets.io/v1/assets/search?author=" + "niceonechain" + "&owner=" + owner + "&category=" + "niceoneepics" + "&page=1&limit=1000&sortField=assetId&sortOrder=asc";
            yield return StartCoroutine(GetRequest(url));
            GetLicense(owner, checkNice1GenesisKey: 1);
            if (license == false)
            {
                url = "https://jungle3.api.simpleassets.io/v1/assets/search?author=" + MainMenuManager.Instance.AUTHOR + "&owner=" + owner + "&category=" + MainMenuManager.Instance.CATEGORY + "&page=1&limit=1000&sortField=assetId&sortOrder=asc";
                yield return StartCoroutine(GetRequest(url));
                GetLicense(owner, checkNice1GenesisKey: 0);
            }
        }
        else
        {
            url = "https://jungle3.api.simpleassets.io/v1/assets/search?author=" + MainMenuManager.Instance.AUTHOR + "&owner=" + owner + "&category=" + MainMenuManager.Instance.CATEGORY + "&page=1&limit=1000&sortField=assetId&sortOrder=asc";
            yield return StartCoroutine(GetRequest(url));
            GetLicense(owner, checkNice1GenesisKey: 0);
        }

    }

    bool license = false;
    void GetLicense(string owner, int checkNice1GenesisKey)
    {

        if (lastRequest.results.Count > 0)
        {
            for (int i = 0; i < lastRequest.results.Count; i++)
            {
                Debug.Log(lastRequest.results[i].idata.name);
                int textStr;
                if (checkNice1GenesisKey == 1)
                    textStr = CheckNice1GenesisKey(owner, lastRequest.results[i].author, MainMenuManager.Instance.CATEGORY, MainMenuManager.Instance.IDATA_NAME, lastRequest.results[i].idata.name, MainMenuManager.Instance.checkNice1GenesisKey);
                else
                    textStr = CheckLicense(owner, MainMenuManager.Instance.AUTHOR, MainMenuManager.Instance.CATEGORY, MainMenuManager.Instance.IDATA_NAME, lastRequest.results[i].idata.name);
                Debug.Log(textStr);
                if (textStr == 1)
                {
                    license = true;
                    break;
                }
            }
            if (license)
            {
                LicenseOK();
            }
            else
            {
                NO_License();
            }
        }
        else
        {
            NO_License();
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    lastRequest = JsonUtility.FromJson<WebRequestResults>(webRequest.downloadHandler.text);

                    break;

                default:
                    break;
            }
        }
    }

    public void LicenseOK()
    {
        // TO DO: write your code here when License is OK
        // EXAMPLE
        Debug.Log("LICENSE");
        MainMenuManager.Instance.HideLogin();
        MainMenuManager.Instance.SetLoggedInMenu();
        MainMenuManager.Instance.SetUserAccount(CurrentAccount.name);
        MainMenuManager.Instance.ShowUser();
        // Start your game
        // - Got to other scene
        // - Deactivate Canvas
    }

    public void NO_License()
    {
        // TO DO: write your code here when License is not OK
        Debug.Log("NO LICENSE");
        if (OnNoLicense != null) OnNoLicense();
        MainMenuManager.Instance.HideLogin();
        MainMenuManager.Instance.SetUserAccount("No License");
        MainMenuManager.Instance.ShowUser();

    }

    #region Errors
    public void ShowLoginError(string errorMessage)
    {
        string errorText = "There was a problem logging into your account. Make sure you are logged into Scatter."
                         + "\nError: " + errorMessage;


        Debug.LogError("There was a problem logging into your account. Make sure you are logged into Scatter.");
        Debug.LogError("Error: " + errorMessage);
    }

    public void ShowApiError(string errorMessage)
    {
        string errorText = "There was a problem communicating with the API. Please try again."
                         + "\nError: " + errorMessage;


        Debug.LogError("There was a problem communicating with the API. Please try again.");
        Debug.LogError("Error: " + errorMessage);
    }
    #endregion
}


[System.Serializable]
public class WalletAccount
{
    public void Initialize(string name, string authority, string publicKey, string blockChain, bool isHardware, string chainID)
    {
        this.name = name;
        this.authority = authority;
        this.publicKey = publicKey;
        this.blockChain = blockChain;
        this.isHardware = isHardware;
        this.chainID = chainID;
    }

    public string name;
    public string authority;
    public string publicKey;
    public string blockChain;
    public bool isHardware;
    public string chainID;
}

[System.Serializable]
public class WebRequestResults
{
    public List<WebResultContainer> results;
}

[System.Serializable]
public class WebResultContainer
{
    public string assetId;
    public string author;
    public string owner;
    public string category;
    public string control;
    public ImmutableData idata;
    public MutableData mdata;
}

[System.Serializable]
public class ImmutableData
{
    public string name;
    public string img;
}

[System.Serializable]
public class MutableData
{
    public string name;
}