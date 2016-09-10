/*
 * This file is part of Project Hybrasyl.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the Affero General Public License as published by
 * the Free Software Foundation, version 3.
 *
 * This program is distributed in the hope that it will be useful, but
 * without ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the Affero General Public License
 * for more details.
 *
 * You should have received a copy of the Affero General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * (C) 2013 Justin Baugh (baughj@hybrasyl.com)
 * (C) 2015 Project Hybrasyl (info@hybrasyl.com)
 *
 * Authors:   Justin Baugh  <baughj@hybrasyl.com>
 *            Kyle Speck    <kojasou@hybrasyl.com>
 */

using C3;
using Hybrasyl.Dialogs;
using Hybrasyl.Enums;
using Hybrasyl.XSD;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Community.CsharpSqlite;

namespace Hybrasyl.Objects
{

    public interface IWorldObject
    {
        byte X { get; set; }
        byte Y { get; set; }
        uint Id { get; set; }
        string Name { get; set; }
        World World { get; }
        Map Map { get; }
        Rectangle Rect { get; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public abstract class WorldObject : IQuadStorable, IWorldObject
    {
        public static readonly ILog Logger =
            LogManager.GetLogger(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The rectangle that defines the object's boundaries.
        /// </summary>
        public Rectangle Rect => new Rectangle(X, Y, 1, 1);

        public bool HasMoved { get; set; }

        public byte X { get; set; }
        public byte Y { get; set; }
        public uint Id { get; set; }
        public Map Map { get; set; }


        [JsonProperty]
        public string Name { get; set; }

        public Script Script { get; set; }
        public World World { get; set; }

        protected WorldObject()
        {
            Name = string.Empty;
        }

        public virtual void SendId()
        {
        }
    }
}