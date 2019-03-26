using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace DodecahedronWireframe
{
    public class SphericalCameraController
    {
        // The camera.
        public PerspectiveCamera TheCamera = null;

        // The controls that provide events.
        private UIElement KeyboardControl = null;
        private UIElement WheelControl = null;
        private UIElement MouseControl = null;

        // Adjustment values.
        public double CameraDR = 0.1;
        public double CameraDTheta = Math.PI / 30;
        public double CameraDPhi = Math.PI / 15;

        // The current position.
        private double CameraR = 8.0;
        private double CameraTheta = Math.PI / 3.0;
        private double CameraPhi = Math.PI / 3.0;

        // Get or set the spherical coordinates.
        // The point's coordinates are (r, theta, phi).
        public Point3D SphericalCoordinates
        {
            get
            {
                return new Point3D(CameraR, CameraTheta, CameraPhi);
            }
            set
            {
                CameraR = value.X;
                CameraTheta = value.Y;
                CameraPhi = value.Z;
            }
        }

        // Get or set the Cartesian coordinates.
        public Point3D CartesianCoordinates
        {
            get
            {
                double x, y, z;
                SphericalToCartesian(CameraR, CameraTheta, CameraPhi, out x, out y, out z);
                return new Point3D(x, y, z);
            }
            set
            {
                double r, theta, phi;
                CartesianToSpherical(value.X, value.Y, value.Z, out r, out theta, out phi);
                CameraR = r;
                CameraTheta = theta;
                CameraPhi = phi;
            }
        }

        // Constructor.
        public SphericalCameraController(PerspectiveCamera camera, Viewport3D viewport,
            UIElement keyboardControl, UIElement wheelControl, UIElement mouseControl)
        {
            TheCamera = camera;
            viewport.Camera = TheCamera;

            KeyboardControl = keyboardControl;
            KeyboardControl.PreviewKeyDown += KeyboardControl_KeyDown;

            WheelControl = wheelControl;
            WheelControl.PreviewMouseWheel += WheelControl_PreviewMouseWheel;

            MouseControl = mouseControl;
            MouseControl.MouseDown += MouseControl_MouseDown;

            PositionCamera();
        }

        // Update the camera's position.
        public void IncreaseR(double amount)
        {
            CameraR += amount;
            if (CameraR < CameraDR) CameraR = CameraDR;
        }
        public void IncreaseR()
        {
            IncreaseR(CameraDR);
        }
        public void DecreaseR(double amount)
        {
            IncreaseR(-amount);
        }
        public void DecreaseR()
        {
            IncreaseR(-CameraDR);
        }

        public void IncreaseTheta(double amount)
        {
            CameraTheta += amount;
        }
        public void IncreaseTheta()
        {
            IncreaseTheta(CameraDTheta);
        }
        public void DecreaseTheta(double amount)
        {
            IncreaseTheta(-amount);
        }
        public void DecreaseTheta()
        {
            IncreaseTheta(-CameraDTheta);
        }

        public void IncreasePhi(double amount)
        {
            CameraPhi += amount;
        }
        public void IncreasePhi()
        {
            IncreasePhi(CameraDPhi);
        }
        public void DecreasePhi(double amount)
        {
            IncreasePhi(-amount);
        }
        public void DecreasePhi()
        {
            IncreasePhi(-CameraDPhi);
        }

        #region Camera Control

        // Adjust the camera's position.
        private void KeyboardControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    IncreasePhi();
                    break;
                case Key.Down:
                    DecreasePhi();
                    break;
                case Key.Left:
                    IncreaseTheta();
                    break;
                case Key.Right:
                    DecreaseTheta();
                    break;
                case Key.Add:
                case Key.OemPlus:
                    IncreaseR();
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    DecreaseR();
                    break;
            }

            // Update the camera's position.
            PositionCamera();
        }

        // Zoom in or out.
        private void WheelControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            DecreaseR(Math.Sign(e.Delta) * CameraDR);
            PositionCamera();
        }

        // Use the mouse to change the camera's position.
        private Point LastPoint;
        private void MouseControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseControl.CaptureMouse();
            MouseControl.MouseMove += new MouseEventHandler(MouseControl_MouseMove);
            MouseControl.MouseUp += new MouseButtonEventHandler(MouseControl_MouseUp);
            LastPoint = e.GetPosition(MouseControl);
        }

        private void MouseControl_MouseMove(object sender, MouseEventArgs e)
        {
            const double xscale = 0.1;
            const double yscale = 0.1;

            Point newPoint = e.GetPosition(MouseControl);
            double dx = newPoint.X - LastPoint.X;
            double dy = newPoint.Y - LastPoint.Y;

            CameraTheta -= dx * CameraDTheta * xscale;
            CameraPhi -= dy * CameraDPhi * yscale;

            LastPoint = newPoint;
            PositionCamera();
        }

        private void MouseControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseControl.ReleaseMouseCapture();
            MouseControl.MouseMove -= new MouseEventHandler(MouseControl_MouseMove);
            MouseControl.MouseUp -= new MouseButtonEventHandler(MouseControl_MouseUp);
        }

        // Use the current values of CameraR, CameraTheta,
        // and CameraPhi to position the camera.
        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            double x, y, z;
            SphericalToCartesian(CameraR, CameraTheta, CameraPhi,
                out x, out y, out z);
            TheCamera.Position = new Point3D(x, y, z);

            // Look toward the origin.
            TheCamera.LookDirection = new Vector3D(-x, -y, -z);
TheCamera.FarPlaneDistance = 1000000;//@
TheCamera.NearPlaneDistance = 0.0001;//@

            // Set the Up direction.
            TheCamera.UpDirection = new Vector3D(0, 1, 0);
        }

        // Convert from Cartesian to spherical coordinates.
        private void CartesianToSpherical(double x, double y, double z,
            out double r, out double theta, out double phi)
        {
            r = Math.Sqrt(x * x + y * y + z * z);
            double h = Math.Sqrt(x * x + z * z);
            theta = Math.Atan2(x, z);
            phi = Math.Atan2(h, y);
        }

        // Convert from spherical to Cartesian coordinates.
        private void SphericalToCartesian(double r, double theta, double phi,
            out double x, out double y, out double z)
        {
            y = r * Math.Cos(phi);
            double h = r * Math.Sin(phi);
            x = h * Math.Sin(theta);
            z = h * Math.Cos(theta);
        }

        #endregion Camera Control
    }
}
