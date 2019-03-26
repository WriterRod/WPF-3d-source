using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Automobile
{
    public class AutomobileCameraController
    {
        // The camera.
        public PerspectiveCamera TheCamera = null;

        // The controls that provide events.
        private UIElement KeyboardControl = null;

        // Adjustment values.
        public double CameraDR = 0.1;
        public double CameraDTheta = Math.PI / 30;

        // The current position and orientation.
        public Point3D CameraPosition { get; set; } = new Point3D(4, 0.5, 5);
        public double CameraTheta = Math.PI * 1.3;

        // Constructor.
        public AutomobileCameraController(PerspectiveCamera camera, Viewport3D viewport,
            UIElement keyboardControl)
        {
            TheCamera = camera;
            viewport.Camera = TheCamera;

            KeyboardControl = keyboardControl;
            KeyboardControl.PreviewKeyDown += KeyboardControl_KeyDown;

            PositionCamera();
        }

        // Update the camera's position.
        protected void MoveForward()
        {
            CameraPosition += AngleToVector(CameraTheta, CameraDTheta);
        }
        protected void MoveBackward()
        {
            CameraPosition += AngleToVector(CameraTheta, -CameraDTheta);
        }

        protected virtual void TurnUp()
        {
            MoveForward();
        }
        protected virtual void TurnDown()
        {
            MoveBackward();
        }

        protected void TurnRight()
        {
            CameraTheta += CameraDTheta;
        }
        protected void TurnLeft()
        {
            CameraTheta -= CameraDTheta;
        }

        protected void MoveLeft()
        {
            Vector3D v = AngleToVector(CameraTheta, CameraDR);
            CameraPosition += new Vector3D(v.Z, 0, -v.X);
        }
        protected void MoveRight()
        {
            Vector3D v = AngleToVector(CameraTheta, CameraDR);
            CameraPosition += new Vector3D(-v.Z, 0, +v.X);
        }

        protected virtual void DoShift()
        {
        }
        protected virtual void DoSpace()
        {
        }

        #region Camera Control

        // Adjust the camera's position.
        private void KeyboardControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.A:
                    TurnLeft();
                    break;
                case Key.Right:
                case Key.D:
                    TurnRight();
                    break;
                case Key.W:
                    MoveForward();
                    break;
                case Key.Up:
                    TurnUp();
                    break;
                case Key.S:
                    MoveBackward();
                    break;
                case Key.Down:
                    TurnDown();
                    break;
                case Key.Q:
                    MoveLeft();
                    break;
                case Key.E:
                    MoveRight();
                    break;
                case Key.LeftShift:
                case Key.RightShift:
                    DoShift();
                    break;
                case Key.Space:
                    DoSpace();
                    break;
            }

            // Update the camera's position.
            PositionCamera();
        }

        // Convert the angle into a vector.
        protected Vector3D AngleToVector(double angle, double length)
        {
            return new Vector3D(
                length * Math.Cos(angle), 0, length * Math.Sin(angle));
        }

        // Use the current values of CameraR, CameraTheta,
        // and CameraPhi to position the camera.
        protected virtual void PositionCamera()
        {
            TheCamera.Position = CameraPosition;
            TheCamera.LookDirection = AngleToVector(CameraTheta, 1);
            TheCamera.UpDirection = new Vector3D(0, 1, 0);
        }

        #endregion Camera Control
    }
}
