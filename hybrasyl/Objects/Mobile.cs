using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybrasyl.Enums;
using Hybrasyl.XSD;

namespace Hybrasyl.Objects
{

    public interface IMobile
    {
        ICreature Target { get; set; }
        Script Script { get; set; }

        bool Pathfind(byte x, byte y);

        bool IsIdle();
        void Awaken();
        void Sleep();
        IMobile Clone();
    }

    public class Monster : Creature, IMobile
    {
        private bool _idle = true;

        private uint _mTarget;

        public ICreature Target
        {
            get
            {
                return World.Objects.ContainsKey(_mTarget) ? (Creature)World.Objects[_mTarget] : null;
            }
            set
            {
                _mTarget = (value as Creature)?.Id ?? 0;
            }
        }

        public override int GetHashCode()
        {
            return (Name.GetHashCode() * Id.GetHashCode()) - 1;
        }

        public bool Pathfind(byte x, byte y)
        {
            var xDelta = Math.Abs(x - X);
            var yDelta = Math.Abs(y - Y);

            if (xDelta > yDelta)
            {
                Walk(x > X ? Direction.East : Direction.West);
            }

            return false;
        }

        public void ShowTo(VisibleObject obj)
        {
            if (obj is User)
            {
                var user = obj as User;
                user.SendVisibleCreature(this);
            }
        }

        public bool IsIdle()
        {
            return _idle;
        }

        public void Awaken()
        {
            _idle = false;
            //add to alive monsters?
        }

        public void Sleep()
        {
            _idle = true;
            //return to idle state
        }

        public IMobile Clone()
        {
            return MemberwiseClone() as IMobile;
        }


    }


}
