namespace Peanuts.Tests
{
    public sealed class MockNutA : ShallowNutAbc
    {
        public string SomeText { get; set; }
    }

    public sealed class MockNutB : DeepNutAbc
    {
        public float SomeFloat { get; set; }
    }

    public sealed class MockNutC : ShallowNutAbc
    {
    }

}