using System.Collections.Generic;

using SharpDX;

namespace BeAwarePlus.Data
{
    internal class Colors
    {
        public List<Vector3> Vector3ToID { get; } = new List<Vector3>()
        {
            new Vector3(0.2f, 0.4588236f, 1),
            new Vector3(0.4f, 1, 0.7490196f),
            new Vector3(0.7490196f, 0, 0.7490196f),
            new Vector3(0.9529412f, 0.9411765f, 0.04313726f),
            new Vector3(1, 0.4196079f, 0),
            new Vector3(0.9960785f, 0.5254902f, 0.7607844f),
            new Vector3(0.6313726f, 0.7058824f, 0.2784314f),
            new Vector3(0.3960785f, 0.8509805f, 0.9686275f),
            new Vector3(0, 0.5137255f, 0.1294118f),
            new Vector3(0.6431373f, 0.4117647f, 0)
        };
    }
}
