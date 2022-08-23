using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManage : MonoBehaviour
{



    private void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    //選單

    [SerializeField, Header("選單")]
    GameObject table1, table2;

    public void ToSettings()
    {
        table1.SetActive(false);
        table2.SetActive(true);
    }


    //設定

    [SerializeField, Header("人數值")]
    TextMeshProUGUI peoples;
    [SerializeField, Header("籌碼值")]
    TextMeshProUGUI chips;
    [SerializeField, Header("一注值")]
    TextMeshProUGUI bets;

    [SerializeField, Header("遊戲設定")]
    GameSettings set;

    public void PlayGame()
    {
        set.peoplesV = Int32.Parse(peoples.text);
        set.chipsV = Int32.Parse(chips.text);
        set.betsV = Int32.Parse(bets.text);

        SceneManager.LoadScene(1);
    }

    public void ToMenu()
    {
        table2.SetActive(false);
        table1.SetActive(true);
    }

    public void ASPeople(int n)
    {
        peoples.text = (Mathf.Clamp(Int32.Parse(peoples.text) + n, 2, 10)).ToString();
    }

    public void ASChips(int n)
    {
        chips.text = (Mathf.Clamp(Int32.Parse(chips.text) + n, 1000, 10000)).ToString();
    }

    public void ASBets(int n)
    {
        bets.text = (Mathf.Clamp(Int32.Parse(bets.text) + n, 50, 1000)).ToString();
    }
}
