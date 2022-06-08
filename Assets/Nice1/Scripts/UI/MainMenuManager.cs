using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if VUPLEX_STANDALONE
using Vuplex.WebView;
#endif
public class MainMenuManager : Singleton<MainMenuManager>
{

    [Header("Login")]
    public UIPanel loginPanel;
    public GameObject loginPanelGO;
    public GameObject loginButton;
    public GameObject logoutButton;
    public UIPanel userPanel;
    public Text textUserAccount;


    [Header("License - Mandatory fields")]
    public string AUTHOR = "niceonedemos";
    public string IDATA_NAME = "GAME LICENSE - LegendaryLegends";
    public string CATEGORY = "llegends";

    [Header("Nice1 Genesis Key")]
    public bool checkNice1GenesisKey_bool;

    [Header("License - Error message")]
    public string errorLicenseText = "You are not licensed to use this game";

    [Header("Vuplex - Error message")]
    public string errorVuplexText = "It is necessary to have the Vuplex plugin installed. For more information go to https://docs.nice1.dev/";

    [HideInInspector]
    public int checkNice1GenesisKey;

    private void Awake()
    {
        checkNice1GenesisKey = checkNice1GenesisKey_bool ? 1 : 0;
    }

    public void SetLoggedInMenu()
    {
        HideLogin();
        if (loginButton != null) loginButton.SetActive(false);
    }

    private void SetNotLoggedInMenu()
    {
        if (loginButton != null) loginButton.SetActive(true);
        if (logoutButton != null) logoutButton.SetActive(false);
    }

    public async void LogOut()
    {
#if VUPLEX_STANDALONE
        await GameObject.FindGameObjectWithTag("canvasWebView").GetComponent<Vuplex.WebView.CanvasWebViewPrefab>().LogOut2();
#endif
        SetNotLoggedInMenu();
    }

    public void ShowLogin()
    {
#if VUPLEX_STANDALONE
        if (loginPanel != null) ShowPanel(loginPanel);
#else
        Debug.Log(errorVuplexText);
#endif
    }

    public void HideLogin()
    {

        if (loginPanel != null) HidePanel(loginPanel);
    }

    public void ShowUser()
    {
        if (userPanel != null) ShowPanel(userPanel);
    }

    public void HideUser()
    {
        if (userPanel != null) HidePanel(userPanel);
    }


    private void ShowPanel(UIPanel panel)
    {
        if (panel != null) panel.ShowPanel();
    }

    private void HidePanel(UIPanel panel)
    {
        if (panel != null) panel.HidePanel();
    }

    public void SetUserAccount(string userAccount)
    {
        textUserAccount.text = userAccount;
    }
}
