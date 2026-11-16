namespace Domain.Ports.Driving
{
    // A pause or speed change the game clock holds until its owner releases it,
    // until its length runs out, or until the run ends. Releasing twice does nothing.
    public interface ITimeHold
    {
        void Release();
    }
}
