using System;

namespace TeileListe.Common.Interface
{
    public interface ITeileListeInterface : IDisposable
    {
        string InterfaceVersion { get; }
    }
}
