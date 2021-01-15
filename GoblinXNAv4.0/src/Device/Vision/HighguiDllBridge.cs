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
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GoblinXNA.Device.Vision
{
    public partial class OpenCVWrapper
    {
        //private const String HIGHGUI_DLL_LIB = "libhighgui200.dll";
        //private const String HIGHGUI_DLL_LIB = "opencv_highgui220.dll";
        private const String HIGHGUI_DLL_LIB = "highgui210.dll";

        #region highgui.dll wrapper code

        #region Property Access Constants

        public static int CV_CAP_PROP_POS_MSEC      = 0;
        public static int CV_CAP_PROP_POS_FRAMES    = 1;
        public static int CV_CAP_PROP_POS_AVI_RATIO = 2;
        public static int CV_CAP_PROP_FRAME_WIDTH   = 3;
        public static int CV_CAP_PROP_FRAME_HEIGHT  = 4;
        public static int CV_CAP_PROP_FPS           = 5;
        public static int CV_CAP_PROP_FOURCC        = 6;
        public static int CV_CAP_PROP_FRAME_COUNT   = 7;
        public static int CV_CAP_PROP_FORMAT        = 8;
        public static int CV_CAP_PROP_MODE          = 9;
        public static int CV_CAP_PROP_BRIGHTNESS    = 10;
        public static int CV_CAP_PROP_CONTRAST      = 11;
        public static int CV_CAP_PROP_SATURATION    = 12;
        public static int CV_CAP_PROP_HUE           = 13;
        public static int CV_CAP_PROP_GAIN          = 14;
        public static int CV_CAP_PROP_EXPOSURE      = 15;
        public static int CV_CAP_PROP_CONVERT_RGB   = 16;
        public static int CV_CAP_PROP_WHITE_BALANCE = 17;
        public static int CV_CAP_PROP_RECTIFICATION = 18;

        #endregion

        #region Image Load Constants

        public static int CV_LOAD_IMAGE_COLOR = 1;
        public static int CV_LOAD_IMAGE_GRAYSCALE = 0;
        public static int CV_LOAD_IMAGE_UNCHANGED = -1;

        #endregion

        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvLoadImage", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cvLoadImage(string filename, int iscolor);

        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvSaveImage", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cvSaveImage(string filename, IntPtr imageData, IntPtr param);

        /// <summary>
        /// Captures an image from a video device.
        /// </summary>
        /// <param name="index">Index of the camera to be used.</param>
        /// <returns>This pointer corresponds to the CvCatpure pointer used throughout OpenCV</returns>
        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvCreateCameraCapture", CallingConvention=CallingConvention.Cdecl)]
        public static extern IntPtr cvCaptureFromCAM(int index);

        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvCreateFileCapture", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cvCaptureFromFile(string filename);

        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvGetCaptureProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern double cvGetCaptureProperty(IntPtr capture, int propertyId);

        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvGrabFrame", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cvGrabFrame(IntPtr capture);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capture"></param>
        /// <returns>IplImage</returns>
        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvQueryFrame", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cvQueryFrame(IntPtr capture);

        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvReleaseCapture", CallingConvention = CallingConvention.Cdecl)]
        public static extern void cvReleaseCapture(ref IntPtr capture);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capture"></param>
        /// <returns>IplImage</returns>
        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvRetrieveFrame", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cvRetrieveFrame(IntPtr capture);

        [DllImport(HIGHGUI_DLL_LIB, EntryPoint = "cvSetCaptureProperty", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cvSetCaptureProperty(IntPtr capture, int propertyId, double value);

        #endregion

        #region Helper Functions 


        #endregion
    }
}
