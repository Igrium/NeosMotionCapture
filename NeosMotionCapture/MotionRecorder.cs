using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

using FrooxEngine;
using FrooxEngine.LogiX;
using NeosMotionCapture.Encoding;

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
        public readonly Sync<float> FrameRate = new Sync<float> { Value = 30 };

        /// <summary>
        /// Whether this component is recording on any client.
        /// </summary>
        public readonly Sync<bool> IsRecording;

        public readonly Sync<Binary> Output;

        /// <summary>
        /// Whether we're recording on this client.
        /// </summary>
        public Boolean ClientRecording { get; private set; }

        private DateTime lastUpdate = DateTime.UtcNow;

        public readonly List<AnimationFile> RecordingCache = new List<AnimationFile>();

        public IAnimationSetEncoder Encoder = new SingleAnimationSetEncoder();

        [ImpulseTarget]
        public void StartRecording()
        {
            if (!IsRecording.Value)
            {
                RecordingCache.Clear();
                foreach (Slot slot in RecordedSlots)
                {
                    RecordingCache.Add(new AnimationFile(slot, this.Slot) { FrameTimeMillis = FrameTimeMillis(FrameRate.Value) });
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
            }
        }

        public void CaptureFrame()
        {
            foreach (AnimationFile animation in RecordingCache)
            {
                animation.Capture();
            }
        }
        public void Save()
        {
            // Encode and save to file.
            string fileName = DateTime.UtcNow.ToString() + ".bvh";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            FileStream fs = File.Create(filePath);
            Encoder.EncodeAnimationSet(RecordingCache, fs);
            fs.Close();

            // Load the new asset into Neos.
            Uri uri = Engine.LocalDB.ImportLocalAsset(filePath, LocalDB.ImportLocation.Move);
            Binary asset = new Binary();
            asset.SetURL(uri);
            Output.Value = asset;
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
