using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Windows.Media.Media3D;

namespace Robot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // The main model group.
        private Model3DGroup MainGroup = null;

        // The robot's Model3DGroups.
        private Model3DGroup RobotGroup, HeadGroup, NeckGroup, ShoulderGroup,
            LuArmGroup, RuArmGroup, LlArmGroup, RlArmGroup, BackGroup,
            LuLegGroup, RuLegGroup, LlLegGroup, RlLegGroup;

        // The camera.
        private PerspectiveCamera TheCamera = null;

        // The camera controller.
        private SphericalCameraController CameraController = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            MainGroup = new Model3DGroup();
            visual3d.Content = MainGroup;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(MainGroup);
            DefineModel();
        }

        // Define the camera.
        private void DefineCamera(Viewport3D viewport)
        {
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            CameraController = new SphericalCameraController
                (TheCamera, viewport, this, mainGrid, mainGrid);

            // Move back a bit from the origin.
            Point3D coords = CameraController.SphericalCoordinates;
            coords.X = 20;
            CameraController.SphericalCoordinates = coords;
        }

        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            Color darker = Color.FromArgb(255, 96, 96, 96);
            Color dark = Color.FromArgb(255, 128, 128, 128);

            group.Children.Add(new AmbientLight(darker));

            group.Children.Add(new DirectionalLight(dark, new Vector3D(0, -1, 0)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(1, -3, -2)));
            group.Children.Add(new DirectionalLight(dark, new Vector3D(-1, 3, 2)));
        }

        // Define the model.
        private void DefineModel()
        {
            // Axes.
            //MainGroup.Children.Add(MeshExtensions.XAxisModel(5, 0.1));
            //MainGroup.Children.Add(MeshExtensions.YAxisModel(12, 0.1));
            //MainGroup.Children.Add(MeshExtensions.ZAxisModel(5, 0.1));
            //MainGroup.Children.Add(MeshExtensions.OriginModel(0.12));

            // Make the ground.
            const double groundY = -5;
            MakeGround(groundY);

            // This group represents the whole robot.
            RobotGroup = new Model3DGroup();
            MainGroup.Children.Add(RobotGroup);

            // Various robot dimensions.
            const double headR = 1.5;           // Head radius.
            const double neckLen = headR;       // Neck length.
            const double backLen = 3 * headR;   // Back length.
            const double shouW = 3 * headR;     // Shoulder width.
            const double uaLen = 2 * headR;     // Upper arm length.
            const double laLen = 2 * headR;     // Lower arm length
            const double hipsW = 2 * headR;     // Hip width.
            const double ulLen = 2 * headR;     // Upper leg length.
            const double llLen = 2 * headR;     // Lower leg length.
            const double boneR = 0.3;           // Bone radius.
            const double jointR = 0.4;          // Joint radius.
            const double height = 2 * headR + neckLen + backLen + ulLen + llLen;
            const double headY = height - headR;    // Distance from center of head to ground.
            Brush boneBrush = Brushes.PowderBlue;

            // This group represents the whole robot.
            RobotGroup = new Model3DGroup();
            MainGroup.Children.Add(RobotGroup);
            RobotGroup.Transform = new TranslateTransform3D(0, headY + groundY, 0);

            // Head.
            // Skull.
            MeshGeometry3D skullMesh = new MeshGeometry3D();
            skullMesh.AddSphere(D3.Origin, headR, 20, 10, true);
            GeometryModel3D skullModel = skullMesh.MakeModel(boneBrush);

            // Nose.
            MeshGeometry3D noseMesh = new MeshGeometry3D();
            Point3D noseCenter = new Point3D(0, 0, headR);
            Point3D[] nosePoints = G3.MakePolygonPoints(10, noseCenter,
                D3.XVector(headR * 0.2), D3.YVector(headR * 0.2));
            Vector3D noseAxis = new Vector3D(0, 0, headR);
            noseMesh.AddConeFrustum(noseCenter, nosePoints, noseAxis, headR * 0.5);
            GeometryModel3D noseModel = noseMesh.MakeModel(Brushes.Orange);

            // Eyes and smile.
            MeshGeometry3D eyeMesh = new MeshGeometry3D();
            Point3D eyeCenter = SphericalToCartesian(headR, -Math.PI * 0.2, Math.PI * 0.4);
            eyeMesh.AddSphere(eyeCenter, headR * 0.2, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, Math.PI * 0.2, Math.PI * 0.4);
            eyeMesh.AddSphere(eyeCenter, headR * 0.2, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, Math.PI * 0, Math.PI * 0.7);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, Math.PI * 0.1, Math.PI * 0.67);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, -Math.PI * 0.1, Math.PI * 0.67);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, Math.PI * 0.15, Math.PI * 0.6);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            eyeCenter = SphericalToCartesian(headR, -Math.PI * 0.15, Math.PI * 0.6);
            eyeMesh.AddSphere(eyeCenter, headR * 0.1, 10, 5, false);
            GeometryModel3D eyeModel = eyeMesh.MakeModel(Brushes.Black);

            // Hat.
            MeshGeometry3D hatMesh = new MeshGeometry3D();
            Point3D hatCenter = new Point3D(0, headR * 0.75, 0);
            hatMesh.AddSphere(hatCenter, headR * 0.75, 20, 10, true);
            const double hatR = headR * 1.2;
            Point3D[] hatPgon = G3.MakePolygonPoints(20, hatCenter,
                D3.XVector(hatR), D3.ZVector(hatR));
            hatMesh.AddCylinder(hatPgon, D3.YVector(-0.2), true);

            GeometryModel3D hatModel = hatMesh.MakeModel(Brushes.SaddleBrown);

            // Head groups.
            HeadGroup = JoinBones(RobotGroup, null);
            HeadGroup.Children.Add(skullModel);
            HeadGroup.Children.Add(noseModel);
            HeadGroup.Children.Add(eyeModel);
            HeadGroup.Children.Add(hatModel);

            // Neck.
            MeshGeometry3D neckMesh = new MeshGeometry3D();
            Point3D[] neckPgon = G3.MakePolygonPoints(10, D3.Origin,
                D3.XVector(boneR), D3.ZVector(boneR));
            neckMesh.AddCylinder(neckPgon, D3.YVector(-neckLen), true);
            GeometryModel3D neckModel = neckMesh.MakeModel(boneBrush);

            NeckGroup = JoinBones(HeadGroup, new TranslateTransform3D(0, -headR, 0));
            NeckGroup.Children.Add(neckModel);

            // Shoulders.
            MeshGeometry3D shoulderMesh = new MeshGeometry3D();
            Point3D[] shouldersPgon = G3.MakePolygonPoints(10,
                new Point3D(-shouW / 2, 0, 0), D3.ZVector(boneR), D3.YVector(-boneR));
            shoulderMesh.AddCylinder(shouldersPgon, D3.XVector(shouW), true);
            GeometryModel3D shoulderModel = shoulderMesh.MakeModel(boneBrush);

            ShoulderGroup = JoinBones(NeckGroup, new TranslateTransform3D(0, -neckLen, 0));
            ShoulderGroup.Children.Add(shoulderModel);

            // Left upper arm.
            MeshGeometry3D luArmMesh = new MeshGeometry3D();
            luArmMesh.AddCylinder(neckPgon, D3.YVector(-uaLen), true);
            luArmMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D luArmModel = luArmMesh.MakeModel(boneBrush);

            LuArmGroup = JoinBones(ShoulderGroup, new TranslateTransform3D(shouW / 2, 0, 0));
            LuArmGroup.Children.Add(luArmModel);

            // Right upper arm.
            MeshGeometry3D ruArmMesh = new MeshGeometry3D();
            ruArmMesh.AddCylinder(neckPgon, D3.YVector(-uaLen), true);
            ruArmMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D ruArmModel = ruArmMesh.MakeModel(boneBrush);

            RuArmGroup = JoinBones(ShoulderGroup, new TranslateTransform3D(-shouW / 2, 0, 0));
            RuArmGroup.Children.Add(ruArmModel);

            // Left lower arm.
            MeshGeometry3D llArmMesh = new MeshGeometry3D();
            llArmMesh.AddCylinder(neckPgon, D3.YVector(-laLen), true);
            llArmMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D llArmModel = llArmMesh.MakeModel(boneBrush);

            LlArmGroup = JoinBones(LuArmGroup, new TranslateTransform3D(0, -uaLen, 0));
            LlArmGroup.Children.Add(llArmModel);

            // Right lower arm.
            MeshGeometry3D rlArmMesh = new MeshGeometry3D();
            rlArmMesh.AddCylinder(neckPgon, D3.YVector(-laLen), true);
            rlArmMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D rlArmModel = rlArmMesh.MakeModel(boneBrush);

            RlArmGroup = JoinBones(RuArmGroup, new TranslateTransform3D(0, -uaLen, 0));
            RlArmGroup.Children.Add(rlArmModel);

            // Back and hips.
            MeshGeometry3D backMesh = new MeshGeometry3D();
            backMesh.AddCylinder(neckPgon, D3.YVector(-backLen), true);
            GeometryModel3D backModel = backMesh.MakeModel(boneBrush);

            MeshGeometry3D hipsMesh = new MeshGeometry3D();
            Point3D[] hipsPgon = G3.MakePolygonPoints(10,
                new Point3D(-hipsW / 2, -backLen, 0), D3.ZVector(boneR), D3.YVector(-boneR));
            hipsMesh.AddCylinder(hipsPgon, D3.XVector(hipsW), true);
            GeometryModel3D hipsModel = hipsMesh.MakeModel(boneBrush);

            BackGroup = JoinBones(NeckGroup, new TranslateTransform3D(0, -neckLen, 0));
            BackGroup.Children.Add(backModel);
            BackGroup.Children.Add(hipsModel);

            // Left upper leg.
            MeshGeometry3D luLegMesh = new MeshGeometry3D();
            luLegMesh.AddCylinder(neckPgon, D3.YVector(-ulLen), true);
            luLegMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D luLegModel = luLegMesh.MakeModel(boneBrush);

            LuLegGroup = JoinBones(BackGroup, new TranslateTransform3D(-hipsW / 2, -backLen, 0));
            LuLegGroup.Children.Add(luLegModel);

            // Right upper leg.
            MeshGeometry3D ruLegMesh = new MeshGeometry3D();
            ruLegMesh.AddCylinder(neckPgon, D3.YVector(-ulLen), true);
            ruLegMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D ruLegModel = ruLegMesh.MakeModel(boneBrush);

            RuLegGroup = JoinBones(BackGroup, new TranslateTransform3D(hipsW / 2, -backLen, 0));
            RuLegGroup.Children.Add(ruLegModel);

            // Left lower leg.
            MeshGeometry3D llLegMesh = new MeshGeometry3D();
            llLegMesh.AddCylinder(neckPgon, D3.YVector(-llLen), true);
            llLegMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D llLegModel = llLegMesh.MakeModel(boneBrush);

            LlLegGroup = JoinBones(LuLegGroup, new TranslateTransform3D(0, -ulLen, 0));
            LlLegGroup.Children.Add(llLegModel);

            // Right lower leg.
            MeshGeometry3D rlLegMesh = new MeshGeometry3D();
            rlLegMesh.AddCylinder(neckPgon, D3.YVector(-llLen), true);
            rlLegMesh.AddSphere(D3.Origin, jointR, 10, 5, true);
            GeometryModel3D rlLegModel = rlLegMesh.MakeModel(boneBrush);

            RlLegGroup = JoinBones(RuLegGroup, new TranslateTransform3D(0, -ulLen, 0));
            RlLegGroup.Children.Add(rlLegModel);
        }

        // Join two bones together.
        private Model3DGroup JoinBones(Model3DGroup parentGroup, Transform3D offset)
        {
            Model3DGroup offsetGroup = new Model3DGroup();
            offsetGroup.Transform = offset;
            parentGroup.Children.Add(offsetGroup);

            Model3DGroup result = new Model3DGroup();
            offsetGroup.Children.Add(result);
            return result;
        }

        // Make the ground mesh.
        private void MakeGround(double groundY)
        {
            MeshGeometry3D groundMesh = new MeshGeometry3D();
            const double dx = 15;
            const double dy = 1;
            const double dz = dx;
            Point3D corner = new Point3D(-dx / 2, groundY - dy, -dz / 2);
            groundMesh.AddBoxWrapped(corner, D3.XVector(dx), D3.YVector(dy), D3.ZVector(dz));

            Point[] topCoords =
            {
                new Point(0.1, 0.1),
                new Point(0.1, 0.9),
                new Point(0.9, 0.9),
                new Point(0.9, 0.1),
            };
            Point[] frontCoords =
            {
                new Point(0.0, 0.1),
                new Point(0.0, 0.9),
                new Point(0.1, 0.9),
                new Point(0.1, 0.1),
            };
            Point[] leftCoords =
            {
                new Point(0.9, 0.0),
                new Point(0.1, 0.0),
                new Point(0.1, 0.1),
                new Point(0.9, 0.1),
            };
            Point[] rightCoords =
            {
                new Point(0.1, 1.0),
                new Point(0.9, 1.0),
                new Point(0.9, 0.9),
                new Point(0.1, 0.9),
            };
            Point[] backCoords =
            {
                new Point(1.0, 0.9),
                new Point(1.0, 0.1),
                new Point(0.9, 0.1),
                new Point(0.9, 0.9),
            };
            Point[] bottomCoords =
            {
                new Point(0.9, 0.1),
                new Point(0.9, 0.9),
                new Point(0.1, 0.9),
                new Point(0.1, 0.1),
            };
            groundMesh.AddBox(corner, D3.XVector(dx), D3.YVector(dy), D3.ZVector(dz),
                frontCoords, leftCoords, rightCoords, backCoords, topCoords, bottomCoords);
            MainGroup.Children.Add(groundMesh.MakeModel("rock.jpg"));
        }

        private void neckSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            NeckGroup.Transform = D3.Rotate(D3.YVector(), D3.Origin, neckSlider.Value);
        }

        private void leftShoulderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LuArmGroup.Transform = D3.Rotate(-D3.XVector(), D3.Origin, leftShoulderSlider.Value);
        }

        private void rightShoulderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RuArmGroup.Transform = D3.Rotate(-D3.XVector(), D3.Origin, rightShoulderSlider.Value);
        }

        private void leftElbowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LlArmGroup.Transform = D3.Rotate(-D3.XVector(), D3.Origin, leftElbowSlider.Value);
        }

        private void rightElbowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RlArmGroup.Transform = D3.Rotate(-D3.XVector(), D3.Origin, rightElbowSlider.Value);
        }

        private void leftHipSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LuLegGroup.Transform = D3.Rotate(-D3.XVector(), D3.Origin, leftHipSlider.Value);
        }

        private void rightHipSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RuLegGroup.Transform = D3.Rotate(-D3.XVector(), D3.Origin, rightHipSlider.Value);
        }

        private void leftKneeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LlLegGroup.Transform = D3.Rotate(-D3.XVector(), D3.Origin, leftKneeSlider.Value);
        }

        private void rightKneeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RlLegGroup.Transform = D3.Rotate(-D3.XVector(), D3.Origin, rightKneeSlider.Value);
        }

        // Convert from spherical to Cartesian coordinates.
        private Point3D SphericalToCartesian(double r, double theta, double phi)
        {
            double y = r * Math.Cos(phi);
            double h = r * Math.Sin(phi);
            double x = h * Math.Sin(theta);
            double z = h * Math.Cos(theta);
            return new Point3D(x, y, z);
        }
    }
}
