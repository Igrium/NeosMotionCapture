using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeosMotionCapture.Encoding
{
    /// <summary>
    /// Encodes multiple animations into one file.
    /// </summary>
    interface IAnimationSetEncoder
    {
        /// <summary>
        /// The animation encoder to use on individual animations (if supported).
        /// </summary>
        IAnimationEncoder Encoder
        {
            set;
            get;
        }

        /// <summary>
        /// Encode a set of animations.
        /// </summary>
        /// <param name="animation">Animations to encode.</param>
        /// <param name="stream">Stream to encode to (assumed to be a full file)</param>    
        void EncodeAnimationSet(List<AnimationFile> animations, Stream stream);
    }
}
