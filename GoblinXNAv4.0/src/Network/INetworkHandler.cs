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

namespace GoblinXNA.Network
{
    public enum TransferSize { Byte = 1, UShort = 2, Int = 4 };

    /// <summary>
    /// An interface that defines how messages will be transferred over the network.
    /// </summary>
    public interface INetworkHandler
    {
        /// <summary>
        /// Gets or sets the specific network client implementation.
        /// </summary>
        IClient NetworkClient { get; set; }

        /// <summary>
        /// Gets or sets the specific network server implementation.
        /// </summary>
        IServer NetworkServer { get; set; }

        /// <summary>
        /// Gets or sets the size to use when transfering each INetworkObject. For example, if you
        /// are going to transfer INetworkObjects that will contain less than than 256 bytes including
        /// its Identifier length, then TransferSize.Byte would be good enough. The default size is
        /// TransferSize.Short.
        /// </summary>
        TransferSize TransferSizePerNetworkObject { get; set; }

        /// <summary>
        /// Adds a network object to send or receive messages associated with the
        /// object over the network.
        /// </summary>
        /// <param name="networkObj">A network object to be transfered over the network</param>
        void AddNetworkObject(INetworkObject networkObj);

        /// <summary>
        /// Removes a network object.
        /// </summary>
        /// <param name="networkObj">A network object to be transfered over the network</param>
        void RemoveNetworkObject(INetworkObject networkObj);

        /// <summary>
        /// Disposes the network objects.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Retrieves and broadcasts messages over the network.
        /// </summary>
        /// <param name="elapsedMsecs"></param>
        void Update(float elapsedMsecs);
    }
}
