using System;

namespace nCode.UI
{
    /// <summary>
    /// Indicates that this MasterPage should be listed in the Page Editor using the given name.
    /// </summary>
    public interface IMasterPage
    {
        string Name { get; }
    }
}