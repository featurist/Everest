namespace Everest
{
    public interface ResponseDetails
    {
        RequestDetails Request { get; }
        int Status { get; }
    }
}