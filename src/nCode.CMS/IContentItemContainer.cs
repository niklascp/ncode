using System;
using System.Collections.Generic;

namespace nCode.CMS
{
    /// <summary>
    /// Indicates that the class can contain content items.
    /// </summary>
    public interface IContentItemContainer
    {
        Guid ID { get; }
        ContentItem[] ContentItems { get; }
    }
}