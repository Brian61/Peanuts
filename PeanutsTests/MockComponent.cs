namespace Peanuts.Tests
{
    public sealed class MockEntityA : Component
    {
        public string SomeText { get; set; }
    }

    public sealed class MockEntityB : Component
    {
        public float SomeFloat { get; set; }
    }

    public sealed class MockNutC : Component
    {
        public int SomeInt { get; set; }
    }
}