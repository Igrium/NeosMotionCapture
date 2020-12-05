using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeosMotionCapture
{
    /// <summary>
    /// Encodes an AnimationFile into an actual animation file.
    /// </summary>
    interface IAnimationEncoder
    {
        
        /// <summary>
        /// Encode an animation.
        /// </summary>
        /// <param name="animation">Animation to encode.</param>
        /// <param name="stream">Stream to encode to (assumed to be a full file)</param>        
        void EncodeAnimation(AnimationFile animation, Stream stream);
    }
}
