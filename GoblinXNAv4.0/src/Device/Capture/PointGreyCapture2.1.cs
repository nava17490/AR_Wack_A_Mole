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
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Graphics;

using GoblinXNA.Helpers;

using FlyCapture2Managed;

namespace GoblinXNA.Device.Capture
{
    /// <summary>
    /// A video capture class that uses the Point Grey FlyCapture library. This is for Point
    /// Grey cameras.
    /// </summary>
    public class PointGreyCapture_2_1 : IVideoCapture
    {
        #region Member Fields

        private ManagedCameraBase camera;
        private ManagedImage processedImage;

        private int videoDeviceID;

        private int cameraWidth;
        private int cameraHeight;
        private bool grayscale;
        private bool cameraInitialized;
        private Resolution resolution;
        private FrameRate frameRate;
        private ImageFormat format;
        private IResizer resizer;

        private VideoMode videoMode;
        private bool isVideoModeSet;

        private ImageReadyCallback imageReadyCallback;

        /// <summary>
        /// Used to count the number of times it failed to capture an image
        /// If it fails more than certain times, it will assume that the video
        /// capture device can not be accessed
        /// </summary>
        private int failureCount;

        private const int FAILURE_THRESHOLD = 100;

        private bool accessing;
        private byte[] pixelData;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a video capture using the Point Grey FlyCapture library.
        /// </summary>
        public PointGreyCapture_2_1()
        {
            cameraInitialized = false;
            videoDeviceID = -1;

            cameraWidth = 0;
            cameraHeight = 0;
            grayscale = false;

            failureCount = 0;
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

        public VideoMode VideoMode
        {
            get { return videoMode; }
            set
            {
                if (cameraInitialized)
                    throw new GoblinException("You should assign this property before calling InitVideoCapture");

                videoMode = value;
                isVideoModeSet = true;
            }
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

            switch (resolution)
            {
                case Resolution._160x120:
                    cameraWidth = 160;
                    cameraHeight = 120;
                    break;
                case Resolution._320x240:
                    cameraWidth = 320;
                    cameraHeight = 240;
                    break;
                case Resolution._640x480:
                    cameraWidth = 640;
                    cameraHeight = 480;
                    break;
                case Resolution._800x600:
                    cameraWidth = 800;
                    cameraHeight = 600;
                    break;
                case Resolution._1024x768:
                    cameraWidth = 1024;
                    cameraHeight = 768;
                    break;
                case Resolution._1280x1024:
                    cameraWidth = 1280;
                    cameraHeight = 960;
                    break;
                case Resolution._1600x1200:
                    cameraWidth = 1600;
                    cameraHeight = 1200;
                    break;
            }

            ManagedBusManager busMgr = new ManagedBusManager();
            uint numCameras = busMgr.GetNumOfCameras();
            if (videoDeviceID >= numCameras || videoDeviceID < 0)
                throw new GoblinException("VideoDeviceID: " + videoDeviceID + " is out of range. There are only " +
                    numCameras + " point grey camera(s) connected");

            ManagedPGRGuid guid = busMgr.GetCameraFromIndex((uint)videoDeviceID);
            InterfaceType ifType = busMgr.GetInterfaceTypeFromGuid(guid);

            if (ifType == InterfaceType.GigE)
                camera = new ManagedGigECamera();
            else
                camera = new ManagedCamera();

            camera.Connect(guid);

            processedImage = new ManagedImage();
            pixelData = new byte[cameraWidth * cameraHeight * 3];

            VideoMode currentVideoMode = VideoMode.NumberOfVideoModes;
            FlyCapture2Managed.FrameRate currentFrameRate = FlyCapture2Managed.FrameRate.NumberOfFrameRates;

            FlyCapture2Managed.FrameRate desiredFrameRate = FlyCapture2Managed.FrameRate.NumberOfFrameRates;

            switch (frameRate)
            {
                case FrameRate._15Hz:
                    desiredFrameRate = FlyCapture2Managed.FrameRate.FrameRate15;
                    break;
                case FrameRate._30Hz:
                    desiredFrameRate = FlyCapture2Managed.FrameRate.FrameRate30;
                    break;
                case FrameRate._60Hz:
                    desiredFrameRate = FlyCapture2Managed.FrameRate.FrameRate60;
                    break;
                case FrameRate._120Hz:
                    desiredFrameRate = FlyCapture2Managed.FrameRate.FrameRate120;
                    break;
                case FrameRate._240Hz:
                    desiredFrameRate = FlyCapture2Managed.FrameRate.FrameRate240;
                    break;
                default:
                    throw new GoblinException(framerate.ToString() + " is not supported");
            }

            try
            {
                ((ManagedCamera)camera).GetVideoModeAndFrameRate(ref currentVideoMode, ref currentFrameRate);

                if(!isVideoModeSet)
                    videoMode = currentVideoMode;

                bool supported = ((ManagedCamera)camera).GetVideoModeAndFrameRateInfo(videoMode, desiredFrameRate);

                if (supported)
                {
                    ((ManagedCamera)camera).SetVideoModeAndFrameRate(videoMode, desiredFrameRate);
                }
                else
                {
                    Log.Write(desiredFrameRate.ToString() + " is not supported in " + videoMode.ToString()  + " mode");
                }
            }
            catch (Exception exp)
            {
                Log.Write(exp.Message + ": " + exp.StackTrace);
            }

            camera.StartCapture(OnImageGrabbed);

            cameraInitialized = true;
        }

        public void GetImageTexture(int[] returnImage, ref IntPtr imagePtr)
        {
            int B = 0, G = 1, R = 2;

            accessing = true;
            if (processedImage != null)
            {
                failureCount = 0;
                if (imagePtr != IntPtr.Zero)
                {
                    Marshal.Copy(pixelData, 0, imagePtr, pixelData.Length);
                }

                bool replaceBackground = false;
                if (imageReadyCallback != null)
                    replaceBackground = imageReadyCallback(imagePtr, returnImage);

                if (!replaceBackground && (returnImage != null))
                {
                    int index = 0;
                    uint rawIndex = 0;
                    unsafe
                    {
                        for (int i = 0; i < processedImage.rows; i++)
                        {
                            for (int j = 0; j < processedImage.stride; j += 3, rawIndex += 3, ++index)
                            {
                                returnImage[index] =
                                    ((pixelData[rawIndex + R] << 16) |
                                        (pixelData[rawIndex + G] << 8) |
                                        (pixelData[rawIndex + B]));
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Write("Failed to capture image", Log.LogLevel.Log);
                failureCount++;

                if (failureCount > FAILURE_THRESHOLD)
                {
                    throw new GoblinException("Video capture device id:" + videoDeviceID +
                        " is used by other application, and can not be accessed");
                }
            }
            accessing = false;
        }

        public void Dispose()
        {
            if (camera.IsConnected())
            {
                camera.StopCapture();
                camera.Disconnect();
            }
        }

        #endregion

        #region Private Methods

        private void OnImageGrabbed(ManagedImage image)
        {
            while (accessing) { }
            image.Convert(PixelFormat.PixelFormatRgb, processedImage);
            unsafe
            {
                Marshal.Copy((IntPtr)processedImage.data, pixelData, 0, pixelData.Length);
            }
        }

        #endregion
    }
}
