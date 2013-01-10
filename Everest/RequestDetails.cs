using System;

namespace Everest
{
    public interface RequestDetails
    {
        string Method { get; }
        Uri RequestUri { get; }
    }
}