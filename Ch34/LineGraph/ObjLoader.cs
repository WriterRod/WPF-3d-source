using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;

namespace LineGraph
{
    // Load an obj 3D graphics file.
    public class ObjLoader
    {
        // Results.
        public Dictionary<string, ObjMaterial> MtlMaterials =
            new Dictionary<string, ObjMaterial>();
        public List<string> MaterialNames = new List<string>();
        public List<string> MeshNames = new List<string>();
        public List<MeshGeometry3D> Meshes = new List<MeshGeometry3D>();

        // Hold information for an object model.
        private class ObjModel
        {
            public string Name = "";
            public string MaterialName = "";
            public List<ObjFace> Faces = new List<ObjFace>();
            public List<Point3D> Vertices = new List<Point3D>();
            public List<Vector3D> Normals = new List<Vector3D>();
            public List<Point> TextureCoordinates = new List<Point>();
        }

        // Hold information on a face.
        private class ObjFace
        {
            public List<int> Vertices = new List<int>();
            public List<int> Normals = new List<int>();
            public List<int> TextureCoords = new List<int>();
            public int SmoothingGroup = 0;

            // Initialize a face from a line of data.
            public ObjFace(List<Point3D> AllVertices, List<Vector3D> AllNormals,
                List<Point> AllTextureCoordinates, string line)
            {
                // Break into fields.
                char[] separators = { ' ' };
                string[] fields = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                // Process the fields.
                for (int i = 1; i < fields.Length; i++)
                {
                    // Split into sub-fields.
                    string[] subfields = fields[i].Split('/');

                    // Save the vertex ID.
                    int index = int.Parse(subfields[0]);
                    if (index < 0) index = AllVertices.Count + index + 1;
                    Vertices.Add(index);

                    if (subfields.Length > 1)
                    {
                        // Save the vertex texture coordinates.
                        if (subfields[1].Length > 0)
                        {
                            index = int.Parse(subfields[1]);
                            if (index < 0) index = AllTextureCoordinates.Count + index + 1;
                            TextureCoords.Add(index);
                        }

                        // Save the vertex normal.
                        if (subfields.Length > 2)
                        {
                            if (subfields[2].Length > 0)
                            {
                                index = int.Parse(subfields[2]);
                                if (index < 0) index = AllNormals.Count + index + 1;
                                Normals.Add(index);
                            }
                        }
                    }
                }
            }
        }

        // Raw data.
        private List<Point3D> AllVertices = new List<Point3D>();
        public List<Point> AllTextureCoordinates = new List<Point>();
        public List<Vector3D> AllNormals = new List<Vector3D>();

        // Materials.
        private ObjMaterial NewMaterial = null;
        private List<ObjMaterial> AllMaterials = new List<ObjMaterial>();

        // Object models.
        private ObjModel NewObjectModel = null;
        private List<ObjModel> AllObjectModels = new List<ObjModel>();

        // Load the obj file.
        public ObjLoader(string dir, string objfile,
            bool invertTextures = false, bool zIsUp = false)
        {
            // Read the obj file.
            ReadObjFile(dir, objfile);

            // Process the models.
            ProcessModels(invertTextures, zIsUp);
        }

        // Read the mtl file.
        private void ReadMtlFile(string dir, string mtlfile)
        {
            char[] whitespace = { ' ', '\t' };
            List<string> unknown = new List<string>();
            using (StreamReader file = new System.IO.StreamReader(dir + mtlfile))
            {
                while (!file.EndOfStream)
                {
                    // Read the next line.
                    string line = CleanLine(file.ReadLine());
                    if (line.Length == 0) continue;

                    string[] fields = line.Split(whitespace, StringSplitOptions.RemoveEmptyEntries);
                    switch (fields[0])
                    {
                        case "newmtl":  // Material.
                            NewMaterial = new ObjMaterial();
                            AllMaterials.Add(NewMaterial);
                            if (fields.Length > 1) NewMaterial.Name = fields[1];
                            break;

                        case "Ns":      // Specular power.
                            NewMaterial.Ns = double.Parse(fields[1]);
                            break;

                        case "Tr":      // Transparency.
                            NewMaterial.Alpha = 1 - double.Parse(fields[1]);
                            break;

                        case "d":       // Dissolve.
                            NewMaterial.Alpha = double.Parse(fields[1]);
                            break;

                        case "Kd":      // Diffuse color.
                            double kdx = double.Parse(fields[1]);
                            double kdy = double.Parse(fields[2]);
                            double kdz = double.Parse(fields[3]);
                            NewMaterial.Kd = new Point3D(kdx, kdy, kdz);
                            break;

                        case "Ks":      // Specular color.
                            double ksx = double.Parse(fields[1]);
                            double ksy = double.Parse(fields[2]);
                            double ksz = double.Parse(fields[3]);
                            NewMaterial.Ks = new Point3D(ksx, ksy, ksz);
                            break;

                        case "Ka":          // Ambient color.
                            double kax = double.Parse(fields[1]);
                            double kay = double.Parse(fields[2]);
                            double kaz = double.Parse(fields[3]);
                            NewMaterial.Ka = new Point3D(kax, kay, kaz);
                            break;

                        case "map_Kd":      // texture file.
                            NewMaterial.Filename = dir + line.Substring(6).Trim();
                            break;

                        default:
                            if (!unknown.Contains(fields[0]))
                                unknown.Add(fields[0]);
                            break;
                    }
                }
            }

            // List unknown tokens.
            if (unknown.Count > 0)
            {
                Console.WriteLine("Unknown mtl tokens:");
                foreach (string token in unknown)
                    Console.WriteLine("    " + token);
            }
        }

        // Read the obj file.
        private void ReadObjFile(string dir, string objfile)
        {
            char[] whitespace = { ' ', '\t' };
            List<string> unknown = new List<string>();

            // The current smoothing group.
            int smoothingGroup = 0;

            // The current material name.
            string curMatName = "";

            // Read the file.
            using (StreamReader file = new System.IO.StreamReader(dir + objfile))
            {
                while (!file.EndOfStream)
                {
                    // Read the next line.
                    string line = CleanLine(file.ReadLine());
                    if (line.Length == 0) continue;

                    string[] fields = line.Split(whitespace, StringSplitOptions.RemoveEmptyEntries);
                    switch (fields[0])
                    {
                        case "o":       // Object model.
                        case "g":       // Group.
                            // Only create a new one if the current one isn't empty.
                            if ((NewObjectModel == null) || (NewObjectModel.Faces.Count > 0))
                            {
                                NewObjectModel = new ObjModel();
                                AllObjectModels.Add(NewObjectModel);
                            }
                            NewObjectModel.MaterialName = curMatName;
                            if (fields.Length > 1) NewObjectModel.Name = fields[1];
                            smoothingGroup = 0;
                            break;

                        case "v":       // Vertex.
                            double x = double.Parse(fields[1]);
                            double y = double.Parse(fields[2]);
                            double z = double.Parse(fields[3]);
                            AllVertices.Add(new Point3D(x, y, z));
                            break;

                        case "vt":      // Texture coordinates.
                            double u = double.Parse(fields[1]);
                            double v = double.Parse(fields[2]);
                            AllTextureCoordinates.Add(new Point(u, v));
                            break;

                        case "vn":      // Normal.
                            double nx = double.Parse(fields[1]);
                            double ny = double.Parse(fields[2]);
                            double nz = double.Parse(fields[3]);
                            AllNormals.Add(new Vector3D(nx, ny, nz));
                            break;

                        case "f":       // Face.
                            ObjFace face = new ObjFace(
                                AllVertices, AllNormals, AllTextureCoordinates, line);
                            face.SmoothingGroup = smoothingGroup;
                            NewObjectModel.Faces.Add(face);
                            NewObjectModel.MaterialName = curMatName;
                            break;

                        case "s":       // Smoothing.
                            if (fields.Length == 0) smoothingGroup = 0;
                            else if (fields[1] == "off") smoothingGroup = 0;
                            else if (fields[1] == "on") smoothingGroup = 1;
                            else if (!int.TryParse(fields[1], out smoothingGroup))
                                smoothingGroup = 0;
                            break;

                        case "usemtl":       // Material.
                            if (fields.Length > 1) curMatName = fields[1];
                            break;

                        case "mtllib":      // Material file.
                            string mtlfile = line.Substring(6).Trim();
                            ReadMtlFile(dir, mtlfile);
                            break;

                        default:
                            if (!unknown.Contains(fields[0]))
                                unknown.Add(fields[0]);
                            break;
                    }
                }
            }

            // If the final model is empty, remove it.
            if (AllObjectModels[AllObjectModels.Count - 1].Faces.Count == 0)
                AllObjectModels.RemoveAt(AllObjectModels.Count - 1);

            // List unknown tokens.
            if (unknown.Count > 0)
            {
                Console.WriteLine("Unknown obj tokens:");
                foreach (string token in unknown)
                    Console.WriteLine("    " + token);
            }
        }

        // Process the models.
        private void ProcessModels(bool invertTextures, bool zIsUp)
        {
            // Make the dictionary of materials.
            foreach (ObjMaterial material in AllMaterials)
            {
                // Make the material's MaterialGroup.
                material.MatGroup = new MaterialGroup();

                // Transparency. (Not used.)
                byte alpha = (byte)(material.Alpha * 255);

                // Diffuse.
                byte diffR = (byte)(material.Kd.X * 255);
                byte diffG = (byte)(material.Kd.Y * 255);
                byte diffB = (byte)(material.Kd.Z * 255);
                Color diffColor = Color.FromArgb(255, diffR, diffG, diffB);
                SolidColorBrush diffBrush = new SolidColorBrush(diffColor);
                DiffuseMaterial diffMat = new DiffuseMaterial(diffBrush);
                material.MatGroup.Children.Add(diffMat);

                // If it has a file, use it.
                if (material.Filename != null)
                {
                    // Use the file.
                    string filename = material.Filename;
                    ImageBrush imgBrush = new ImageBrush();
                    imgBrush.ViewportUnits = BrushMappingMode.Absolute;
                    imgBrush.TileMode = TileMode.Tile;

                    // Invert the texture if necessary.
                    if (invertTextures)
                    {
                        TransformGroup trans = new TransformGroup();
                        trans.Children.Add(new ScaleTransform(1, -1));
                        trans.Children.Add(new TranslateTransform(0, 1));
                        imgBrush.Transform = trans;
                    }

                    imgBrush.ImageSource = new BitmapImage(new Uri(filename, UriKind.Relative));
                    DiffuseMaterial imgMat = new DiffuseMaterial(imgBrush);
                    material.MatGroup.Children.Add(imgMat);
                }

                // Specular.
                byte specR = (byte)(material.Ks.X * 255);
                byte specG = (byte)(material.Ks.Y * 255);
                byte specB = (byte)(material.Ks.Z * 255);
                Color specColor = Color.FromArgb(255, specR, specG, specB);
                SolidColorBrush specBrush = new SolidColorBrush(specColor);
                SpecularMaterial specMat = new SpecularMaterial(specBrush, material.Ns);
                material.MatGroup.Children.Add(specMat);

                // We ignore Ka and Tr.

                // Add it to the materials dictionary.
                MtlMaterials.Add(material.Name, material);
            }

            // Convert the object models into meshes.
            foreach (ObjModel model in AllObjectModels)
            {
                // Make the mesh.
                MeshGeometry3D mesh = new MeshGeometry3D();
                Meshes.Add(mesh);
                MeshNames.Add(model.Name);
                MaterialNames.Add(model.MaterialName);

                // Make a new list of smoothing groups.
                Dictionary<int, Dictionary<Point3D, int>> smoothingGroups =
                    new Dictionary<int, Dictionary<Point3D, int>>();

                // Entry 0 is null (no smoothing).
                smoothingGroups.Add(0, null);

                // Make the faces.
                foreach (ObjFace face in model.Faces)
                {
                    // Make the face's vertices.
                    int numPoints = face.Vertices.Count;
                    Point3D[] points = new Point3D[numPoints];
                    for (int i = 0; i < numPoints; i++)
                        points[i] = AllVertices[face.Vertices[i] - 1];

                    // Get texture coordinates if present.
                    Point[] textureCoords = null;
                    if (face.TextureCoords.Count > 0)
                    {
                        textureCoords = new Point[numPoints];
                        for (int i = 0; i < numPoints; i++)
                            textureCoords[i] = AllTextureCoordinates[face.TextureCoords[i] - 1];
                    }

                    // Get normals if present.
                    Vector3D[] normals = null;
                    if (face.Normals.Count > 0)
                    {
                        normals = new Vector3D[numPoints];
                        for (int i = 0; i < numPoints; i++)
                            normals[i] = AllNormals[face.Normals[i] - 1];
                    }

                    // Get the point dictionary for this smoothing group.
                    // Add new groups if needed.
                    if (!smoothingGroups.ContainsKey(face.SmoothingGroup))
                    {
                        smoothingGroups.Add(face.SmoothingGroup,
                            new Dictionary<Point3D, int>());
                    }
                    Dictionary<Point3D, int> pointDict = smoothingGroups[face.SmoothingGroup];

                    // Make the polygon.
                    mesh.AddPolygon(pointDict: pointDict,
                        textureCoords: textureCoords, normals: normals,
                        points: points);
                }

                // If Z is up, rotate the model.
                if (zIsUp)
                {
                    mesh.ApplyTransformation(D3.Rotate(D3.XVector(), D3.Origin, -90));
                }
            }
        }

        // Fit the meshes to the indicated box.
        public void FitToBox(Rect3D box)
        {
            // Get the combined bounds of all meshes.
            Rect3D bounds = Meshes[0].Bounds;
            for (int i = 1; i < Meshes.Count; i++) bounds.Union(Meshes[i].Bounds);

            // Get the transformation.
            Transform3DGroup group = MeshExtensions.GetBoxMapping(bounds, box);

            // Apply the transformation to all meshes.
            foreach (MeshGeometry3D mesh in Meshes)
                mesh.ApplyTransformation(group);
        }

        // Remove comments and trim the line.
        private string CleanLine(string line)
        {
            if (line.Contains("#")) line = line.Substring(0, line.IndexOf("#"));
            return line.Trim();
        }
    }
}
