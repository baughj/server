using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybrasyl.Dialogs;
using Hybrasyl.Enums;
using Hybrasyl.XSD;

namespace Hybrasyl.Objects
{
    public interface IMerchant : IVisibleObject
    {
        MerchantJob Jobs { get; }
        Dictionary<string, XSD.ItemType> Inventory { get; }
    }

}
