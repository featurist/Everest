namespace Everest.Content
{
    public interface BodyContent
    {
        string AsString();
        string MediaType { get; }
    }
}