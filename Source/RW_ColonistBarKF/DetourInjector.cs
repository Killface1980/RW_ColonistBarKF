using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommunityCoreLibrary;
using RimWorld;

namespace RW_ColonistBarKF
{
    class DetourInjector : SpecialInjector
    {
        public override bool Inject()
        {
            // Detour FloatMenuMaker
            if (!CommunityCoreLibrary.Detours.TryDetourFromTo(typeof(ColonistBar).GetMethod("ColonistsInScreenRect"),
                typeof(ColonistBar_KF).GetMethod("ColonistsInScreenRect")))
                return false;
            return true;

        }

    }
}
