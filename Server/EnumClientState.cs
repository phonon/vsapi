﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Server
{
    /// <summary>
    /// The current connection state of a player thats currently connecting to the server
    /// </summary>
    public enum EnumClientState
    {
        Offline = 0,
        Connecting = 1,
        Connected = 2,
        Playing = 3,
    }
}
