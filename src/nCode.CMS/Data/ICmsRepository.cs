using System;
namespace nCode.CMS.Data
{
    public interface ICmsRepository : IDisposable
    {
        ContentPage GetContentPage(Guid id);

        void ConvertContentPage(ContentPage contentPage);
    }
}
