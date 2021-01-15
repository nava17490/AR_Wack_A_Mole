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
 * Author: Ohan Oda (ohan@cs.columbia.edu)
 * 
 *************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using GoblinXNA.Device.Vision;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Kinect;

namespace GoblinXNA.Device.Capture
{
    /// <summary>
    /// An implementation of IVideoCapture for Kinect depth camera. This implementation uses
    /// Microsoft Kinect SDK.
    /// </summary>
    public class KinectMSCapture : IVideoCapture
    {
        #region Member Fields

        private int videoDeviceID;

        private int cameraWidth;
        private int cameraHeight;
        private bool grayscale;
        private bool cameraInitialized;
        private Resolution resolution;
        private FrameRate frameRate;
        private ImageFormat format;
        private IResizer resizer;

        private ImageReadyCallback imageReadyCallback;

        private KinectSensor sensor;

        private int[] videoData;
        private byte[] rawVideo;

        private bool copyingRawVideo = false;

        private bool depthStreamEnabled;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a video capture using the Kinect camera with the Microsoft SDK.
        /// </summary>
        /// <param name="depthFrameEnabled">Whether the depth frame will be enabled. If set to true, the color
        /// frame will be obtained in AllFramesReady event so that the color frame is synchronized with the
        /// depth frame. Otherwise, the color frame will be obtained in ColorFrameReady event.</param>
        public KinectMSCapture(bool depthStreamEnabled)
        {
            cameraInitialized = false;
            videoDeviceID = -1;

            this.depthStreamEnabled = depthStreamEnabled;

            cameraWidth = 0;
            cameraHeight = 0;
            grayscale = false;

            imageReadyCallback = null;
        }

        #endregion

        #region Properties

        public int Width
        {
            get { return cameraWidth; }
        }

        public int Height
        {
            get { return cameraHeight; }
        }

        public int VideoDeviceID
        {
            get { return videoDeviceID; }
        }

        public bool GrayScale
        {
            get { return grayscale; }
        }

        public bool Initialized
        {
            get { return cameraInitialized; }
        }

        public ImageFormat Format
        {
            get { return format; }
        }

        public IResizer MarkerTrackingImageResizer
        {
            get { return resizer; }
            set { resizer = value; }
        }

        public SpriteEffects RenderFormat
        {
            get { return SpriteEffects.None; }
        }

        public ImageReadyCallback CaptureCallback
        {
            set { imageReadyCallback = value; }
        }

        public KinectSensor Sensor
        {
            get { return sensor; }
        }

        /// <summary>
        /// Gets or sets whether this Kinect capture is currently used for calibration purpose only.
        /// If you're performing calibration using CameraCalibration project, make sure to set this
        /// to true.
        /// </summary>
        public bool UsedForCalibration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether to mirror the video image. Note that this will affect the image pointer
        /// which is used for vision tracking as well.
        /// </summary>
        public bool MirrorImage
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        public void InitVideoCapture(int videoDeviceID, FrameRate framerate, Resolution resolution,
            ImageFormat format, bool grayscale)
        {
            if (cameraInitialized)
                return;

            this.resolution = resolution;
            this.grayscale = grayscale;
            this.frameRate = framerate;
            this.videoDeviceID = videoDeviceID;
            this.format = format;

            ColorImageFormat colorFormat = ColorImageFormat.Undefined;

            switch (resolution)
            {
                case Resolution._640x480:
                    cameraWidth = 640;
                    cameraHeight = 480;
                    colorFormat = ColorImageFormat.RgbResolution640x480Fps30;
                    break;
                case Resolution._1280x1024:
                    cameraWidth = 1280;
                    cameraHeight = 960;
                    colorFormat = ColorImageFormat.RgbResolution1280x960Fps12;
                    break;
                default:
                    throw new GoblinException(resolution.ToString() + " is not supported by Kinect video. The only " +
                        "supported resolution is 640x480");
            }

            if (framerate != FrameRate._30Hz)
                throw new GoblinException(framerate.ToString() + " is not supported by Kinect video. The only supported " +
                    "frame rate is 30 Hz");

            sensor = KinectSensor.KinectSensors[0];

            sensor.ColorStream.Enable(colorFormat);

            sensor.Start();

            if(depthStreamEnabled)
                sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(AllImagesReady);
            else
                sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(VideoImageReady);

            if (UsedForCalibration)
                videoData = new int[cameraWidth * cameraHeight];

            cameraInitialized = true;
        }

        public void GetImageTexture(int[] returnImage, ref IntPtr imagePtr)
        {
            if (returnImage != null)
            {
                if (UsedForCalibration)
                    Buffer.BlockCopy(videoData, 0, returnImage, 0, videoData.Length * sizeof(int));
                else
                    videoData = returnImage;
            }

            if (imagePtr != IntPtr.Zero)
            {
                copyingRawVideo = true;
                switch (format)
                {
                    case ImageFormat.B8G8R8A8_32:
                        Marshal.Copy(videoData, 0, imagePtr, videoData.Length);
                        break;
                    case ImageFormat.R8G8B8_24:
                        unsafe
                        {
                            byte* dst = (byte*)imagePtr;
                            int color = 0;
                            int dstIndex = 0;
                            for(int i = 0; i < videoData.Length; ++i, dstIndex += 3)
                            {
                                color = videoData[i];
                                *(dst + dstIndex) = (byte)(color >> 16);
                                *(dst + dstIndex + 1) = (byte)(color >> 8);
                                *(dst + dstIndex + 2) = (byte)(color);
                            }
                        }
                        break;
                }
                copyingRawVideo = false;
            }
        }

        public void Dispose()
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor.Dispose();
            }
        }

        #endregion

        #region Private Methods

        private void AllImagesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (!UsedForCalibration && videoData == null)
                return;

            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    CopyFrameData(imageFrame);
                }
            }
        }

        private void VideoImageReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (!UsedForCalibration && videoData == null)
                return;

            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {
                    CopyFrameData(imageFrame);
                }
            }
        }

        private void CopyFrameData(ColorImageFrame imageFrame)
        {
            while (copyingRawVideo) { }

            if (rawVideo == null)
                rawVideo = new byte[imageFrame.PixelDataLength];

            imageFrame.CopyPixelDataTo(rawVideo);

            int videoIndex = 0;
            int index = 0;

            if (MirrorImage)
            {
                for (int i = 0; i < imageFrame.Height; ++i)
                {
                    index = i * imageFrame.Width;
                    for (int j = imageFrame.Width - 1; j >= 0; --j,
                        videoIndex += imageFrame.BytesPerPixel)
                        videoData[index + j] = (int)(rawVideo[videoIndex] << 16 | rawVideo[videoIndex + 1] << 8 |
                            rawVideo[videoIndex + 2]);
                }
            }
            else
            {
                for (int i = 0; i < videoData.Length; ++i, videoIndex += imageFrame.BytesPerPixel)
                {
                    videoData[i] = (int)(rawVideo[videoIndex] << 16 | rawVideo[videoIndex + 1] << 8 |
                        rawVideo[videoIndex + 2]);
                }
            }

            if (imageReadyCallback != null)
                imageReadyCallback(IntPtr.Zero, videoData);
        }

        #endregion
    }
}
