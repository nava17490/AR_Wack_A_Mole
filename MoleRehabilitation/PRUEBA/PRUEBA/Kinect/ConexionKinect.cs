using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Kinect;
using GoblinXNA.Device.Capture;
using MoleRehabilitation.Interfaz;
using MoleRehabilitation.LogicaJuego;
using MoleRehabilitation.BD;
using Coding4Fun.Kinect.WinForm;

namespace MoleRehabilitation.Kinect
{
    class ConexionKinect : KinectMSCapture
    {
        #region Atributos
        private bool enableColor, enableDepth, enableSkeletal;
        private Texture2D pantallaError;
        private Skeleton[] skeletons;
        private Vector2 resolution;
        #endregion

        #region Constructores
        /// <summary>
        /// Crea una nueva conexion al dispositivo kinect
        /// </summary>
        /// <param name="enableColor"></param>
        /// <param name="enableDepth"></param>
        /// <param name="enableSkeletal"></param>
        /// <param name="pantallaError"></param>
        public ConexionKinect(bool enableColor, bool enableDepth, bool enableSkeletal, Texture2D pantallaError)
            : base(enableDepth)
        {
            this.enableColor = enableColor;
            this.enableDepth = enableDepth;
            this.enableSkeletal = enableSkeletal;
            this.pantallaError = pantallaError;
        }

        #endregion

        #region Metodos Públicos

        /// <summary>
        /// Inicializa la captura de video de Kinect
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public int initializeColorImage(Vector2 resolution)
        {
            this.resolution = resolution;
            base.InitVideoCapture(0, FrameRate._30Hz, Resolution._1280x1024, GoblinXNA.Device.Capture.ImageFormat.R8G8B8_24, false);
            return 0;
        }

        /// <summary>
        /// Inicializa la captura de posiciones de esqueleto
        /// </summary>
        /// <returns></returns>
        public int initializeSkeleton()
        {
            if (!enableSkeletal) return -1;
            base.Sensor.SkeletonStream.Enable();
            base.Sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
            return 0;
        }

        /// <summary>
        /// Desconecta Kinect
        /// </summary>
        public void Unitialize()
        {
            base.Sensor.Stop();
        }

        /// <summary>
        /// Devuelve si esta conectada la kinect
        /// </summary>
        /// <returns></returns>
        public bool isEnabled()
        {
            try
            {
                return !(base.Sensor.Status == KinectStatus.Disconnected);
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Métodos Privados
        
        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool receivedData = false;
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if (skeletons == null) //allocate the first time
                    {
                        skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }
                    receivedData = true;
                }
                if (receivedData)
                {
                    // DISPLAY OR PROCESS IMAGE DATA IN skeletons HERE
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    foreach (Skeleton s in skeletons)
                    {
                        if (SkeletonTrackingState.Tracked == s.TrackingState)
                        {
                            Hashtable joints = new Hashtable();
                            GestorJuego.Instancia.ExistePosicionIncorrecta = false;

                            foreach (Joint j in s.Joints)
                            {
                                joints.Add(j.JointType, SkeletalExtensions.ScaleTo(j, (int)resolution.X, (int)resolution.Y));
                            }
                            GestorJuego.Instancia.Joints = joints;
                        }
                        else
                            GestorJuego.Instancia.ExistePosicionIncorrecta = true;
                    }
                }
            }
            
        }

        #endregion
    }
}
