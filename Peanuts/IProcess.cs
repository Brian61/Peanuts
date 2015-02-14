namespace Peanuts
{
    /// <summary>
    /// The IProcess interface must be implemented by Processes (aka Systems) in order
    /// to work with Vendor instances and recieve notifications.
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Allows a change in Vendor instance, signals the Process to clear out its list
        /// of valid Bag identifiers and prepare for notification of new ones.
        /// </summary>
        /// <param name="vendor">A Vendor instance, may be null.</param>
        void ChangeVendor(Vendor vendor);

        /// <summary>
        /// Called by Vendor instances when a Bag has a change to its Nut subtypes.
        /// </summary>
        /// <param name="id">The contextually unique integer identifier for the Bag instance.</param>
        /// <param name="lockMix">A Mix instance describing the set of Nut subtypes in the Bag.</param>
        void OnChangeBagMix(int id, Mix lockMix);

        /// <summary>
        /// Intended for use by users to run a process.
        /// </summary>
        /// <param name="gameTick">a user supplied long integer value.</param>
        /// <param name="context">an optional user supplied object.</param>
        void Update(long gameTick, object context = null);
    }
}