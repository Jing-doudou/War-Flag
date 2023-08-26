
using UnityEngine;
using UnityEngine.UI;

public class PieceInit : PieceBase
{
    public void Init(string _info, int at, int hpm, int avm, ChessTurn _type, string text, Color color)
    {
        info = _info;
        Info = info;
        AttackValue = at;
        hpMax = hpm;
        hp = hpMax;
        activeMaxValue = avm;
        type = _type;
        activeValue = activeMaxValue;
        Text.text = text;
        image.color = color;
        if (PieceMain.me == _type)
        {
            PieceMain.myPieceList.Add(this);
        }
    }
    new void Start()
    {
        base.Start();
    }
}
