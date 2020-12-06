using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

using FrooxEngine;
using FrooxEngine.LogiX;
using NeosMotionCapture.Encoding;
using BaseX;

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

        public readonly SyncRef<StaticBinary> Output;

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
                Save();
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
            string fileName = DateTime.UtcNow.ToString("yyyy-dd-M--HH-mm-ss") + ".bvh";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            UniLog.Log("Recording Name: " + fileName);
            UniLog.Log("Temp Path: " + Path.GetTempPath());
            UniLog.Log("Recording File Path: " + filePath);

            FileStream fs = File.Create(filePath);
            Encoder.EncodeAnimationSet(RecordingCache, fs);
            fs.Close();


            // Load the new asset into Neos DB.
            Uri uri = Engine.LocalDB.ImportLocalAsset(filePath, LocalDB.ImportLocation.Move);
            Binary binary = new Binary();
            binary.SetURL(uri);
            UniLog.Log("Recording URL: " + uri.ToString());

            // Spawn into world.
            Output.Target.URL.Value = uri;

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

        protected override void OnAwake()
        {
            base.OnAwake();
            FrameRate.Value = 30;
        }

        protected static float FrameTimeMillis(float frameRate)
        {
            return 1000 / frameRate;
        }
    }
}
