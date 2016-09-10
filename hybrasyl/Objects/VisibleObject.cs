using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybrasyl.Enums;
using log4net;

namespace Hybrasyl.Objects
{
    public interface IVisibleObject : IWorldObject
    {
        Direction Direction { get; set; }
        ushort Sprite { get; set; }
        string Portrait { get; set; }
        string DisplayText { get; set; }

        DeathPileInfo DeathPile { get; set; }

        void AoiEntry(IVisibleObject obj);
        void AoiDeparture(IVisibleObject obj);
        void OnClick(IPlayer obj);
        Rectangle GetBoundingBox();
        Rectangle GetViewport();
        Rectangle GetShoutViewport();

        void Show();
        void ShowTo(IVisibleObject obj);
        void Hide();

        void HideFrom(IVisibleObject obj);
        void Remove();
        void Teleport(ushort mapid, byte x, byte y);
        void Teleport(string name, byte x, byte y);
        int Distance(IWorldObject obj);
        void Say(string message);
        void Shout(string message);
        void Effect(short x, short y, ushort effect, short speed);
        void Effect(ushort effect, short speed);
        void PlaySound(ServerPacket packet);
    }

    public abstract class VisibleObject : WorldObject, IVisibleObject
    {
        public new static readonly ILog Logger =
               LogManager.GetLogger(
               System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Direction Direction { get; set; }
        public ushort Sprite { get; set; }
        public String Portrait { get; set; }
        public string DisplayText { get; set; }
        public DeathPileInfo DeathPile { get; set; }

        protected VisibleObject()
        {
            DisplayText = string.Empty;
        }

        public virtual void AoiEntry(IVisibleObject obj) { }

        public virtual void AoiDeparture(IVisibleObject obj) { }

        public abstract void OnClick(IPlayer invoker);

        public virtual void OnDeath() { }

        public Rectangle GetBoundingBox()
        {
            return new Rectangle(X, Y, 1, 1);
        }

        public Rectangle GetViewport()
        {
            return new Rectangle((X - Constants.VIEWPORT_SIZE / 2),
                (Y - Constants.VIEWPORT_SIZE / 2), Constants.VIEWPORT_SIZE,
                Constants.VIEWPORT_SIZE);
        }

        public Rectangle GetShoutViewport()
        {
            return new Rectangle((X - Constants.VIEWPORT_SIZE),
                (Y - Constants.VIEWPORT_SIZE), Constants.VIEWPORT_SIZE * 2,
                Constants.VIEWPORT_SIZE * 2);
        }

        public virtual void Show()
        {
            var withinViewport = Map.EntityTree.GetObjects(GetViewport());
            Logger.DebugFormat($"WithinViewport contains {withinViewport.Count} objects");

            foreach (var obj in withinViewport)
            {
                Logger.DebugFormat($"Object type is {obj.GetType()} and its name is {obj.Name}");
                obj.AoiEntry(this);
            }
        }

        public virtual void ShowTo(IVisibleObject obj) { }

        public virtual void Hide() { }

        public virtual void HideFrom(IVisibleObject obj) { }

        public virtual void Remove()
        {
            Map.Remove(this);
        }

        public virtual void Teleport(ushort mapid, byte x, byte y)
        {
            if (!World.Maps.ContainsKey(mapid)) return;
            Map?.Remove(this);
            Logger.DebugFormat("Teleporting {0} to {1}.", Name, World.Maps[mapid].Name);
            World.Maps[mapid].Insert(this, x, y);
        }

        public virtual void Teleport(string name, byte x, byte y)
        {
            Map targetMap;
            if (!World.MapCatalog.TryGetValue(name, out targetMap)) return;
            Map?.Remove(this);
            Logger.DebugFormat("Teleporting {0} to {1}.", Name, targetMap.Name);
            targetMap.Insert(this, x, y);
        }

        public virtual void SendMapInfo() { }

        public virtual void SendLocation() { }

        public int Distance(IWorldObject obj)
        {
            return Point.Distance(obj.X, obj.Y, X, Y);
        }

        public virtual void Say(string message)
        {
            foreach (var obj in Map.EntityTree.GetObjects(GetViewport()))
            {
                if (obj is User)
                {
                    var user = obj as User;
                    var x0D = new ServerPacket(0x0D);
                    x0D.WriteByte(0x00);
                    x0D.WriteUInt32(Id);
                    x0D.WriteString8($"{Name}: {message}");
                    user.Enqueue(x0D);
                }
            }
        }

        public virtual void Shout(string message)
        {
            foreach (var obj in Map.EntityTree.GetObjects(GetShoutViewport()))
            {
                if (obj is User)
                {
                    var user = obj as User;
                    var x0D = new ServerPacket(0x0D);
                    x0D.WriteByte(0x01);
                    x0D.WriteUInt32(Id);
                    x0D.WriteString8($"{Name}! {message}");

                    user.Enqueue(x0D);
                }
            }
        }

        public virtual void Effect(short x, short y, ushort effect, short speed)
        {
            foreach (var user in Map.EntityTree.GetObjects(GetViewport()).OfType<User>().Select(obj => obj))
            {
                user.SendEffect(x, y, effect, speed);
            }
        }

        public virtual void Effect(ushort effect, short speed)
        {
            foreach (var user in Map.EntityTree.GetObjects(GetViewport()).OfType<User>().Select(obj => obj))
            {
                user.SendEffect(Id, effect, speed);
            }
        }

        public virtual void PlaySound(ServerPacket packet)
        {
            foreach (var user in Map.EntityTree.GetObjects(GetViewport()).OfType<User>().Select(obj => obj))
            {
                var nPacket = (ServerPacket)packet.Clone();
                user.Enqueue(nPacket);
            }
        }
    }


}
