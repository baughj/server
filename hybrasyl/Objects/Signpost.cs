using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hybrasyl.Objects
{
    public class Signpost : VisibleObject
    {
        public string Message { get; set; }
        public bool IsMessageboard { get; set; }
        public string BoardName { get; set; }

        public Signpost(byte postX, byte postY, string message, bool messageboard = false,
            string boardname = null)
        {
            X = postX;
            Y = postY;
            Message = message;
            IsMessageboard = messageboard;
            BoardName = boardname;
        }

        public override void OnClick(IPlayer invoker)
        {
            Logger.DebugFormat("Signpost was clicked");
            if (!IsMessageboard)
            {
                invoker.SendMessage(Message, Message.Length < 1024 ? (byte)MessageTypes.SLATE : (byte)MessageTypes.SLATE_WITH_SCROLLBAR);
            }
            else
            {
                var board = World.GetBoard(BoardName);
                invoker.Enqueue(board.RenderToPacket(true));
            }
        }
    }

}
