public sealed class RegressionTestFailedException : Exception
{
    public RegressionTestFailedException() 
        : this("Regresssion testing failed dueo to unknown cause.")
    {
    }

    public RegressionTestFailedException(string message)
        : base(message)
    {
    }

    public RegressionTestFailedException(string message, Exception innerException)
        : base(message, innerException)
    {       
    }
}