using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hybrasyl.Objects
{
    public class Reactor : InteractableObject
    {
        public bool Ready;
        public Reactor() { }

        public void OnSpawn()
        {
            // Do we have a script?
            /*
                        Script thescript;
                        if (_reactor.script_name == String.Empty)
                            Game.World.ScriptProcessor.TryGetScript(_reactor.name, out thescript);
                        else
                            Game.World.ScriptProcessor.TryGetScript(_reactor.script_name, out thescript);

                        if (thescript == null)
                        {
                            Logger.WarnFormat("reactor {0}: script not found", _reactor.name);
                            return;
                        }

                        Script = thescript;

                        Script.AssociateScriptWithObject(this);

                        if (!Script.InstantiateScriptable())
                        {
                            Logger.WarnFormat("reactor {0}: script instantiation failed", _reactor.name);
                            return;
                        }

                        Script.ExecuteScriptableFunction("OnSpawn");
                        Ready = true;
             */
        }

        public override void OnClick(IPlayer player) { }
        public override void DisplayPursuits(IPlayer player) { }

        public void OnEntry(WorldObject obj)
        {
            if (Ready)
                Script.ExecuteScriptableFunction("OnEntry", Script.GetObjectWrapper(obj));
        }

        public void AoiEntry(WorldObject obj)
        {
            if (Ready)
                Script.ExecuteScriptableFunction("OnAoiEntry", Script.GetObjectWrapper(obj));
        }

        public void OnLeave(WorldObject obj)
        {
            if (Ready)
                Script.ExecuteScriptableFunction("OnLeave", Script.GetObjectWrapper(obj));
        }

        public void AoiDeparture(WorldObject obj)
        {
            if (Ready)
                Script.ExecuteScriptableFunction("OnAoiDeparture", Script.GetObjectWrapper(obj));
        }

        public void OnDrop(WorldObject obj, WorldObject dropped)
        {
            if (Ready)
                Script.ExecuteScriptableFunction("OnDrop", Script.GetObjectWrapper(obj),
                    Script.GetObjectWrapper(dropped));
        }
    }

}
