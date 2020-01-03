namespace Tests
{
    public class BarMock : IBarDisplay
    {
        public bool FlipX { get; set; }
        public void Hide() { }
        public void FillTo(float proportion) { }
    }

}
