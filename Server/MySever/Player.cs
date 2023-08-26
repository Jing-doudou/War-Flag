using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MySever
{
    internal class Player
    {
        public string pname { get; set; }
        public ChessTurn type { get; set; }
        public bool isPlay { get; set; }
        public int win { get; set; }
        public int lost { get; set; }
        public Socket socket { get; set; }
        public Player(Socket s)
        {
            type = ChessTurn.Black;
            socket = s;
        }
    }
}
