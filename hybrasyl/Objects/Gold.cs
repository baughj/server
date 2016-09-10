using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hybrasyl.Objects
{
    public class Gold : VisibleObject
    {
        public uint Amount { get; set; }

        public new string Name
        {
            get
            {
                if (Amount == 1) return "Silver Coin";
                else if (Amount < 100) return "Gold Coin";
                else if (Amount < 1000) return "Silver Pile";
                else return "Gold Pile";
            }
        }

        public new ushort Sprite
        {
            get
            {
                if (Amount == 1) return 138;
                else if (Amount < 100) return 137;
                else if (Amount < 1000) return 141;
                else return 140;
            }
        }

        public Gold(uint amount)
        {
            Amount = amount;
        }

        public override void OnClick(IPlayer player) { }

        public override void ShowTo(IVisibleObject obj)
        {
            if (!(obj is User)) return;
            var user = (User)obj;
            user.SendVisibleGold(this);
        }
    }

}
