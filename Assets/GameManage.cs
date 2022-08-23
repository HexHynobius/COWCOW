using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManage : MonoBehaviour
{
    #region 資料
    [SerializeField, Header("面板")]
    Transform table;
    [SerializeField, Header("設定檔")]
    GameSettings set;
    [SerializeField, Header("提示文字")]
    TextMeshProUGUI hint;
    [SerializeField, Header("玩家面板")]
    GameObject playerPanel;

    [SerializeField, Header("槍莊面板")]
    GameObject banker;
    [SerializeField, Header("下注面板")]
    GameObject player;
    [SerializeField, Header("下一局面板")]
    GameObject nextGame;
    [SerializeField, Header("設定底注")]
    TextMeshProUGUI setBets;

    [SerializeField, Header("結束面板")]
    GameObject over;

    [SerializeField, Header("牌組圖片")]
    Sprite[] spriteList;

    List<PlayerInformation> playerList = new List<PlayerInformation>();

    List<int> deck = new List<int>();

    int[] bankoptions = new int[4] { 0, 1, 2, 4 };
    int[] playoptions = new int[4] { 1, 2, 3, 4 };

    int bankering;

    bool decideBanker = false;
    bool decidePlayer = false;
    bool decideNextGame = false;

    float tempA = 0;
    float tempD = 0;
    #endregion

    #region 事件
    private void Awake()
    {
    }
    void Start()
    {
        setBets.text = "底注:" + set.betsV.ToString();

        AddPlayerInformation();
        playerList[0].name.text = "玩家";

        StartCoroutine(Process());
    }

    void Update()
    {

    }
    #endregion

    #region 方法
    IEnumerator Process()
    {
        RestBox(deck);

        yield return new WaitForSeconds(1);

        for (int i = 0; i < playerList.Count; i++)
        {
            Tranbox(deck, playerList[i].handList, 5);
            ((playerList[i].cat, playerList[i].categories.text), playerList[i].suit, playerList[i].catV) = Categories(playerList[i].handList);
        }

        //發牌
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < playerList.Count; j++)
            {
                yield return new WaitForSeconds(0.05f);
                playerList[j].card[i].enabled = true;
            }
        }

        //看手牌
        for (int i = 0; i < 4; i++)
        {
            playerList[0].card[i].sprite = spriteList[playerList[0].handList[i]];
        }


        yield return new WaitForSeconds(1);

        //選擇搶莊倍率
        banker.SetActive(true);
        while (!decideBanker)
        { yield return null; }

        yield return new WaitForSeconds(1);

        for (int ai = 1; ai < playerList.Count; ai++)
        {
            int temp = bankoptions[UnityEngine.Random.Range(0, 4)];
            playerList[ai].multipleBanker = temp;
            playerList[ai].multiple.enabled = true;
            playerList[ai].multiple.text = "x" + temp.ToString();
        }

        FindBanker();

        yield return new WaitForSeconds(3);

        playerList[bankering].bankering.SetActive(true);

        for (int i = 0; i < playerList.Count; i++)
        {
            if (bankering == i)
            {
                continue;
            }
            else
            {
                playerList[i].multiple.enabled = false;
            }
        }

        if (bankering == 0)
        {
            hint.text = "等待其他玩家下注";
            hint.enabled = true;
        }
        else
        {
            hint.enabled = false;
            player.SetActive(true);
            while (!decidePlayer)
            { yield return null; }
        }


        yield return new WaitForSeconds(2);
        hint.enabled = false;

        for (int i = 0; i < playerList.Count; i++)
        {
            int temp = playoptions[UnityEngine.Random.Range(0, 4)];
            if (bankering == i || i == 0)
            {
                continue;
            }
            else
            {
                playerList[i].multipleplayer = temp;
                playerList[i].multiple.enabled = true;
                playerList[i].multiple.text = "x" + temp.ToString();
            }
        }

        yield return new WaitForSeconds(1);
        hint.enabled = true;
        hint.text = "開牌";

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < playerList.Count; j++)
            {
                playerList[j].card[i].sprite = spriteList[playerList[j].handList[i]];
                playerList[j].categories.enabled = true;
            }
        }

        yield return new WaitForSeconds(2);

        hint.text = "結算結果";

        tempA = 0;
        tempD = 0;

        for (int i = 0; i < playerList.Count; i++)
        {
            if (bankering == i)
            {
                continue;
            }
            else
            {
                if (Differences(playerList[bankering], playerList[i]) > 0)
                {
                    playerList[i].differences.text = (-1 * Differences(playerList[bankering], playerList[i])).ToString();
                    tempA += Differences(playerList[bankering], playerList[i]);
                }
                else
                {
                    playerList[i].differences.text = "+" + (-1 * Differences(playerList[bankering], playerList[i])).ToString();
                    tempD += Differences(playerList[bankering], playerList[i]);
                }
            }
        }
        if ((tempA + tempD) > 0)
        {
            playerList[bankering].differences.text = "+" + ((tempA + tempD)).ToString();
        }
        else
        {
            playerList[bankering].differences.text = ((tempA + tempD)).ToString();
        }

        if (Int32.Parse(playerList[bankering].chips.text) + (tempA + tempD) <= 0)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (bankering == i)
                {
                    continue;
                }
                else
                {
                    if (Differences(playerList[bankering], playerList[i]) > 0)
                    {
                        playerList[i].differences.text = (-1 * DifferencesV2(playerList[bankering], playerList[i], Int32.Parse(playerList[bankering].chips.text) + tempA, tempD)).ToString();
                    }
                    else
                    {
                        playerList[i].differences.text = "+" + (-1 * DifferencesV2(playerList[bankering], playerList[i], Int32.Parse(playerList[bankering].chips.text) + tempA, tempD)).ToString();
                    }
                }
            }
            playerList[bankering].differences.text = (-1 * Int32.Parse(playerList[bankering].chips.text)).ToString();
        }

        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].differences.enabled = true;
            playerList[i].chips.text = (Int32.Parse(playerList[i].chips.text) + Int32.Parse(playerList[i].differences.text)).ToString();
        }


        yield return new WaitForSeconds(1);





        if (playerList[0].chips.text == "0")
        {
            over.SetActive(true);
            while (true)
            { yield return null; }
        }


        //確認結果

        hint.enabled = false;

        nextGame.SetActive(true);
        while (!decideNextGame)
        { yield return null; }


        //移除玩家

        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].chips.text == "0")
            {
                Destroy(table.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i].chips.text == "0")
            {
                playerList.RemoveAll(e => e.chips.text == "0");
            }
        }

        if (playerList.Count == 1)
        {
            over.SetActive(true);
        }


        yield return new WaitForSeconds(1);

        hint.text = "下一局";
        hint.enabled = true;

        //收牌
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < playerList.Count; j++)
            {
                playerList[j].card[i].enabled = false;
            }
        }

        //換牌背
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < playerList.Count; j++)
            {
                playerList[j].card[i].sprite = spriteList[52];
            }
        }

        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].bankering.SetActive(false);
            playerList[i].categories.enabled = false;
            playerList[i].multiple.enabled = false;
            playerList[i].differences.enabled = false;
        }

        decideBanker = false;
        decidePlayer = false;
        decideNextGame = false;

        yield return new WaitForSeconds(1);

        hint.enabled = false;

        StartCoroutine(Process());
    }

    void AddPlayerInformation()
    {
        for (int i = 0; i < set.peoplesV; i++)
        {
            playerList.Add(new PlayerInformation());
        }

        float angle;

        for (int i = 0; i < set.peoplesV; i++)
        {
            angle = (-90 - (360 / set.peoplesV * i)) * Mathf.Deg2Rad;
            playerList[i].playerPanel = Instantiate(playerPanel, table, false);
            playerList[i].playerPanel.transform.position = new Vector3(playerList[i].playerPanel.transform.position.x + (Screen.width * 0.4f) * Mathf.Cos(angle), playerList[i].playerPanel.transform.position.y + (Screen.height * 0.38f) * Mathf.Sin(angle) + Screen.height / 20, 0);

            playerList[i].chips = playerList[i].playerPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            playerList[i].chips.text = set.chipsV.ToString();

            playerList[i].name = playerList[i].playerPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            playerList[i].name.text = $"CPU{i}";

            playerList[i].categories = playerList[i].playerPanel.transform.GetChild(11).GetComponent<TextMeshProUGUI>();
            playerList[i].bets = playerList[i].playerPanel.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
            playerList[i].differences = playerList[i].playerPanel.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

            playerList[i].bankering = playerList[i].playerPanel.transform.GetChild(5).gameObject;

            playerList[i].card[0] = playerList[i].playerPanel.transform.GetChild(6).GetComponent<Image>();
            playerList[i].card[1] = playerList[i].playerPanel.transform.GetChild(7).GetComponent<Image>();
            playerList[i].card[2] = playerList[i].playerPanel.transform.GetChild(8).GetComponent<Image>();
            playerList[i].card[3] = playerList[i].playerPanel.transform.GetChild(9).GetComponent<Image>();
            playerList[i].card[4] = playerList[i].playerPanel.transform.GetChild(10).GetComponent<Image>();

            playerList[i].multiple = playerList[i].playerPanel.transform.GetChild(12).GetComponent<TextMeshProUGUI>();
        }
    }

    void RestBox(List<int> box)
    {
        box.Clear();
        for (int i = 0; i < 52; i++)
        {
            box.Add(i);
        }
    }

    void Tranbox(List<int> sendbox, List<int> receivebox, int quantity)
    {

        receivebox.Clear();

        int tem;
        for (int i = 0; i < quantity; i++)
        {
            tem = UnityEngine.Random.Range(0, sendbox.Count);
            receivebox.Add(sendbox[tem]);
            sendbox.RemoveAt(tem);
        }
    }

    void FindBanker()
    {
        List<int> n4 = new List<int>();
        List<int> n2 = new List<int>();
        List<int> n1 = new List<int>();

        for (int i = 0; i < playerList.Count; i++)
        {
            switch (playerList[i].multipleBanker)
            {
                case 4:
                    n4.Add(i);
                    break;
                case 2:
                    n2.Add(i);
                    break;
                case 1:
                    n1.Add(i);
                    break;
                case 0:
                    break;
            }
        }
        if (n4.Count > 0)
        {
            if (n4.Count > 1)
            { hint.text = "多人搶莊\n隨機定莊"; }
            else { hint.text = "選定莊家"; }
            hint.enabled = true;
            bankering = n4[UnityEngine.Random.Range(0, n4.Count)];
        }
        else if (n2.Count > 0)
        {
            if (n2.Count > 1)
            { hint.text = "多人搶莊\n隨機定莊"; }
            else { hint.text = "選定莊家"; }
            hint.enabled = true;
            bankering = n2[UnityEngine.Random.Range(0, n2.Count)];
        }
        else if (n1.Count > 0)
        {
            if (n1.Count > 1)
            { hint.text = "多人搶莊\n隨機定莊"; }
            else { hint.text = "選定莊家"; }
            hint.enabled = true;
            bankering = n1[UnityEngine.Random.Range(0, n1.Count)];
            playerList[bankering].multipleBanker = 1;
            playerList[bankering].multiple.text = "x1";
        }
        else
        {
            hint.text = "沒人搶莊\n隨機定莊";
            hint.enabled = true;
            bankering = UnityEngine.Random.Range(0, playerList.Count);
        }
    }
    void SumDifferences(List<PlayerInformation> list, int bankering)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (bankering == i)
            {
                continue;
            }
            else
            {
                if (Differences(list[bankering], list[i]) > 0)
                {
                    list[i].differences.text = (Differences(list[bankering], list[i])).ToString();
                    list[i].chips.text = (Int32.Parse(list[i].chips.text) + Differences(list[bankering], list[i])).ToString();
                    list[bankering].chips.text = (Int32.Parse(list[bankering].chips.text) + Differences(list[bankering], list[i])).ToString();
                }
            }
        }

        int temp = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (bankering == i)
            {
                continue;
            }
            else
            {
                if (Differences(list[bankering], list[i]) < 0)
                {
                    temp = temp - Differences(list[bankering], list[i]);
                }
            }
        }
        if (Int32.Parse(list[bankering].chips.text) < temp)
        {
            int tempX = (Int32.Parse(list[bankering].chips.text) / temp);
            for (int i = 0; i < list.Count; i++)
            {
                if (bankering == i)
                {
                    continue;
                }
                else
                {
                    if (Differences(list[bankering], list[i]) < 0)
                    {
                        list[i].differences.text = (tempX * (Differences(list[bankering], list[i]))).ToString();
                        list[i].chips.text = (Int32.Parse(list[i].chips.text) + (tempX * (Differences(list[bankering], list[i])))).ToString();
                        list[bankering].chips.text = (tempX * (Int32.Parse(list[bankering].chips.text) + Differences(list[bankering], list[i]))).ToString();
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (bankering == i)
                {
                    continue;
                }
                else
                {
                    if (Differences(list[bankering], list[i]) < 0)
                    {
                        list[i].differences.text = (Differences(list[bankering], list[i])).ToString();
                        list[i].chips.text = (Int32.Parse(list[i].chips.text) + Differences(list[bankering], list[i])).ToString();
                        list[bankering].chips.text = (Int32.Parse(list[bankering].chips.text) + Differences(list[bankering], list[i])).ToString();
                    }
                }
            }
        }
    }

    float DifferencesV2(PlayerInformation banker, PlayerInformation player, float tempA, float tempD)
    {
        int bm, pm;
        if (banker.cat >= 12) { bm = 4; }
        else if (banker.cat == 11) { bm = 3; }
        else if (banker.cat >= 8) { bm = 2; }
        else { bm = 1; }

        if (player.cat >= 12) { pm = -4; }
        else if (player.cat == 11) { pm = -3; }
        else if (player.cat >= 8) { pm = -2; }
        else { pm = -1; }

        switch (banker.cat - player.cat)
        {
            case > 0:
                if (banker.multipleBanker * player.multipleplayer * bm * set.betsV > Int32.Parse(player.chips.text))
                {
                    return Int32.Parse(player.chips.text);
                }
                else { return banker.multipleBanker * player.multipleplayer * bm * set.betsV; }
            case 0:
                switch (banker.catV - player.catV)
                {
                    case > 0:
                        if (banker.multipleBanker * player.multipleplayer * bm * set.betsV > Int32.Parse(player.chips.text))
                        {
                            return Int32.Parse(player.chips.text);
                        }
                        else { return banker.multipleBanker * player.multipleplayer * bm * set.betsV; }
                    case 0:
                        switch (banker.suit - player.suit)
                        {
                            case > 0:
                                if (banker.multipleBanker * player.multipleplayer * bm * set.betsV > Int32.Parse(player.chips.text))
                                {
                                    return Int32.Parse(player.chips.text);
                                }
                                else { return banker.multipleBanker * player.multipleplayer * bm * set.betsV; }
                            case < 0:
                                return Mathf.Round(tempA * ((banker.multipleBanker * player.multipleplayer * pm * set.betsV) / (-1 * tempD)));
                        }
                        return 0;
                    case < 0:
                        return Mathf.Round(tempA * ((banker.multipleBanker * player.multipleplayer * pm * set.betsV) / (-1 * tempD)));
                }
            case < 0:
                return Mathf.Round(tempA * ((banker.multipleBanker * player.multipleplayer * pm * set.betsV) / (-1 * tempD)));
        }
    }


    int Differences(PlayerInformation banker, PlayerInformation player)
    {
        int bm, pm;
        if (banker.cat >= 12) { bm = 4; }
        else if (banker.cat == 11) { bm = 3; }
        else if (banker.cat >= 8) { bm = 2; }
        else { bm = 1; }

        if (player.cat >= 12) { pm = -4; }
        else if (player.cat == 11) { pm = -3; }
        else if (player.cat >= 8) { pm = -2; }
        else { pm = -1; }

        switch (banker.cat - player.cat)
        {
            case > 0:
                if (banker.multipleBanker * player.multipleplayer * bm * set.betsV > Int32.Parse(player.chips.text))
                {
                    return Int32.Parse(player.chips.text);
                }
                else { return banker.multipleBanker * player.multipleplayer * bm * set.betsV; }
            case 0:
                switch (banker.catV - player.catV)
                {
                    case > 0:
                        if (banker.multipleBanker * player.multipleplayer * bm * set.betsV > Int32.Parse(player.chips.text))
                        {
                            return Int32.Parse(player.chips.text);
                        }
                        else { return banker.multipleBanker * player.multipleplayer * bm * set.betsV; }
                    case 0:
                        switch (banker.suit - player.suit)
                        {
                            case > 0:
                                if (banker.multipleBanker * player.multipleplayer * bm * set.betsV > Int32.Parse(player.chips.text))
                                {
                                    return Int32.Parse(player.chips.text);
                                }
                                else { return banker.multipleBanker * player.multipleplayer * bm * set.betsV; }
                            case < 0:
                                return banker.multipleBanker * player.multipleplayer * pm * set.betsV;
                        }
                        return 0;
                    case < 0:
                        return banker.multipleBanker * player.multipleplayer * pm * set.betsV;
                }
            case < 0:
                return banker.multipleBanker * player.multipleplayer * pm * set.betsV;
        }
    }


    //牌型 15~0

    //花色 3~0

    //數字 13~1

    ((int, string), int, int) Categories(List<int> box)
    {
        List<int> boxSort = new List<int>();

        for (int i = 0; i < box.Count; i++)
        {
            boxSort.Add(i);
            boxSort[i] = box[i];
        }

        boxSort.Sort();

        List<int> n = new List<int>();
        List<int> nS = new List<int>();
        List<int> s = new List<int>();

        for (int i = 0; i < boxSort.Count; i++)
        {
            n.Add((boxSort[i] % 13) + 1);
            nS.Add((boxSort[i] % 13) + 1);
            s.Add(boxSort[i] / 13);
        }

        if (n.Count <= 10 && n[0] < 5 && n[1] < 5 && n[2] < 5 && n[3] < 5 && n[4] < 5)
        {
            return ((15, "<color=#9932CC>五小牛</color>"), s[4], n[4]);
        } //("五小牛"); }
        else if (n[0] == n[1] && n[0] == n[2] && n[0] == n[3])
        {
            return ((14, "<color=#9932CC>炸彈</color>"), s[3], n[3]);
        } //("炸彈_1"); }
        else if (n[1] == n[2] && n[1] == n[3] && n[1] == n[4])
        {
            return ((14, "<color=#9932CC>炸彈</color>"), s[4], n[4]);
        } //("炸彈_2"); }
        else if (n[0] > 10 && n[1] > 10 && n[2] > 10 && n[3] > 10 && n[4] > 10)
        {
            return ((13, "<color=#9932CC>五花牛</color>"), s[4], n[4]);
        } //("五花牛"); }
        else if (n[0] == 10 && n[1] > 10 && n[2] > 10 && n[3] > 10 && n[4] > 10)
        {
            return ((12, "<color=#9932CC>四花牛</color>"), s[4], n[4]);
        } //("四花牛"); }
        else
        {
            for (int i = 0; i < n.Count; i++)
            {
                if (nS[i] > 10)
                    nS[i] = 10;
            }

            if ((nS[0] + nS[1] + nS[2]) % 10 == 0)
            {
                return (SwitchRemainder(nS[3], nS[4]), s[4], n[4]);
            }
            else if ((nS[0] + nS[1] + nS[3]) % 10 == 0)
            {
                return (SwitchRemainder(nS[2], nS[4]), s[4], n[4]);
            }
            else if ((nS[0] + nS[1] + nS[4]) % 10 == 0)
            {
                return (SwitchRemainder(nS[2], nS[3]), s[4], n[4]);
            }

            else if ((nS[0] + nS[2] + nS[3]) % 10 == 0)
            {
                return (SwitchRemainder(nS[1], nS[4]), s[4], n[4]);
            }
            else if ((nS[0] + nS[2] + nS[4]) % 10 == 0)
            {
                return (SwitchRemainder(nS[1], nS[3]), s[4], n[4]);
            }
            else if ((nS[0] + nS[3] + nS[4]) % 10 == 0)
            {
                return (SwitchRemainder(nS[1], nS[2]), s[4], n[4]);
            }

            else if ((nS[1] + nS[2] + nS[3]) % 10 == 0)
            {
                return (SwitchRemainder(nS[0], nS[4]), s[4], n[4]);
            }
            else if ((nS[1] + nS[2] + nS[4]) % 10 == 0)
            {
                return (SwitchRemainder(nS[0], nS[3]), s[4], n[4]);
            }
            else if ((nS[1] + nS[3] + nS[4]) % 10 == 0)
            {
                return (SwitchRemainder(nS[0], nS[2]), s[4], n[4]);
            }

            else if ((nS[2] + nS[3] + nS[4]) % 10 == 0)
            {
                return (SwitchRemainder(nS[0], nS[1]), s[4], n[4]);
            }

            else
            {
                return ((1, "無牛"), s[4], n[4]);
            }// ("無牛"); }
        }
    }

    (int, string) SwitchRemainder(int a, int b)
    {
        switch ((a + b) % 10)
        {
            case 0:
                return (11, "<color=#FFD700>牛牛</color>");
            case 9:
                return (10, "<color=#007FFF>牛九</color>");
            case 8:
                return (9, "<color=#007FFF>牛八</color>");
            case 7:
                return (8, "<color=#007FFF>牛七</color>");
            case 6:
                return (7, "<color=#E32636>牛六</color>");
            case 5:
                return (6, "<color=#E32636>牛五</color>");
            case 4:
                return (5, "<color=#E32636>牛四</color>");
            case 3:
                return (4, "<color=#E32636>牛三</color>");
            case 2:
                return (3, "<color=#E32636>牛二</color>");
            case 1:
                return (2, "<color=#E32636>牛一</color>");
        }
        return (1, "無牛"); //(null);
    }

    //按鈕

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void DoBanker(int n)
    {
        banker.SetActive(false);
        decideBanker = true;
        playerList[0].multiple.text = "x" + n.ToString();
        playerList[0].multiple.enabled = true;
        playerList[0].multipleBanker = n;
    }

    public void DoPlayer(int n)
    {
        player.SetActive(false);
        decidePlayer = true;
        playerList[0].multiple.text = "x" + n.ToString();
        playerList[0].multiple.enabled = true;
        playerList[0].multipleplayer = n;
    }

    public void DoNextGame()
    {
        nextGame.SetActive(false);
        decideNextGame = true;
    }
    #endregion
}

public class PlayerInformation
{
    public GameObject playerPanel;

    public TextMeshProUGUI name;

    public TextMeshProUGUI chips;//籌碼
    public TextMeshProUGUI categories;//牌型
    public TextMeshProUGUI bets;//下注
    public TextMeshProUGUI differences;//差值
    public GameObject bankering;//莊家圖
    public TextMeshProUGUI multiple;//倍率


    public Image[] card = new Image[5];

    public List<int> handList = new List<int>();

    public int multipleBanker;
    public int multipleplayer;

    public int cat;
    public int suit;
    public int catV;

}

/*
for (int i = 0; i < playerList.Count; i++)
{

}
*/