/************************************************************************************ 
 * Copyright (c) 2008-2012, Columbia University
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Columbia University nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY COLUMBIA UNIVERSITY ''AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * 
 * ===================================================================================
 * Authors: Ohan Oda (ohan@cs.columbia.edu) 
 * 
 *************************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;

namespace MarkerLayout
{
    public class LayoutManager : IDisposable
    {
        #region Structs

        /// <summary>
        /// A struct that specifies a marker configuration
        /// </summary>
        private struct MarkerConfig
        {
            /// <summary>
            /// The ID of this marker
            /// </summary>
            public int ID;

            /// <summary>
            /// The upper left corner of this marker
            /// </summary>
            public Point Position;

            /// <summary>
            /// The final size (after cropping or resizing) of this marker
            /// </summary>
            public Size Size;

            /// <summary>
            /// The 8-bit grayscaled image data of this marker
            /// </summary>
            public byte[] Image;

            /// <summary>
            /// The actual image width of this marker
            /// </summary>
            public int ImageWidth;

            /// <summary>
            /// The actual image height of this marker
            /// </summary>
            public int ImageHeight;

            /// <summary>
            /// The cropping area to be applied on this marker (the .pgm raw marker files generated
            /// by 'ARTag create marker' program may output additional white boundary and labels around
            /// the actual marker square, so cropping may need to be applied)
            /// </summary>
            public Rectangle CropArea;
        }

        private struct CoordframeConfig
        {
            public String CoodframeName;
            public int MinPoint;
        }

        private struct Background
        {
            public byte[] Image;

            public Point Position;

            public Size Size;

            public int ImageWidth;

            public int ImageHeight;
        }

        #endregion

        #region Member Fields

        private float pixelPerInch;
        private Point configCenter;

        private Bitmap markerImage;
        private Background background;
        private Dictionary<int, MarkerConfig> configs;
        private Dictionary<CoordframeConfig, List<int>> coordframes;
        private List<int> markerIDs;
        private List<String> frameNames;

        private CoordframeConfig coodframeConfig;

        #endregion

        #region Constructors

        /// <summary>
        /// Parses an XML file to create the marker layout.
        /// </summary>
        /// <param name="xmlFile"></param>
        public LayoutManager(String xmlFile)
        {
            ParseXML(xmlFile);
        }

        /// <summary>
        /// Creates a marker layout manager with specified image width and height, and pixels per inch.
        /// </summary>
        /// <param name="width">The width of the entire marker array</param>
        /// <param name="height">The height of the entire marker array</param>
        /// <param name="pixelPerInch">The number of pixels per inch (conversion used from
        /// marker image dimention to the configuration coordinates)</param>
        public LayoutManager(int width, int height, float pixelPerInch)
            : this(width, height, pixelPerInch, Point.Empty)
        { }

        /// <summary>
        /// Creates a marker layout manager with specified image width and height, pixels per inch,
        /// and the center point for the configuration file.
        /// </summary>
        /// <param name="width">The width of the entire marker array</param>
        /// <param name="height">The height of the entire marker array</param>
        /// <param name="pixelPerInch">The number of pixels per inch (conversion used from
        /// marker image dimention to the configuration coordinates)</param>
        /// <param name="configCenter">The center point of the configuration file where (0, 0, 0)
        /// coordinate should be defined at.</param>
        public LayoutManager(int width, int height, float pixelPerInch, Point configCenter)
        {
            this.pixelPerInch = pixelPerInch;
            this.configCenter = configCenter;

            Initialize(width, height);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the compiled bitmap image.
        /// </summary>
        /// <remarks>
        /// You need to call the Compile() function before accessing this property. 
        /// </remarks>
        public Bitmap MarkerImage
        {
            get { return markerImage; }
        }

        /// <summary>
        /// Gets or sets the center ((0,0,0) point) of the configuration file.
        /// </summary>
        public Point ConfigCenter
        {
            get { return configCenter; }
            set { configCenter = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Begins the current coordinate frame configuration with the given name.
        /// </summary>
        /// <param name="coordframeName">The name of the coordframe</param>
        /// <exception cref="ArgumentException">If 'coordframeName' is already used.</exception>
        /// <exception cref="InvalidOperationException">
        /// If called before ending the previous BeginCoordframe(...) call.
        public void BeginCoordframe(String coordframeName)
        {
            BeginCoordframe(coordframeName, 0);
        }

        /// <summary>
        /// Begins the current coordinate frame configuration with the given name and min points. 
        /// </summary>
        /// <param name="coordframeName">The name of the coordframe</param>
        /// <param name="minPoints">The 'min_points' definition</param>
        /// <exception cref="ArgumentException">If 'coordframeName' is already used.</exception>
        /// <exception cref="InvalidOperationException">
        /// If called before ending the previous BeginCoordframe(...) call.
        /// </exception>
        public void BeginCoordframe(String coordframeName, int minPoints)
        {
            if (!coodframeConfig.CoodframeName.Equals(""))
                throw new InvalidOperationException("You should end current coordframe '" 
                    + coodframeConfig.CoodframeName
                    + "' by calling EndCoordframe() function before calling next BeginCoordframe(...)");

            if (frameNames.Contains(coordframeName))
                throw new ArgumentException(coordframeName + " is already used. You can not have more " +
                    "than one identical coordframe names in single configuration.");

            if (coordframeName.Equals(""))
                coordframeName = "emptyString";

            coodframeConfig.CoodframeName = coordframeName;
            coodframeConfig.MinPoint = minPoints;
        }

        /// <summary>
        /// Ends the current coordinate frame configuration.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If called before the BeginCoordframe(...) call.
        /// </exception>
        public void EndCoordframe()
        {
            if (coodframeConfig.CoodframeName.Equals(""))
                throw new InvalidOperationException("You should call BeginCoordframe(...) function before " +
                    "calling EndCoordframe()");

            if (markerIDs.Count == 0)
                throw new InvalidOperationException("You can not end a coordframe section without adding " +
                    "any markers in '" + coodframeConfig.CoodframeName + "' section");

            coordframes.Add(coodframeConfig, new List<int>(markerIDs));
            coodframeConfig.CoodframeName = "";
            coodframeConfig.MinPoint = 0;
            markerIDs.Clear();
        }

        public void AddMarker(int markerID, Point upperLeftCorner, String imagefile)
        {
            AddMarker(markerID, upperLeftCorner, Size.Empty, imagefile);
        }

        public void AddMarker(int markerID, Point upperLeftCorner, Size size, String imagefile)
        {
            AddMarker(markerID, upperLeftCorner, size, Rectangle.Empty, imagefile);
        }

        public void AddMarker(int markerID, Point upperLeftCorner, Rectangle cropArea, String imagefile)
        {
            AddMarker(markerID, upperLeftCorner, Size.Empty, cropArea, imagefile);
        }

        /// <summary>
        /// Adds a marker with its configurations and image to layout manager. 
        /// </summary>
        /// <param name="markerID">The ID of the marker (specified by the ARTag)</param>
        /// <param name="upperLeftCorner">The upper left corner (in pixels) of this marker in the entire
        /// marker array.</param>
        /// <param name="size">The size of this marker to be printed on the marker array. If different
        /// from the original image size, the marker image will be resized to fit this size.</param>
        /// <param name="cropArea">The area to be retained after cropping other parts outside of
        /// this area</param>
        /// <param name="imagefile">The file name of the marker image (must have an extension that
        /// indicates the image format)</param>
        /// <remarks>
        /// This function can only be called between BeginCoordframe(...) and EndCoordframe()
        /// function calls.
        /// </remarks>
        /// <exception cref="ArgumentException">If the given marker ID is already added</exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <see cref="BeginCoordframe"/>
        /// <seealso cref="EndCoordframe"/>
        public void AddMarker(int markerID, Point upperLeftCorner, Size size, Rectangle cropArea, 
            String imagefile)
        {
            if (coodframeConfig.CoodframeName.Equals(""))
                throw new InvalidOperationException("You can only call this function between " +
                    "BeginCoordframe(...) and EndCoordframe() function calls");

            if (configs.ContainsKey(markerID))
                throw new ArgumentException("You can't add same marker ID in one config file");

            MarkerConfig config = new MarkerConfig();
            config.ID = markerID;
            config.Position = upperLeftCorner;

            int width = 0, height = 0;
            config.Image = ImageReader.Load(imagefile, ref width, ref height);
            config.ImageWidth = width;
            config.ImageHeight = height;
            config.CropArea = cropArea;
            if (size.Equals(Size.Empty))
            {
                if (!cropArea.Equals(Rectangle.Empty))
                    config.Size = cropArea.Size;
                else
                    config.Size = new Size(width, height);
            }
            else
                config.Size = size;

            // if crop area is not empty, then make sure that the crop area is within the marker size
            if (!cropArea.Equals(Rectangle.Empty))
            {
                if (cropArea.X < 0 || cropArea.X > width)
                    throw new ArgumentException("Your crop area's X position is outside of the marker size");
                if (cropArea.Y < 0 || cropArea.Y > height)
                    throw new ArgumentException("Your crop area's Y position is outside of the marker size");
                if ((cropArea.X + cropArea.Width) > width)
                    throw new ArgumentException("Your crop area's X dimension exceeds the marker width");
                if ((cropArea.Y + cropArea.Height) > height)
                    throw new ArgumentException("Your crop area's Y dimension exceeds the marker height");
            }

            configs.Add(markerID, config);
            markerIDs.Add(markerID);
        }

        /// <summary>
        /// Compiles the marker layout image. This function should be called before calling the
        /// OutputImage(...) function.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Compile()
        {
            // Use BitmapData to read the pixels of the Bitmap image
            BitmapData bmpData = markerImage.LockBits(
                new Rectangle(0, 0, markerImage.Width, markerImage.Height),
                ImageLockMode.ReadWrite, markerImage.PixelFormat);

            // Clears the bitmap image with white color
            WhiteOut(bmpData);

            foreach (MarkerConfig config in configs.Values)
            {
                if(((config.Position.X + config.Size.Width) > markerImage.Size.Width) ||
                    ((config.Position.Y + config.Size.Height) > markerImage.Size.Height))
                    throw new Exception("Marker: " + config.ID + " layout (position + size) exceeds the " +
                        "limit of the marker image size");

                byte[] img = config.Image;
                Size origSize = new Size(config.ImageWidth, config.ImageHeight);

                if (!config.CropArea.Equals(Rectangle.Empty))
                {
                    img = ImageReader.Crop(img, origSize, config.CropArea);
                    origSize = config.CropArea.Size;
                }

                if (!config.Size.Equals(origSize))
                    img = ImageReader.Resize(img, origSize, config.Size, 
                        ImageReader.ResizeOption.NearestNeighbor);

                try
                {
                    unsafe
                    {
                        byte* dest = (byte*)bmpData.Scan0;
                        dest += bmpData.Stride * config.Position.Y;
                        for (int i = 0; i < config.Size.Height; i++)
                        {
                            for (int j = config.Position.X; j < config.Position.X + config.Size.Width; j++)
                            {
                                *(dest + j) = img[i * config.Size.Width + (j - config.Position.X)];
                            }

                            dest += bmpData.Stride;
                        }
                    }
                }
                catch (AccessViolationException)
                {
                    throw new Exception("Marker: " + config.ID + " layout (position + size) exceeds the " +
                        "limit of the marker image size");
                }
            }

            markerImage.UnlockBits(bmpData);
        }

        /// <summary>
        /// Outputs the marker layout image in the specified image format.
        /// </summary>
        /// <param name="imageFilename">The name of the image file to be generated</param>
        /// <param name="format">The image format of the generated marker layout image</param>
        /// <remarks>
        /// You need to call Compile() function before calling this function. Otherwise, the generated
        /// image will be a white image.
        /// </remarks>
        /// <see cref="Compile"/>
        public void OutputImage(String imageFilename, System.Drawing.Imaging.ImageFormat format)
        {
            markerImage.Save(imageFilename, format);
        }

        /// <summary>
        /// Outputs an ARTag configuration file based on the marker layout. If you have or generated 
        /// other configuration files, and want to append the config information to this configuration
        /// file, then you can pass the filenames of those additional configuration files to 'appendConfigs'
        /// parameter.
        /// </summary>
        /// <param name="configFilename">The name of the configuration file to be generated</param>
        /// <param name="type">The type of config file depending on different tracking libraries</param>
        /// <param name="appendConfigs">A list of external configuration file names to append at the end
        /// of the configuration file to be generated</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void OutputConfig(String configFilename, params String[] appendConfigs)
        {
            if (!coodframeConfig.CoodframeName.Equals(""))
                throw new InvalidOperationException("You forgot to end your current coordframe: " +
                    coodframeConfig.CoodframeName 
                    + ". Call EndCoordframe() before calling this OutputConfig(...)");

            float ratio = 1 / (float)pixelPerInch;

            try
            {
                if (configFilename.EndsWith(".xml"))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "no");

                    foreach (KeyValuePair<CoordframeConfig, List<int>> coordframe in coordframes)
                    {
                        XmlElement xmlRootNode = xmlDoc.CreateElement("multimarker");
                        xmlRootNode.SetAttribute("markers", coordframe.Value.Count.ToString());
                        xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
                        xmlDoc.AppendChild(xmlRootNode);

                        foreach (int markerID in coordframe.Value)
                        {
                            XmlElement markerXml = xmlDoc.CreateElement("marker");
                            markerXml.SetAttribute("index", markerID.ToString());
                            markerXml.SetAttribute("status", "2");

                            MarkerConfig config = configs[markerID];

                            int x = config.Position.X - configCenter.X;
                            int y = config.Position.Y - configCenter.Y;

                            PointF configPos = new PointF(x * ratio, y * ratio);
                            SizeF configSize = new SizeF(config.Size.Width * ratio, config.Size.Height * ratio);

                            XmlElement[] cornerXml = new XmlElement[4];
                            cornerXml[0] = xmlDoc.CreateElement("corner");
                            cornerXml[0].SetAttribute("x", configPos.X.ToString());
                            cornerXml[0].SetAttribute("y", "" + (-(configPos.Y + configSize.Height)));
                            cornerXml[0].SetAttribute("z", "0");

                            cornerXml[1] = xmlDoc.CreateElement("corner");
                            cornerXml[1].SetAttribute("x", "" + (configPos.X + configSize.Width));
                            cornerXml[1].SetAttribute("y", "" + (-(configPos.Y + configSize.Height)));
                            cornerXml[1].SetAttribute("z", "0");

                            cornerXml[2] = xmlDoc.CreateElement("corner");
                            cornerXml[2].SetAttribute("x", "" + (configPos.X + configSize.Width));
                            cornerXml[2].SetAttribute("y", "" + (-configPos.Y));
                            cornerXml[2].SetAttribute("z", "0");

                            cornerXml[3] = xmlDoc.CreateElement("corner");
                            cornerXml[3].SetAttribute("x", "" + configPos.X);
                            cornerXml[3].SetAttribute("y", "" + (-configPos.Y));
                            cornerXml[3].SetAttribute("z", "0");

                            foreach (XmlElement corner in cornerXml)
                                markerXml.AppendChild(corner);

                            xmlRootNode.AppendChild(markerXml);
                        }
                    }

                    xmlDoc.Save(configFilename);
                }
                else
                {
                    TextWriter tw = new StreamWriter(configFilename);

                    foreach (KeyValuePair<CoordframeConfig, List<int>> coordframe in coordframes)
                    {
                        tw.WriteLine("" + coordframe.Value.Count);
                        tw.WriteLine();

                        foreach (int markerID in coordframe.Value)
                            tw.WriteLine("" + markerID);
                        tw.WriteLine();

                        foreach (int markerID in coordframe.Value)
                            tw.WriteLine("2"); // status indicator
                        tw.WriteLine();

                        foreach (int markerID in coordframe.Value)
                        {
                            MarkerConfig config = configs[markerID];

                            int x = config.Position.X - configCenter.X;
                            int y = config.Position.Y - configCenter.Y;

                            PointF configPos = new PointF(x * ratio, y * ratio);
                            SizeF configSize = new SizeF(config.Size.Width * ratio, config.Size.Height * ratio);
                            tw.WriteLine("" + configPos.X + " " + (-(configPos.Y + configSize.Height)) + " 0");
                            tw.WriteLine("" + (configPos.X + configSize.Width) + " " +
                                (-(configPos.Y + configSize.Height)) + " 0");
                            tw.WriteLine("" + (configPos.X + configSize.Width) + " " + (-configPos.Y) + " 0");
                            tw.WriteLine("" + configPos.X + " " + (-configPos.Y) + " 0");
                        }

                        tw.Write("\n\n"); // add two blank gaps
                    }

                    tw.Close();
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message + exp.StackTrace);
            }
        }

        public void Dispose()
        {
            configs.Clear();
            coordframes.Clear();
            markerImage.Dispose();
        }

        #endregion

        #region Private Methods

        private void Initialize(int width, int height)
        {
            markerImage = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            // Specify the 256 indexed color to be 256 grayscale color
            ColorPalette grayPalette = markerImage.Palette;
            for (int i = 0; i < grayPalette.Entries.Length; i++)
                grayPalette.Entries[i] = Color.FromArgb(i, i, i);
            markerImage.Palette = grayPalette;

            configs = new Dictionary<int, MarkerConfig>();
            coordframes = new Dictionary<CoordframeConfig, List<int>>();
            markerIDs = new List<int>();
            frameNames = new List<string>();

            coodframeConfig.CoodframeName = "";
            coodframeConfig.MinPoint = 0;
        }

        /// <summary>
        /// Clears the given Bitmap image with white (255) pixels.
        /// </summary>
        /// <param name="bmpData"></param>
        private void WhiteOut(BitmapData bmpData)
        {
            unsafe
            {
                byte* dest = (byte*)bmpData.Scan0;
                for (int i = 0; i < bmpData.Height * bmpData.Stride; i++)
                    *(dest + i) = (byte)255;
            }
        }

        /// <summary>
        /// Parses an XML file that contains marker configurations.
        /// </summary>
        /// <param name="xmlFile"></param>
        private void ParseXML(String xmlFile)
        {
            XmlTextReader reader = new XmlTextReader(xmlFile);

            bool layoutStarted = false;
            bool coordframeStarted = false;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.Equals("MarkerLayout"))
                        {
                            if (layoutStarted)
                                throw new XmlException("You can not include more than one 'MarkerLayout' " +
                                    "element in a single XML file");

                            try
                            {
                                int width = int.Parse(reader.GetAttribute("width"));
                                int height = int.Parse(reader.GetAttribute("height"));
                                pixelPerInch = int.Parse(reader.GetAttribute("pixelPerInch"));
                                if (reader.GetAttribute("center") != null)
                                {
                                    String[] ints = reader.GetAttribute("center").Split(',');
                                    configCenter = new Point(int.Parse(ints[0]), int.Parse(ints[1]));
                                }
                                else
                                    configCenter = Point.Empty;

                                Initialize(width, height);
                            }
                            catch (Exception)
                            {
                                throw new XmlException("You need to have 'width', 'height', and 'pixelPerInch' "
                                    + "attributes in the 'MarkerLayout' element, and an optional 'center' " +
                                    "attribute. 'width', 'height', and 'pixelPerInch' has to be an integer, " +
                                    "and 'center' has to include two integers separated by a comma.");
                            }

                            layoutStarted = true;
                        }
                        else if (reader.Name.Equals("Coordframe"))
                        {
                            if (!layoutStarted)
                                throw new XmlException("'Coordframe' has to be defined inside 'MarkerLayout' " +
                                    "element clause");

                            if (coordframeStarted)
                                throw new XmlException("You need to end 'Coordframe' element by adding " +
                                    "'/Coordframe' end element before going to the next 'Coordframe'");

                            try
                            {
                                String name = reader.GetAttribute("name");
                                int minPoints = 0;
                                if (reader.GetAttribute("minPoints") != null)
                                    minPoints = int.Parse(reader.GetAttribute("minPoints"));

                                BeginCoordframe(name, minPoints);
                                coordframeStarted = true;
                            }
                            catch (Exception)
                            {
                                throw new XmlException("You need to have 'name' attribute in the 'Coordframe' "
                                    + "element, and an optional 'minPoints' attribute. 'minPoints' attribute "
                                    + "has to be an integer.");
                            }
                        }
                        else if (reader.Name.Equals("Marker"))
                        {
                            if (!coordframeStarted)
                                throw new XmlException("'Marker' has to be defined inside 'Coordframe' " +
                                    "element clause");

                            try
                            {
                                int id = int.Parse(reader.GetAttribute("id"));
                                String filename = reader.GetAttribute("image");
                                String[] ints = reader.GetAttribute("upperLeftCorner").Split(',');
                                Point upperLeftCorner = new Point(int.Parse(ints[0]), int.Parse(ints[1]));
                                Size size = Size.Empty;
                                Rectangle cropArea = Rectangle.Empty;
                                if (reader.GetAttribute("size") != null)
                                {
                                    ints = reader.GetAttribute("size").Split(',');
                                    size = new Size(int.Parse(ints[0]), int.Parse(ints[1]));
                                }
                                if (reader.GetAttribute("cropArea") != null)
                                {
                                    ints = reader.GetAttribute("cropArea").Split(',');
                                    cropArea = new Rectangle(int.Parse(ints[0]), int.Parse(ints[1]),
                                        int.Parse(ints[2]), int.Parse(ints[3]));
                                }

                                AddMarker(id, upperLeftCorner, size, cropArea, filename);
                            }
                            catch (Exception)
                            {
                                throw new XmlException("You need to 'id', 'image', and 'upperLeftCorner' " +
                                    "attributes in the 'Marker' element, and optional 'size' and 'cropArea' " +
                                    "attributes. 'id' has to an integer; 'image' has to be a string that " +
                                    "specifies the filepath; 'upperLeftCorner' and 'size' has to contain two " +
                                    "integers separated by a comma; and 'cropArea' has to contain four " +
                                    "integers each separated by a comma");
                            }
                        }
                        else if (reader.Name.Equals("OutputImage"))
                        {
                            String name = "";
                            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Gif;
                            if (reader.GetAttribute("name") != null)
                                name = reader.GetAttribute("name");
                            else
                                throw new XmlException("You need to have 'name' attribute in the 'OutputImage' "
                                    + "element and an optional 'format' attribute. 'format' has to be one of "
                                    + "the following string: Gif, Jpeg, Bmp, or Tiff. The default 'format' is " 
                                    + "Gif");

                            if (reader.GetAttribute("format") != null)
                            {
                                String fmt = reader.GetAttribute("format");
                                if (fmt.Equals("Gif"))
                                    format = System.Drawing.Imaging.ImageFormat.Gif;
                                else if (fmt.Equals("Jpeg"))
                                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                                else if (fmt.Equals("Bmp"))
                                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                                else if (fmt.Equals("Tiff"))
                                    format = System.Drawing.Imaging.ImageFormat.Tiff;
                            }

                            Compile();
                            OutputImage(name, format);
                        }
                        else if (reader.Name.Equals("OutputConfig"))
                        {
                            String name = "";
                            if (reader.GetAttribute("name") != null)
                                name = reader.GetAttribute("name");
                            else
                                throw new XmlException("You need to have 'name' attribute in the 'OutputConfig' "
                                    + "element and an optional 'appends' attribute. 'appends' has to be a comma "
                                    + "separated strings that specify each file to be appended.");

                            if (reader.GetAttribute("appends") != null)
                            {
                                String[] appends = reader.GetAttribute("appends").Split(',');
                                OutputConfig(name, appends);
                            }
                            else
                                OutputConfig(name);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name.Equals("Coordframe"))
                        {
                            EndCoordframe();
                            coordframeStarted = false;
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
