using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Models
{
    public enum Layer
    {
        Default                 = 0,
        TransparentFx           = 1,
        IgnoreRaycast           = 2,
        EmptyA                  = 3,
        Water                   = 4,
        UI                      = 5,
        EmptyB                  = 6,
        EmptyC                  = 7,

        ModuleBlocking          = 8,
        ModuleNonBlocking       = 9,
        Player                  = 10,
        UITop                   = 11,
    }
}
