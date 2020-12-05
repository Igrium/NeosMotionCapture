using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeosMotionCapture.Encoding
{
    /// <summary>
    /// Encodes only the first item in an animation set.
    /// FOR TESTING ONLY! Has no practical use.
    /// </summary>
    class SingleAnimationSetEncoder : IAnimationSetEncoder
    {
        public IAnimationEncoder Encoder { get { return _encoder; } set { _encoder = value; } }
        private IAnimationEncoder _encoder = new BVHAnimationEncoder();

        public void EncodeAnimationSet(List<AnimationFile> animations, Stream stream)
        {
            Encoder.EncodeAnimation(animations[0], stream);
        }
    }
}
