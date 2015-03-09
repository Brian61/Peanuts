namespace Peanuts.Tests
{
    public sealed class MockComponentA : Component
    {
        public string SomeText { get; set; }
    }

    public sealed class MockComponentB : Component
    {
        public float SomeFloat { get; set; }
    }

    public sealed class MockComponentC : Component
    {
        public int SomeInt { get; set; }
    }
}