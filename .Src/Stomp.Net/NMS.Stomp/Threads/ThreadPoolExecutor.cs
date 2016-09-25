/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace Apache.NMS.Stomp.Threads
{
    /// <summary>
    ///     This class provides a wrapper around the ThreadPool mechanism in .NET
    ///     to allow for serial execution of jobs in the ThreadPool and provide
    ///     a means of shutting down the execution of jobs in a deterministic
    ///     way.
    /// </summary>
    public class ThreadPoolExecutor
    {
        #region Fields

        private Boolean closing;
        private readonly ManualResetEvent executionComplete = new ManualResetEvent( true );
        private Boolean running;
        private readonly Mutex syncRoot = new Mutex();
        private readonly Queue<Future> workQueue = new Queue<Future>();

        #endregion

        #region Properties

        public Boolean IsShutdown { get; private set; }

        #endregion

        public void QueueUserWorkItem( WaitCallback worker )
        {
            QueueUserWorkItem( worker, null );
        }

        public void QueueUserWorkItem( WaitCallback worker, Object arg )
        {
            if ( worker == null )
                throw new ArgumentNullException( "Invalid WaitCallback passed" );

            if ( !IsShutdown )
                lock ( syncRoot )
                    if ( !IsShutdown || !closing )
                    {
                        workQueue.Enqueue( new Future( worker, arg ) );

                        if ( !running )
                        {
                            executionComplete.Reset();
                            running = true;
                            ThreadPool.QueueUserWorkItem( QueueProcessor, null );
                        }
                    }
        }

        public void Shutdown()
        {
            if ( !IsShutdown )
            {
                syncRoot.WaitOne();

                if ( !IsShutdown )
                {
                    closing = true;
                    workQueue.Clear();

                    if ( running )
                    {
                        syncRoot.ReleaseMutex();
                        executionComplete.WaitOne();
                        syncRoot.WaitOne();
                    }

                    IsShutdown = true;
                }

                syncRoot.ReleaseMutex();
            }
        }

        private void QueueProcessor( Object unused )
        {
            Future theTask = null;

            lock ( syncRoot )
            {
                if ( workQueue.Count == 0 || closing )
                {
                    running = false;
                    executionComplete.Set();
                    return;
                }

                theTask = workQueue.Dequeue();
            }

            try
            {
                theTask.Run();
            }
            finally
            {
                if ( closing )
                {
                    running = false;
                    executionComplete.Set();
                }
                else
                {
                    ThreadPool.QueueUserWorkItem( QueueProcessor, null );
                }
            }
        }

        #region Nested Types

        /// <summary>
        ///     Represents an asynchronous task that is executed on the ThreadPool
        ///     at some point in the future.
        /// </summary>
        internal class Future
        {
            #region Fields

            private readonly WaitCallback callback;
            private readonly Object callbackArg;

            #endregion

            #region Ctor

            public Future( WaitCallback callback, Object arg )
            {
                this.callback = callback;
                callbackArg = arg;
            }

            #endregion

            public void Run()
            {
                if ( callback == null )
                    throw new Exception( "Future executed with null WaitCallback" );

                try
                {
                    callback( callbackArg );
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}