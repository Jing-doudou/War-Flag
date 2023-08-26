using System;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum ChessTurn
{
    None, White, Black
}
public class Main : MonoBehaviour
{
    private static ChessTurn ct = ChessTurn.Black;
    Transform bg;
    Button btnStart;
    Transform ready;

    PieceMain PM;

    Transform loginPanel;
    Button btnLogin;
    Button btnExit;
    InputField inputText;
    Transform resultPanel;
    Text resultText;
    Button btnClose;
    Button btnQuit;
    public static bool isStart;
    Socket socket;

    public static ChessTurn Ct
    {
        get => ct; set
        {
            ct = value;
            PieceMain.next.gameObject.SetActive(PieceMain.me == ct);
        }
    }

    private void Start()
    {
        Init();
        NetManager.Connect("127.0.0.1", 8888);
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Play", OnPlay);
        NetManager.AddListener("GameStart", OnGameStart);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("Lose", OnResult);
        NetManager.AddListener("Next", OnNext);
        NetManager.AddListener("Move", MovePiece);
        NetManager.AddListener("AV", UpdateAV);
        NetManager.AddListener("PieceAV", AddPieceAV);
        NetManager.AddListener("PieceHp", AddPieceHp);
        NetManager.AddListener("Attack", PieceAttack);
    }

    private void PieceAttack(string str)
    {
        PieceBase p1 = null, p2 = null;
        string[] s = str.Split('_');
        string[] pLastindex = s[0].Split('-');
        int pLX = int.Parse(pLastindex[0]);
        int pLY = int.Parse(pLastindex[1]);
        string pAtt = (PieceMain.x - 1 - pLX).ToString() + "-" + (PieceMain.y - 1 - pLY).ToString();

        string[] pindex = s[1].Split('-');
        int pX = int.Parse(pindex[0]);
        int pY = int.Parse(pindex[1]);
        string pHurt = (PieceMain.x - 1 - pX).ToString() + "-" + (PieceMain.y - 1 - pY).ToString();
        for (int i = 0; i < PM.preParent.transform.childCount; i++)
        {
            PieceBase piece = PM.preParent.transform.GetChild(i).GetComponent<PieceBase>();
            if (piece.Info == pAtt)
            {
                p1 = piece;
            }
            if (piece.Info == pHurt)
            {
                p2 = piece;
            }
        }
        p2.AttackPiece(p1);
    }

    private void AddPieceHp(string str)
    {
        string[] s = str.Split('_');
        string[] pLastindex = s[0].Split('-');
        int pLX = int.Parse(pLastindex[0]);
        int pLY = int.Parse(pLastindex[1]);
        string pStart = (PieceMain.x - 1 - pLX).ToString() + "-" + (PieceMain.y - 1 - pLY).ToString();
        for (int i = 0; i < PM.preParent.transform.childCount; i++)
        {
            PieceBase piece = PM.preParent.transform.GetChild(i).GetComponent<PieceBase>();
            if (piece.Info == pStart)
            {
                piece.Hp += 10;
            }
        }
    }

    private void UpdateAV(string str)
    {
        PieceMain.EnemyAV = int.Parse(str);
    }

    private void AddPieceAV(string str)
    {
        string[] s = str.Split('_');
        string[] pLastindex = s[0].Split('-');
        int pLX = int.Parse(pLastindex[0]);
        int pLY = int.Parse(pLastindex[1]);
        string pStart = (PieceMain.x - 1 - pLX).ToString() + "-" + (PieceMain.y - 1 - pLY).ToString();
        for (int i = 0; i < PM.preParent.transform.childCount; i++)
        {
            PieceBase piece = PM.preParent.transform.GetChild(i).GetComponent<PieceBase>();
            if (piece.Info == pStart)
            {
                piece.ActiveValue++;
            }
        }
    }

    private void OnNext(string str)
    {
        Ct = 3 - Ct;
    }
    private void MovePiece(string str)
    {
        string[] s = str.Split('_');
        string[] pLastindex = s[0].Split('-');
        int pLX = int.Parse(pLastindex[0]);
        int pLY = int.Parse(pLastindex[1]);
        string pStart = (PieceMain.x - 1 - pLX).ToString() + "-" + (PieceMain.y - 1 - pLY).ToString();

        string[] pindex = s[1].Split('-');
        int pX = int.Parse(pindex[0]);
        int pY = int.Parse(pindex[1]);
        string pStop = (PieceMain.x - 1 - pX).ToString() + "-" + (PieceMain.y - 1 - pY).ToString();
        int av = int.Parse(s[2]);
        for (int i = 0; i < PM.preParent.transform.childCount; i++)
        {
            PieceBase piece = PM.preParent.transform.GetChild(i).GetComponent<PieceBase>();
            if (piece.Info == pStart)
            {
                piece.Info = pStop;
                piece.ActiveValue = av;
            }
        }

    }
    private void OnResult(string str)
    {
        PieceMain.AV = PieceMain.EnemyAV = 0;
        if (PieceMain.me.ToString() == str)
        {

            resultText.text = "菜";
            resultText.color = Color.blue;
        }
        else
        {
            resultText.text = "巧";
            resultText.color = Color.yellow;

        }
        resultPanel.gameObject.SetActive(true);
    }

    private void OnLeave(string str)
    {
        resultText.text = "妙";
        resultText.color = Color.yellow;
        resultPanel.gameObject.SetActive(true);
    }

    private void OnGameStart(string str)
    {
        Debug.Log("收到开始协议");
        ready.gameObject.SetActive(false);
        isStart = true;
        btnStart.interactable = false;
        string[] s = str.Split('_');
        PieceMain.me = (ChessTurn)int.Parse(s[0]);
        Ct = ChessTurn.Black;
        PieceMain.enemyNameText.text = "敌人"+s[1]+"的行动力：";
        PM.Test();
        PieceMain.AV = 10;
    }

    private void OnPlay(string str)
    {
        string[] s = str.Split('_');
        Ct = (ChessTurn)(3 - int.Parse(s[2]));
    }

    private void OnEnter(string str)
    {
        loginPanel.gameObject.SetActive(false);
    }

    private void Init()
    {
        PM = GameObject.Find("BackGround").GetComponent<PieceMain>();
        resultPanel = transform.Find("ResultPanel");
        resultText = FindType<Text>(resultPanel, "ResultText");
        btnClose = FindType<Button>(resultPanel, "BtnClose");
        btnClose.onClick.AddListener(() =>
        {
            ReSsut();
            resultPanel.gameObject.SetActive(false);
        });
        loginPanel = transform.Find("LoginPanel");
        btnLogin = FindType<Button>(loginPanel, "LoginBtn");
        inputText = FindType<InputField>(loginPanel, "InputField");
        btnLogin.onClick.AddListener(() =>
        {
            if (inputText.text == "")
            {
                return;
            }
            PieceMain.myName = inputText.text;
            NetManager.Send("Enter|" + PieceMain.myName);
        });
        btnExit = FindType<Button>(loginPanel, "ExitBtn");
        btnExit.onClick.AddListener(() =>
        {
            Application.Quit();
        });


        bg = transform.Find("BG");
        btnStart = FindType<Button>(bg, "StartBtn");
        ready = bg.Find("Ready");
        btnStart.onClick.AddListener(() =>
        {
            PieceMain.myNameText.text = PieceMain.myName + "的行动力：";
            ready.gameObject.SetActive(true);
            NetManager.Send("GameStart|");
        });
        btnQuit = FindType<Button>(bg, "BtnQuit");
        btnQuit.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void Update()
    {
        NetManager.Updata();
    }

    public static T FindType<T>(Transform t, string n)
    {
        return t.Find(n).GetComponent<T>();
    }
    private void ReSsut()
    {
        isStart = false;
        //me.SetId();
        btnStart.interactable = true;
        Ct = ChessTurn.Black;
        ////第二局初始化
        //PM.Test();
    }
}
