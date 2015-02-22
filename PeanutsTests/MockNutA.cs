namespace Peanuts.Tests
{
    public sealed class MockNutA : Nut
    {
        public string SomeText { get; set; }
    }

    public sealed class MockNutB : Nut
    {
        public float SomeFloat { get; set; }
    }

    public sealed class MockNutC : Nut
    {
        public int SomeInt { get; set; }
    }

}