using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PieceMain : MonoBehaviour
{
    public static string myName;
    public static ChessTurn me = ChessTurn.White;
    public GameObject pointPrefab;
    public GameObject piecePrefab;//pieceMap test
    public Transform preParent;//pieceµÄ¸¸¼¶
    public static PointInfo[,] point;
    public static int x = 13;
    public static int y = 16;
    public static PieceBase selectPoint;
    public static List<PieceBase> myPieceList = new List<PieceBase>();

    public static Text AVPanelText;
    public static Text enemyAVPanelText;
    public static Text myNameText;
    public static Text enemyNameText;
    private static int aV = 10;
    private static int enemyAV = 10;
    public static Button next;

    public static int AV
    {
        get => aV; set
        {
            aV = value;
            if (aV >= 10)
            {
                aV = 10;
            }
            NetManager.Send("AV|" + aV);
            AVPanelText.text = aV.ToString();
        }
    }

    public static int EnemyAV
    {
        get => enemyAV; set
        {
            enemyAV = value;
            if (enemyAV >= 10)
            {
                enemyAV = 10;
            }
            enemyAVPanelText.text = enemyAV.ToString();
        }
    }

    void Awake()
    {
        CreatPoint();
    }
    private void Start()
    {
        next = GameObject.Find("Next").GetComponent<Button>();
        next.onClick.AddListener(delegate ()
        {
            Main.Ct = 3 - Main.Ct;
            selectPoint = null;
            for (int i = 0; i < PieceMain.x; i++)
            {
                for (int j = 0; j < PieceMain.y; j++)
                {
                    PieceMain.point[i, j].ActionValueListClear();
                }
            }
            if (GameObject.Find("MyPanel").transform.childCount != 0)
            {
                Destroy(GameObject.Find("MyPanel").transform.GetChild(0).gameObject);
            }
            //·¢ËÍ»»ÊÖÐÅÏ¢
            NetManager.Send("Next|"); 
            AV += myPieceList.Count;
        }); 
        AVPanelText = GameObject.Find("MyActive/ActiveValue").GetComponent<Text>();
        enemyAVPanelText = GameObject.Find("EnemyActive/ActiveValue").GetComponent<Text>();
        myNameText = GameObject.Find("MyActive/Name").GetComponent<Text>();
        enemyNameText = GameObject.Find("EnemyActive/Name").GetComponent<Text>();
    }

    public void Test()
    {
        AV = EnemyAV = 10;
        if (preParent.childCount != 0)
        {
            for (int i = 0; i < preParent.childCount; i++)
            {
                Destroy(preParent.GetChild(i).gameObject);
            }
        }
        ChessTurn enemyType = 3 - me;
        GameObject p1 = Instantiate(piecePrefab, preParent);
        p1.GetComponent<PieceInit>().Init("12-0", 30, 200, 5, me, "½«", Color.red);

        GameObject p2 = Instantiate(piecePrefab, preParent);
        p2.GetComponent<PieceInit>().Init("10-0", 20, 90, 2, me, "¼×", Color.red);

        GameObject p21 = Instantiate(piecePrefab, preParent);
        p21.GetComponent<PieceInit>().Init("11-1", 20, 90, 2, me, "¼×", Color.red);

        GameObject p3 = Instantiate(piecePrefab, preParent);
        p3.GetComponent<PieceInit>().Init("12-2", 20, 90, 2, me, "¼×", Color.red);

        GameObject p31 = Instantiate(piecePrefab, preParent);
        p31.GetComponent<PieceInit>().Init("12-1", 10, 50, 3, me, "²ì", Color.red);

        GameObject p32 = Instantiate(piecePrefab, preParent);
        p32.GetComponent<PieceInit>().Init("11-0", 10, 50, 3, me, "²ì", Color.red);
        /////////////////////////////////////////////////////////////////////////
        GameObject p4 = Instantiate(piecePrefab, preParent);
        p4.GetComponent<PieceInit>().Init("0-15", 30, 200, 5, enemyType, "½«", Color.blue);

        GameObject p5 = Instantiate(piecePrefab, preParent);
        p5.GetComponent<PieceInit>().Init("1-15", 10, 50, 3, enemyType, "²ì", Color.blue);

        GameObject p51 = Instantiate(piecePrefab, preParent);
        p51.GetComponent<PieceInit>().Init("0-14", 10, 50, 3, enemyType, "²ì", Color.blue);

        GameObject p6 = Instantiate(piecePrefab, preParent);
        p6.GetComponent<PieceInit>().Init("0-13", 20, 90, 2, enemyType, "¼×", Color.blue);

        GameObject p61 = Instantiate(piecePrefab, preParent);
        p61.GetComponent<PieceInit>().Init("1-14", 20, 90, 2, enemyType, "¼×", Color.blue);

        GameObject p62 = Instantiate(piecePrefab, preParent);
        p62.GetComponent<PieceInit>().Init("2-15", 20, 90, 2, enemyType, "¼×", Color.blue);
    }

    private void CreatPoint()
    {
        point = new PointInfo[x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject pieceMap = Instantiate(pointPrefab, this.transform);
                pieceMap.name = i + "-" + j;
                point[i, j] = pieceMap.GetComponent<PointInfo>();
                point[i, j].pointName = pieceMap.name;
            }
        }
    }

}
