namespace Peanuts
{
    public interface IProcess
    {
        void ChangeVendor(Vendor vendor);
        void OnChangeBagMix(Bag bag);
        void Update(long gameTick, object context = null);
    }
}