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
    public PieceBase target;//�˵����Ƿ���piece
    [SerializeField] int? actionValue = null;
    public List<int> actionValueList = new List<int>();//�洢��ʱ�����ڴ˵��ʣ���ж���
    public Button button;//piece���ƶ���λ��

    public void ActionValueListAdd(int value)
    {
        //��ֵͬ���ü���
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
                //ֵΪ0ʱ�򲻼������ø����Ĵ˺���
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
        //�����ƶ�Э��
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
        //ˢ�µ�ͼ
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
    /// ��ʾʣ��active
    /// </summary>
    protected void ActivePower(int value)
    {
        ActionValueListAdd(value);
        //���ķ������ж���-1�������piece�Ĳ�����
        string[] pindex = pointName.Split('-');
        int pX = int.Parse(pindex[0]);
        int pY = int.Parse(pindex[1]);
        //�ж���Ϊ0ʱֹͣ��ɢ
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
