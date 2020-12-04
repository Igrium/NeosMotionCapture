using System;
using System.Collections.Generic;
using System.Text;

using FrooxEngine;

/// <summary>
/// Component responsible for performing motion capture on a list of 
/// </summary>
namespace NeosMotionCapture
{
    [Category("transform")]
    class MotionRecorder : Component
    {
        /// <summary>
        /// All of the children of these slots will get recorded.
        /// </summary>
        public readonly SyncRefList<Slot> RecordedSlots;
    }
}
