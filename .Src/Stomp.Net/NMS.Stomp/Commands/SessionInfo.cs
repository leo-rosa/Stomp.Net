/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using Apache.NMS.Stomp.State;

#endregion

namespace Apache.NMS.Stomp.Commands
{
    public class SessionInfo : BaseCommand
    {
        #region Fields

        #endregion

        #region Properties

        public SessionId SessionId { get; set; }

        /// <summery>
        ///     Return an answer of true to the isSessionInfo() query.
        /// </summery>
        public override Boolean IsSessionInfo
        {
            get { return true; }
        }

        #endregion

        #region Ctor

        public SessionInfo()
        {
        }

        public SessionInfo( ConnectionInfo connectionInfo, Int64 sessionId )
        {
            SessionId = new SessionId( connectionInfo.ConnectionId, sessionId );
        }

        #endregion

        /// <summery>
        ///     Get the unique identifier that this object and its own
        ///     Marshaler share.
        /// </summery>
        public override Byte GetDataStructureType()
        {
            return DataStructureTypes.SessionInfoType;
        }

        /// <summery>
        ///     Returns a string containing the information for this DataStructure
        ///     such as its type and value of its elements.
        /// </summery>
        public override String ToString()
        {
            return GetType()
                       .Name + "[" + "SessionId=" + SessionId + "]";
        }

        public override Response visit( ICommandVisitor visitor )
        {
            return visitor.processAddSession( this );
        }
    }
}