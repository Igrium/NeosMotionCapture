using System;
using System.Collections.Generic;
using System.Text;

using FrooxEngine;
using FrooxEngine.LogiX;


/// <summary>
/// Component responsible for performing motion capture on a list of 
/// </summary>
namespace NeosMotionCapture
{
    [Category("Transform")]
    class MotionRecorder : Component
    {
        /// <summary>
        /// All of the children of these slots will get recorded.
        /// </summary>
        public readonly SyncRefList<Slot> RecordedSlots = new SyncRefList<Slot>();

        /// <summary>
        /// The frame rate to record in.
        /// </summary>
        public readonly Sync<float> FrameRate;

        /// <summary>
        /// Whether this component is recording on any client.
        /// </summary>
        public readonly Sync<bool> IsRecording;

        public readonly Sync<string> Output;

        /// <summary>
        /// Whether we're recording on this client.
        /// </summary>
        public Boolean ClientRecording { get; private set; }
        private DateTime lastUpdate = DateTime.UtcNow;
        public readonly List<AnimationFile> RecordingCache = new List<AnimationFile>();

        [ImpulseTarget]
        public void StartRecording()
        {
            if (!IsRecording.Value)
            {
                RecordingCache.Clear();
                foreach (Slot slot in RecordedSlots)
                {
                    RecordingCache.Add(new AnimationFile(slot, this.Slot));
                }

                IsRecording.Value = true;
                ClientRecording = true;
            }
        }

        [ImpulseTarget]
        public void StopRecording()
        {
            if (ClientRecording)
            {
                ClientRecording = false;
                IsRecording.Value = false;
                Output.Value = "I didn't crash!";
            }
        }


        public void CaptureFrame()
        {
            
        }

        protected override void OnCommonUpdate()
        {
            base.OnCommonUpdate();

            if (ClientRecording)
            {
                double deltaT = (DateTime.UtcNow - lastUpdate).TotalMilliseconds;
                if (deltaT > FrameTimeMillis(FrameRate.Value))
                {
                    CaptureFrame();
                    lastUpdate = DateTime.UtcNow;
                }
            }
        }

        protected static float FrameTimeMillis(float frameRate)
        {
            return 1000 / frameRate;
        }
    }
}
