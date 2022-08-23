using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestAPI : MonoBehaviour
{
    List<int> test = new List<int>();
    List<int> myTest = new List<int>();


    public TextMeshProUGUI boxText;

    public TextMeshProUGUI handsText;
    public TextMeshProUGUI cardText;

    string card;

    private void Awake()
    {

    }

    private void Start()
    {
        RestBox(test);
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
        int tem;
        for (int i = 0; i < quantity; i++)
        {
            tem = Random.Range(0, sendbox.Count);
            receivebox.Add(sendbox[tem]);
            sendbox.RemoveAt(tem);
        }
    }

    string Hands(List<int> box)
    {
        box.Sort();

        List<int> n = new List<int>();
        List<int> s = new List<int>();

        for (int i = 0; i < box.Count; i++)
        {
            n.Add((box[i] % 13) + 1);
            s.Add(box[i] / 13);
        }

        if (n.Count <= 10 && n[0] < 5 && n[1] < 5 && n[2] < 5 && n[3] < 5 && n[4] < 5)
        { return ("五小牛"); }
        else if (n[0] == n[1] && n[0] == n[2] && n[0] == n[3])
        { return ("炸彈_1"); }
        else if (n[1] == n[2] && n[1] == n[3] && n[1] == n[4])
        { return ("炸彈_2"); }
        else if (n[0] > 10 && n[1] > 10 && n[2] > 10 && n[3] > 10 && n[4] > 10)
        { return ("五花牛"); }
        else if (n[0] == 10 && n[1] > 10 && n[2] > 10 && n[3] > 10 && n[4] > 10)
        { return ("四花牛"); }
        else
        {
            for (int i = 0; i < n.Count; i++)
            {
                if (n[i] > 10)
                    n[i] = 10;
            }

            if ((n[0] + n[1] + n[2]) % 10 == 0)
            { return SwitchRemainder(n[3], n[4]); }
            else if ((n[0] + n[1] + n[3]) % 10 == 0)
            { return SwitchRemainder(n[2], n[4]); }
            else if ((n[0] + n[1] + n[4]) % 10 == 0)
            { return SwitchRemainder(n[2], n[3]); }

            else if ((n[0] + n[2] + n[3]) % 10 == 0)
            { return SwitchRemainder(n[1], n[4]); }
            else if ((n[0] + n[2] + n[4]) % 10 == 0)
            { return SwitchRemainder(n[1], n[3]); }
            else if ((n[0] + n[3] + n[4]) % 10 == 0)
            { return SwitchRemainder(n[1], n[2]); }

            else if ((n[1] + n[2] + n[3]) % 10 == 0)
            { return SwitchRemainder(n[0], n[4]); }
            else if ((n[1] + n[2] + n[4]) % 10 == 0)
            { return SwitchRemainder(n[0], n[3]); }
            else if ((n[1] + n[3] + n[4]) % 10 == 0)
            { return SwitchRemainder(n[0], n[2]); }

            else if ((n[2] + n[3] + n[4]) % 10 == 0)
            { return SwitchRemainder(n[0], n[1]); }
            
            else { return ("無牛"); }
        }
    }

    string SwitchRemainder(int a, int b)
    {
        switch ((a + b) % 10)
        {
            case 0:
                return ("牛牛");
            case 1:
                return ("牛一");
            case 2:
                return ("牛二");
            case 3:
                return ("牛三");
            case 4:
                return ("牛四");
            case 5:
                return ("牛五");
            case 6:
                return ("牛六");
            case 7:
                return ("牛七");
            case 8:
                return ("牛八");
            case 9:
                return ("牛九");
        }
        return (null);
    }

    //測試用
    void Changbox(List<int> sendbox, List<int> receivebox, int quantity)
    {
        receivebox.Clear();
        int tem;
        for (int i = 0; i < quantity; i++)
        {
            tem = Random.Range(0, sendbox.Count);
            receivebox.Add(sendbox[tem]);
            sendbox.RemoveAt(tem);
        }
    }

    public void Chang()
    {
        myTest.Clear();
        int tem;
        for (int i = 0; i < 5; i++)
        {
            tem = Random.Range(0, test.Count);
            myTest.Add(test[tem]);
            test.RemoveAt(tem);
        }

        card = "";
        foreach (var i in myTest)
        {
            card += "花色" + i / 13 + "   " + "數字" + ((i % 13) + 1) + "\n";
        }

        cardText.text = card;
        handsText.text = Hands(myTest);
        boxText.text = (test.Count).ToString();

        test.Clear();
        for (int i = 0; i < 52; i++)
        {
            test.Add(i);
        }
        boxText.text = (test.Count).ToString();
    }
}
