    public interfeace IClockService : IDisposable
    {
        RtcController Controller { get; private set; }
        void SetTime(DateTime value)
        void Dispose()
    }