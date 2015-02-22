using System;

namespace Peanuts
{
    /// <summary>
    /// The abstract base class for all data objects using this library.
    /// </summary>
    public abstract class Component : ICloneable
    {
        /// <summary>
        /// This is implemented as a shallow copy.
        /// </summary>
        /// <returns>A copy of the current object</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}