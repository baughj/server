using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybrasyl.Dialogs;

namespace Hybrasyl.Objects
{
    public interface IInteractable
    {
        void DisplayPursuits(IPlayer invoker);
        void ResetPursuits();
        void AddPursuit(DialogSequence pursuit);
        void RegisterDialogSequence(DialogSequence sequence);
        List<DialogSequence> Pursuits { get; }
        List<DialogSequence> DialogSequences { get; }
        Dictionary<string, DialogSequence> SequenceCatalog { get; }
    }

    public abstract class InteractableObject : VisibleObject, IInteractable
    {

        public List<DialogSequence> Pursuits { get; }
        public List<DialogSequence> DialogSequences { get; }
        public Dictionary<string, DialogSequence> SequenceCatalog { get; }

        protected InteractableObject()
        {
            Pursuits = new List<DialogSequence>();
            DialogSequences = new List<DialogSequence>();
            SequenceCatalog = new Dictionary<string, DialogSequence>();
        }

        public void ResetPursuits()
        {
            Pursuits.Clear();
            DialogSequences.Clear();
            SequenceCatalog.Clear();
        }

        public void AddPursuit(DialogSequence pursuit)
        {
            if (pursuit.Id == null)
            {
                // This is a local sequence, so assign it into the pursuit range and
                // assign an ID
                pursuit.Id = (uint) (Constants.DIALOG_SEQUENCE_SHARED + Pursuits.Count);
                Pursuits.Add(pursuit);
            }
            else
            {
                // This is a shared sequence
                Pursuits.Add(pursuit);
            }

            if (SequenceCatalog.ContainsKey(pursuit.Name))
            {
                Logger.WarnFormat("Pursuit {0} is being overwritten", pursuit.Name);
                SequenceCatalog.Remove(pursuit.Name);

            }

            SequenceCatalog.Add(pursuit.Name, pursuit);

            if (pursuit.Id > Constants.DIALOG_SEQUENCE_SHARED)
            {
                pursuit.AssociateSequence(this);
            }
        }

        public void RegisterDialogSequence(DialogSequence sequence)
        {
            sequence.Id = (uint) (Constants.DIALOG_SEQUENCE_PURSUITS + DialogSequences.Count);
            DialogSequences.Add(sequence);
            if (SequenceCatalog.ContainsKey(sequence.Name))
            {
                Logger.WarnFormat("Dialog sequence {0} is being overwritten", sequence.Name);
                SequenceCatalog.Remove(sequence.Name);

            }
            SequenceCatalog.Add(sequence.Name, sequence);
        }

        public abstract void DisplayPursuits(IPlayer invoker);
    }
}
