using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEditor;

//public enum ChessTurn
//{
//    None, White, Black
//}
public class PieceBase : MonoBehaviour
{
    public string info;
    public int activeValue;
    public int activeMaxValue;
    public Button piece;
    public Vector2 point;

    public int hp;
    public int hpMax;
    private int attackValue;
    public ChessTurn type;
    public Text Text;
    public Image image;
    public GameObject pieceMessagePrefab;
    public string Info
    {
        get => info;
        set
        {
            string[] pLastindex = info.Split('-');
            int pLX = int.Parse(pLastindex[0]);
            int pLY = int.Parse(pLastindex[1]);
            info = value;
            gameObject.name = info;
            string[] pindex = info.Split('-');
            int pX = int.Parse(pindex[0]);
            int pY = int.Parse(pindex[1]);
            point = PieceMain.point[pX, pY].transform.localPosition;
            transform.localPosition = point;
            PieceMain.point[pLX, pLY].target = null;
            PieceMain.point[pX, pY].target = this;
        }
    }

    public int Hp
    {
        get => hp; set
        {
            hp = value;
            if (hp <= 0)
            {
                if (Text.text.ToString() == "将" && type == PieceMain.me)
                {
                    //发送此方失败协议
                    NetManager.Send("Lose|"+PieceMain.me);
                }
                hp = 0;
                PieceMain.myPieceList.Remove(this);
                Destroy(this.gameObject);
            }
        }
    }

    public int AttackValue
    {
        get
        {
            return attackValue;
        }
        set { attackValue = value; }
    }

    public int ActiveValue
    {
        get => activeValue;
        set
        {
            activeValue = value;
        }
    }

    public void Start()
    {
        piece = GetComponent<Button>();
        piece.onClick.AddListener(() =>
        {
            if (Main.Ct != PieceMain.me)
            {
                return;
            }
            if (type != PieceMain.me)
            {
                if (PieceMain.selectPoint == null)
                {
                    //生成敌人piece的信息
                    if (GameObject.Find("MyPanel").transform.childCount != 0)
                    {
                        Destroy(GameObject.Find("MyPanel").transform.GetChild(0).gameObject);
                    }
                    GameObject pieceText = Instantiate(pieceMessagePrefab, GameObject.Find("MyPanel").transform);
                    Text name = pieceText.transform.Find("Name").GetComponent<Text>();
                    Text _activeText = pieceText.transform.Find("ActiveValue").GetComponent<Text>();
                    Text _hpText = pieceText.transform.Find("HP").GetComponent<Text>();
                    name.text = Text.text;
                    _activeText.text = ActiveValue.ToString();
                    _hpText.text = Hp.ToString();
                }
                AttackPieceOnClekc();
                return;
            }
            MovePieceOnClick();

            PieceInfo();
        });
    }
    /// <summary>
    /// 棋子面板的事件添加
    /// </summary>
    public void PieceInfo()
    {
        if (GameObject.Find("MyPanel").transform.childCount != 0)
        {
            Destroy(GameObject.Find("MyPanel").transform.GetChild(0).gameObject);
        }
        GameObject pieceText = Instantiate(pieceMessagePrefab, GameObject.Find("MyPanel").transform);
        Text name = pieceText.transform.Find("Name").GetComponent<Text>();
        name.text = Text.text;

        Text _activeText = pieceText.transform.Find("ActiveValue").GetComponent<Text>();
        _activeText.text = ActiveValue.ToString();

        Button _active = pieceText.transform.Find("ActiveValue").GetComponent<Button>();
        _active.onClick.AddListener(delegate ()
        {
            if (PieceMain.AV == 0 || ActiveValue == activeMaxValue) { return; }
            PieceMain.AV--;
            ActiveValue++;
            MovePieceOnClick();
            _activeText.text = ActiveValue.ToString();
            NetManager.Send("PieceAV|" + Info + "_" + ActiveValue);
        });

        Text _hpText = pieceText.transform.Find("HP").GetComponent<Text>();
        _hpText.text = Hp.ToString();

        Button _hp = pieceText.transform.Find("HP").GetComponent<Button>();
        _hp.onClick.AddListener(delegate ()
        {
            if (PieceMain.AV == 0 || Hp == hpMax) { return; }
            PieceMain.AV--;
            Hp += 10;
            _hpText.text = Hp.ToString();
            NetManager.Send("PieceHp|" + Info + "_" + Hp);
        });
    }
    /// <summary>
    /// 向正下方棋盘传送行动力
    /// </summary>
    public void MovePieceOnClick()
    {
        //清空所有piece点的行动力列表
        for (int i = 0; i < PieceMain.x; i++)
        {
            for (int j = 0; j < PieceMain.y; j++)
            {
                PieceMain.point[i, j].ActionValueListClear();
            }
        }
        //重新开始赋值行动力
        string[] pindex = Info.Split('-');
        int pX = int.Parse(pindex[0]);
        int pY = int.Parse(pindex[1]);
        //将此piece存入main的selectPoint，方便移动时调用
        PieceMain.selectPoint = this;
        PieceMain.point[pX, pY].ActionValueListAdd(ActiveValue);
    }
    /// <summary>
    /// 攻击事件
    /// </summary>
    public void AttackPieceOnClekc()
    {

        if (PieceMain.selectPoint == null)
        {
            return;
        }
        if (PieceMain.selectPoint.ActiveValue == 0)
        {
            return;
        }
        PieceMain.selectPoint.ActiveValue--;
        //清空面板
        if (GameObject.Find("MyPanel").transform.childCount != 0)
        {
            Destroy(GameObject.Find("MyPanel").transform.GetChild(0).gameObject);
        }
        int dis;
        string[] pindex = PieceMain.selectPoint.Info.Split('-');
        int pX = int.Parse(pindex[0]);
        int pY = int.Parse(pindex[1]);
        string[] p2index = Info.Split('-');
        int p2X = int.Parse(p2index[0]);
        int p2Y = int.Parse(p2index[1]);
        dis = Mathf.Abs(pY - p2Y) + Mathf.Abs(pX - p2X);
        if (dis <= 2)
        {
            AttackPiece(PieceMain.selectPoint);
            //发送攻击协议
            NetManager.Send("Attack|" + PieceMain.selectPoint.Info.ToString() + "_" + this.Info.ToString());
        }
    }
    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="p1">攻击者</param> 
    public void AttackPiece(PieceBase p1)
    {
        StartCoroutine(AttAnimator(p1));
        //刷新地图行动力标志
        for (int i = 0; i < PieceMain.x; i++)
        {
            for (int j = 0; j < PieceMain.y; j++)
            {
                PieceMain.point[i, j].ActionValueListClear();
            }
        }
    }
    public IEnumerator AttAnimator(PieceBase p1)
    {
        Vector2 oldP = p1.transform.localPosition;
        while (Vector3.Distance(p1.transform.localPosition, this.transform.localPosition) > 0.2f)
        {
            float x = p1.transform.localPosition.x - (p1.transform.localPosition.x - this.transform.localPosition.x) * 0.2f;
            float y = p1.transform.localPosition.y - (p1.transform.localPosition.y - this.transform.localPosition.y) * 0.2f;
            p1.transform.localPosition = new Vector2(x, y);
            yield return new WaitForFixedUpdate();
        }
        while (Vector3.Distance(p1.transform.localPosition, oldP) > 0.2f)
        {
            float x = p1.transform.localPosition.x - (p1.transform.localPosition.x - oldP.x) * 0.4f;
            float y = p1.transform.localPosition.y - (p1.transform.localPosition.y - oldP.y) * 0.4f;
            p1.transform.localPosition = new Vector2(x, y);
            yield return new WaitForFixedUpdate();
        }
        this.Hp -= p1.AttackValue;
        yield break;
    }
}