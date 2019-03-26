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

namespace RobotArm
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
        private Model3DGroup RobotGroup, BaseGroup, ShoulderGroup, ElbowGroup,
            WristGroup, Finger1Group, Finger2Group;

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
            //MainGroup.Children.Add(MeshExtensions.XAxisModel(4, 0.1));
            //MainGroup.Children.Add(MeshExtensions.YAxisModel(4, 0.1));
            //MainGroup.Children.Add(MeshExtensions.ZAxisModel(4, 0.1));
            //MainGroup.Children.Add(MeshExtensions.OriginModel(0.12));

            // Make the ground.
            MeshGeometry3D groundMesh = new MeshGeometry3D();
            const double dx = 4;
            const double dy = 1;
            const double dz = dx;
            const double groundY = -2;
            Point3D corner = new Point3D(-dx / 2, groundY - dy, -dz / 2);
            Point[] topCoords =
            {
                new Point(0, 0),
                new Point(0, dz),
                new Point(dx, dz),
                new Point(dx, 0),
            };
            Point[] sideCoords =
            {
                new Point(0, 0),
                new Point(0, dz),
                new Point(dy, dz),
                new Point(dy, 0),
            };
            groundMesh.AddBox(corner, D3.XVector(dx), D3.YVector(dy), D3.ZVector(dz),
                sideCoords, sideCoords, sideCoords, sideCoords, topCoords, topCoords);
            MainGroup.Children.Add(groundMesh.MakeModel("metal.jpg"));

            // This group represents the whole robot.
            RobotGroup = new Model3DGroup();
            MainGroup.Children.Add(RobotGroup);

            // Base.
            const double baseWidth = 0.4;
            const double baseLength = 2.5;
            MeshGeometry3D baseMesh = new MeshGeometry3D();
            baseMesh.AddBox(new Point3D(-baseWidth / 2, 0, -baseWidth / 2),
                D3.XVector(baseWidth), D3.YVector(baseLength), D3.ZVector(baseWidth));
            GeometryModel3D baseModel = baseMesh.MakeModel(Brushes.Pink);

            const int numCyl = 20;
            const double bJointWidth = 0.5;
            const double bJointRadius = 0.5;
            Point3D[] bPgon = G3.MakePolygonPoints(numCyl, new Point3D(0, 0, 0),
                D3.XVector(bJointRadius), D3.ZVector(-bJointRadius));
            baseMesh.AddCylinder(bPgon, D3.YVector(bJointWidth), true);

            BaseGroup = JoinBones(RobotGroup, new TranslateTransform3D(0, groundY, 0));
            BaseGroup.Children.Add(baseModel);

            // Shoulder.
            const double shoulderWidth = 0.4;
            const double shoulderLength = 2;
            MeshGeometry3D shoulderMesh = new MeshGeometry3D();
            shoulderMesh.AddBox(new Point3D(-shoulderWidth / 2, 0, -shoulderWidth / 2),
                D3.XVector(shoulderWidth), D3.YVector(shoulderLength), D3.ZVector(shoulderWidth));

            const double sJointWidth = 0.5;
            const double sJointRadius = 0.4;
            Point3D[] sPgon = G3.MakePolygonPoints(numCyl, new Point3D(0, 0, -sJointWidth / 2),
                D3.XVector(sJointRadius), D3.YVector(sJointRadius));
            shoulderMesh.AddCylinder(sPgon, D3.ZVector(sJointWidth), true);
            GeometryModel3D shoulderModel = shoulderMesh.MakeModel(Brushes.LightGreen);

            ShoulderGroup = JoinBones(BaseGroup, new TranslateTransform3D(0, baseLength, 0));
            ShoulderGroup.Children.Add(shoulderModel);

            // Elbow.
            const double elbowWidth = 0.4;
            const double elbowLength = 1.5;
            MeshGeometry3D elbowMesh = new MeshGeometry3D();
            elbowMesh.AddBox(new Point3D(-elbowWidth / 2, 0, -elbowWidth / 2),
                D3.XVector(elbowWidth), D3.YVector(elbowLength), D3.ZVector(elbowWidth));

            const double eJointWidth = 0.5;
            const double eJointRadius = 0.4;
            Point3D[] ePgon = G3.MakePolygonPoints(numCyl, new Point3D(0, 0, -eJointWidth / 2),
                D3.XVector(eJointRadius), D3.YVector(eJointRadius));
            elbowMesh.AddCylinder(ePgon, D3.ZVector(eJointWidth), true);
            GeometryModel3D elbowModel = elbowMesh.MakeModel(Brushes.LightBlue);

            ElbowGroup = JoinBones(ShoulderGroup, new TranslateTransform3D(0, shoulderLength, 0));
            ElbowGroup.Children.Add(elbowModel);

            // Wrist.
            const double wDx = 1.5;
            const double wDy = 0.2;
            const double wDz = 0.4;
            MeshGeometry3D wristMesh = new MeshGeometry3D();
            wristMesh.AddBox(new Point3D(-wDx / 2, 0, -wDz / 2),
                D3.XVector(wDx), D3.YVector(wDy), D3.ZVector(wDz));

            const double wJointRadius = 0.3;
            Point3D[] wPgon = G3.MakePolygonPoints(numCyl, new Point3D(0, -wDy / 2, 0),
                D3.XVector(wJointRadius), D3.ZVector(-wJointRadius));
            wristMesh.AddCylinder(wPgon, D3.YVector(wDy), true);
            GeometryModel3D wristModel = wristMesh.MakeModel(Brushes.Red);

            WristGroup = JoinBones(ElbowGroup, new TranslateTransform3D(0, elbowLength, 0));
            WristGroup.Children.Add(wristModel);

            // Finger 1.
            const double fDx = 0.1;
            const double fDy = 0.5;
            const double fDz = 0.2;
            MeshGeometry3D finger1Mesh = new MeshGeometry3D();
            finger1Mesh.AddBox(new Point3D(-fDx / 2, 0, -fDz / 2),
                D3.XVector(fDx), D3.YVector(fDy), D3.ZVector(fDz));
            GeometryModel3D finger1Model = finger1Mesh.MakeModel(Brushes.Green);

            Finger1Group = JoinBones(WristGroup, new TranslateTransform3D(-fDx / 2, wDy, 0));
            Finger1Group.Children.Add(finger1Model);

            // Finger 2.
            MeshGeometry3D finger2Mesh = new MeshGeometry3D();
            finger2Mesh.AddBox(new Point3D(-fDx / 2, 0, -fDz / 2),
                D3.XVector(fDx), D3.YVector(fDy), D3.ZVector(fDz));
            GeometryModel3D finger2Model = finger2Mesh.MakeModel(Brushes.Green);

            Finger2Group = JoinBones(WristGroup, new TranslateTransform3D(fDx / 2, wDy, 0));
            Finger2Group.Children.Add(finger2Model);
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

        private void baseSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BaseGroup.Transform = D3.Rotate(D3.YVector(), D3.Origin, baseSlider.Value);
        }

        private void shoulderSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ShoulderGroup.Transform = D3.Rotate(D3.ZVector(), D3.Origin, shoulderSlider.Value);
        }

        private void elbowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ElbowGroup.Transform = D3.Rotate(D3.ZVector(), D3.Origin, elbowSlider.Value);
        }

        private void wristSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WristGroup.Transform = D3.Rotate(D3.YVector(), D3.Origin, wristSlider.Value);
        }

        private void handSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Finger1Group.Transform = new TranslateTransform3D(-handSlider.Value, 0, 0);
            Finger2Group.Transform = new TranslateTransform3D(handSlider.Value, 0, 0);
        }
    }
}
