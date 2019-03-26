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
    public class MouseCameraController : AutomobileCameraController
    {
        // New properties.
        public double CameraPhi = Math.PI / 2;
        public const double CameraDPhi = Math.PI / 30;
        public const double CameraPhiMin = 0.25 * Math.PI;
        public const double CameraPhiMax = 0.75 * Math.PI;

        // Constructor.
        public MouseCameraController(PerspectiveCamera camera, Viewport3D viewport,
            UIElement keyboardControl)
            : base(camera, viewport, keyboardControl)
        {
        }

        // Update the camera's position and orientation.
        protected override void TurnUp()
        {
            CameraPhi -= CameraDPhi;
            if (CameraPhi < CameraPhiMin) CameraPhi = CameraPhiMin;
        }
        protected override void TurnDown()
        {
            CameraPhi += CameraDPhi;
            if (CameraPhi > CameraPhiMax) CameraPhi = CameraPhiMax;
        }

        // Use the current values of CameraX, CameraY, CameraZ,
        // CameraTheta, and CameraPhi to position the camera.
        protected override void PositionCamera()
        {
            TheCamera.Position = CameraPosition;

            Vector3D v = AngleToVector(CameraTheta, 1);
            double y = Math.Cos(CameraPhi);
            TheCamera.LookDirection = new Vector3D(v.X, y, v.Z);
            TheCamera.UpDirection = new Vector3D(0, 1, 0);
        }
    }
}
