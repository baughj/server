using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Hybrasyl.Objects
{
    /// <summary>
    /// Due to Door's refusal to not suck, it needs to be stuck in the quadtree.
    /// So here it is as a VisibleObject subclass. It needs to be rewritten to use the
    /// Merchant / Signpost onClick way of doing things.
    /// </summary>
    public class Door : VisibleObject
    {
        public new static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool Closed { get; set; }
        public bool IsLeftRight { get; set; }
        public bool UpdateCollision { get; set; }

        public Door(byte x, byte y, bool closed = false, bool isLeftRight = false, bool updateCollision = true)
        {
            X = x;
            Y = y;
            Closed = closed;
            IsLeftRight = isLeftRight;
            UpdateCollision = updateCollision;
        }

        public override void OnClick(IPlayer invoker)
        {
            invoker.Map.ToggleDoors(X, Y);
        }

        public override void AoiEntry(IVisibleObject obj)
        {
            ShowTo(obj);
        }

        public override void ShowTo(IVisibleObject obj)
        {
            if (!(obj is User)) return;
            var user = (User)obj;
            user.SendDoorUpdate(X, Y, Closed,
                IsLeftRight);
        }
    }





}
