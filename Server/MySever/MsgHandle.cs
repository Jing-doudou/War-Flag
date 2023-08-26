using System;

namespace MySever
{
    internal class MsgHandle
    {
        public static void MsgEnter(ClientState c, string msgArgs)
        {
            c.player.pname = msgArgs;
            string sendStr = "Enter|";
            Console.WriteLine("玩家" + msgArgs + "上线");
            Program.Send(c, sendStr);
        }
        public static void MsgAV(ClientState c, string msgArgs)
        {
            ClientState other = Program.GetOpp(c);
            Program.Send(other, "AV|" + msgArgs);
        }
        public static void MsgLose(ClientState c, string msgArgs)
        {
            c.player.isPlay = false;
            ClientState opp = Program.GetOpp(c);
            opp.player.isPlay = false;
            foreach (ClientState item in Program.clients.Values)
            {
                Program.Send(item, "Lose|" + msgArgs);
            }
        }
        public static void MsgPieceAV(ClientState c, string msgArgs)
        {
            ClientState other = Program.GetOpp(c);
            Program.Send(other, "PieceAV|" + msgArgs);
        }
        public static void MsgPieceHp(ClientState c, string msgArgs)
        {
            ClientState other = Program.GetOpp(c);
            Program.Send(other, "PieceHp|" + msgArgs);
        }
        public static void MsgPlay(ClientState c, string msgArgs)
        {
            string sendStr = "Play|" + msgArgs + "_" + (int)c.player.type;
            foreach (ClientState cs in Program.clients.Values)
            {
                Program.Send(cs, sendStr);
                Console.WriteLine("发送：" + sendStr);
            }
            Console.WriteLine("++++++++++++++++++++++++++++++");
            //判断这步输赢ChessLogic.PlayChess(c, msgArgs);
            //
        }
        public static void MsgMove(ClientState c, string msgArgs)
        {
            ClientState other = Program.GetOpp(c);
            Program.Send(other, "Move|" + msgArgs);
        }
        public static void MsgAttack(ClientState c, string msgArgs)
        {
            ClientState other = Program.GetOpp(c);
            Program.Send(other, "Attack|" + msgArgs);
        }
        public static void MsgNext(ClientState c, string msgArgs)
        {
            ClientState other = Program.GetOpp(c);
            Program.Send(other, "Next|");
        }

        public static void MsgGameStart(ClientState c, string msgArgs)
        {
            c.player.isPlay = true;
            if (Program.clients.Count < 2)
            {
                return;
            }
            ClientState opp = Program.GetOpp(c);

            if (!opp.player.isPlay)
            {
                return;
            }
            c.player.type = 3 - c.player.type;
            opp.player.type = 3 - c.player.type;
            foreach (ClientState item in Program.clients.Values)
            {
                ClientState other = Program.GetOpp(item);
                Program.Send(item, "GameStart|" + (int)item.player.type + "_" + other.player.pname);
            }
            //初始化服务器棋子信息ChessLogic.Init();
        }
    }
}
