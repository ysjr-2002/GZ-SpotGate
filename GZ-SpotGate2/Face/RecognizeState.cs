﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    /// <summary>
    /// 识别状态
    /// </summary>
    enum RecognizeState
    {
        recognizing,

        recognized,

        unrecognized,

        lastface,

        gone,
    }
}
