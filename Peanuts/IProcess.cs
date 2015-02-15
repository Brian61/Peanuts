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
        /// <param name="bag">The Bag instance that changed.</param>
        void OnChangeBagMix(Bag bag);
    }
}
