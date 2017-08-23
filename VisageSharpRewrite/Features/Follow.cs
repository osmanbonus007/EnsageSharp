using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using System.Collections.Generic;
using VisageSharpRewrite.Abilities;

namespace VisageSharpRewrite.Features
{
    public class Follow
    {

        private Hero me
        {
            get
            {
                return Variables.Hero;
            }          
        }

        private FamiliarControl familiarControl
        {
            get
            {
                return Variables.familiarControl;
            }
        }

        public Follow()
        {

        }

        public void Execute(List<Unit> familiars)
        {
            
            if (!familiarControl.AnyFamiliarNearMe(familiars, 1000))
            {
                if (Utils.SleepCheck("fmove"))
                {
                    foreach (var f in familiars)
                    {
                        if (f.CanMove())
                        {
                            f.Follow(me);
                        }
                    }
                    Utils.Sleep(1000, "fmove");
                }
            }
            
        }

        public void PlayerExecution(ExecuteOrderEventArgs args, List<Unit> familiars)
        {
            
            if (familiarControl.AnyFamiliarNearMe(familiars, 1000))
            {
                if (args.OrderId == OrderId.MoveLocation && Utils.SleepCheck("fsmove"))
                {
                    foreach (var f in familiars)
                    {
                        if (f.CanMove())
                        {
                            f.Move(Game.MousePosition);
                        }
                    }
                    Utils.Sleep(1000, "fsmove");
                }
            }
            
        }


    }
}
