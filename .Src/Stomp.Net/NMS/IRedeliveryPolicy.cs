

#region Usings

using System;

#endregion

namespace Apache.NMS
{
    public interface IRedeliveryPolicy : ICloneable
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the collision avoidance percent.  This causes the redelivery delay
        ///     to be adjusted in order to avoid possible collision when messages are redelivered
        ///     to concurrent consumers.
        /// </summary>
        /// <value>The collision avoidance factor.</value>
        Int32 CollisionAvoidancePercent { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to [use collision avoidance].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use collision avoidance]; otherwise, <c>false</c>.
        /// </value>
        Boolean UseCollisionAvoidance { get; set; }

        /// <summary>
        ///     The time in milliseconds to initially delay a redelivery
        /// </summary>
        /// <value>The initial redelivery delay.</value>
        Int32 InitialRedeliveryDelay { get; set; }

        /// <summary>
        ///     Gets or sets the maximum redeliveries.  A value less than zero indicates
        ///     that there is no maximum and the NMS provider should retry forever.
        /// </summary>
        /// <value>The maximum redeliveries.</value>
        Int32 MaximumRedeliveries { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [use exponential back off].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [use exponential back off]; otherwise, <c>false</c>.
        /// </value>
        Boolean UseExponentialBackOff { get; set; }

        /// <summary>
        ///     Gets or sets the back off multiplier.
        /// </summary>
        /// <value>The back off multiplier.</value>
        Int32 BackOffMultiplier { get; set; }

        #endregion

        /// <summary>
        ///     The time in milliseconds to delay a redelivery
        /// </summary>
        /// <param name="redeliveredCounter">The redelivered counter.</param>
        /// <returns></returns>
        Int32 RedeliveryDelay( Int32 redeliveredCounter );
    }
}