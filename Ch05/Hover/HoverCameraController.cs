using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Hover
{
    public class HoverCameraController : MouseCameraController
    {
        // New properties.
        public const double CameraDY = 0.5;

        // Constructor.
        public HoverCameraController(PerspectiveCamera camera, Viewport3D viewport,
            UIElement keyboardControl)
            : base(camera, viewport, keyboardControl)
        {
        }

        // Update the camera's position and orientation.
        protected override void DoShift()
        {
            CameraPosition = new Point3D(
                CameraPosition.X,
                CameraPosition.Y - CameraDY,
                CameraPosition.Z);
        }
        protected override void DoSpace()
        {
            CameraPosition = new Point3D(
                CameraPosition.X,
                CameraPosition.Y + CameraDY,
                CameraPosition.Z);
        }
    }
}
