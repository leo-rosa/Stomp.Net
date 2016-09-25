/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

#region Usings

using System;
using System.Reflection;

#endregion

namespace Apache.NMS.Util
{
    /// <summary>
    ///     Utility class used to set NMS properties via introspection for IMessage derived
    ///     instances.  This class allows IMessage classes to define Message specific properties
    ///     that can be accessed using the standard property get / set semantics.
    ///     This is especially useful for NMSX type properties which can vary by provider and
    ///     are obtianed via a call to IConnectionMetaData.NMSXPropertyNames.  The client can
    ///     set the properties on an IMessage instance without a direct cast to the providers
    ///     specific Message types.
    ///     Properties accessed in this way are treated as NMS Message headers which are never
    ///     read-only therefore there is no exception thrown if the message itself is in the
    ///     read-only property mode.
    /// </summary>
    public class MessagePropertyIntercepter : PrimitiveMapInterceptor
    {
        #region Constants

        private const BindingFlags publicBinding = BindingFlags.Public | BindingFlags.Instance;

        #endregion

        #region Fields

        private readonly Type messageType;

        #endregion

        #region Ctor

        public MessagePropertyIntercepter( IMessage message, IPrimitiveMap properties )
            : base( message, properties )
        {
            messageType = message.GetType();
        }

        public MessagePropertyIntercepter( IMessage message, IPrimitiveMap properties, Boolean readOnly )
            : base( message, properties, readOnly )
        {
            messageType = message.GetType();
        }

        #endregion

        protected override Object GetObjectProperty( String name )
        {
            var propertyInfo = messageType.GetProperty( name, publicBinding );

            if ( name.StartsWith( "NMS" ) )
                if ( null != propertyInfo && propertyInfo.CanRead )
                {
                    return propertyInfo.GetValue( message, null );
                }
                else
                {
                    var fieldInfo = messageType.GetField( name, publicBinding );

                    if ( null != fieldInfo )
                        return fieldInfo.GetValue( message );
                }

            return base.GetObjectProperty( name );
        }

        protected override void SetObjectProperty( String name, Object value )
        {
            var propertyInfo = messageType.GetProperty( name, publicBinding );

            if ( !name.StartsWith( "NMS" ) )
            {
                base.SetObjectProperty( name, value );
            }
            else if ( null != propertyInfo && propertyInfo.CanWrite )
            {
                propertyInfo.SetValue( message, value, null );
            }
            else
            {
                var fieldInfo = messageType.GetField( name, publicBinding );

                if ( null != fieldInfo && !fieldInfo.IsLiteral && !fieldInfo.IsInitOnly )
                    fieldInfo.SetValue( message, value );
                else
                    base.SetObjectProperty( name, value );
            }
        }
    }
}