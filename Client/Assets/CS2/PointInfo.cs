using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;

public class PointInfo : MonoBehaviour
{
    public string pointName;
    public Image image;
    public Text text;
    public PieceBase target;//此点上是否有piece
    [SerializeField] int? actionValue = null;
    public List<int> actionValueList = new List<int>();//存储此时棋子在此点的剩余行动力
    public Button button;//piece被移动的位置

    public void ActionValueListAdd(int value)
    {
        //相同值则不用加入
        //
        actionValueList.Add(value);
        ActionValue = actionValueList.Max();
    }
    public void ActionValueListClear()
    {
        actionValueList.Clear();
        ActionValue = null;
    }

    public int? ActionValue
    {
        get
        {
            return actionValue;
        }
        set
        {
            if (actionValue == value)
            {
                return;
            }
            actionValue = value;
            if (value != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(MovePiece);
                image.color = Color.white;
                text.text = actionValue.ToString();
                //值为0时则不继续调用附近的此函数
                if ((int)actionValue == 0)
                {
                    return;
                }
                ActivePower((int)actionValue);
            }
            else
            {
                text.text = " ";
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(UpdateMap);
                image.color = Color.black;
            }
        }
    }
    public void MovePiece()
    {
        string[] pindex = pointName.Split('-');
        int pX = int.Parse(pindex[0]);
        int pY = int.Parse(pindex[1]);
        //发送移动协议
        NetManager.Send("Move|" + PieceMain.selectPoint.Info + "_" + pointName+"_"+ (int)ActionValue);
        PieceMain.selectPoint.Info = pX + "-" + pY;
        PieceMain.selectPoint.ActiveValue = (int)ActionValue;
        PieceMain.selectPoint = null;
        UpdateMap();
    }
    public void UpdateMap()
    {
        if (GameObject.Find("MyPanel").transform.childCount != 0)
        {
            Destroy(GameObject.Find("MyPanel").transform.GetChild(0).gameObject);
        }
        //刷新地图
        for (int i = 0; i < PieceMain.x; i++)
        {
            for (int j = 0; j < PieceMain.y; j++)
            {
                PieceMain.point[i, j].ActionValueListClear();
            }
        }
        PieceMain.selectPoint = null;
    }
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            UpdateMap();
        });
        image = GetComponent<Image>();
        text = transform.Find("ActivePower").GetComponent<Text>();
    }

    /// <summary>
    /// 显示剩余active
    /// </summary>
    protected void ActivePower(int value)
    {
        ActionValueListAdd(value);
        //向四方传递行动力-1，如果有piece的不传递
        string[] pindex = pointName.Split('-');
        int pX = int.Parse(pindex[0]);
        int pY = int.Parse(pindex[1]);
        //行动力为0时停止扩散
        if (value == 0)
        {
            return;
        }
        if (pX != 0 && (PieceMain.point[pX - 1, pY].target == null))
        {
            PieceMain.point[pX - 1, pY].ActionValueListAdd((int)ActionValue - 1);
        }
        if (pY != 0 && (PieceMain.point[pX, pY - 1].target == null))
        {
            PieceMain.point[pX, pY - 1].ActionValueListAdd((int)ActionValue - 1);
        }
        if (pX != PieceMain.x - 1 && (PieceMain.point[pX + 1, pY].target == null))
        {
            PieceMain.point[pX + 1, pY].ActionValueListAdd((int)ActionValue - 1);
        }
        if (pY != PieceMain.y - 1 && (PieceMain.point[pX, pY + 1].target == null))
        {
            PieceMain.point[pX, pY + 1].ActionValueListAdd((int)ActionValue - 1);
        }
    }

}
