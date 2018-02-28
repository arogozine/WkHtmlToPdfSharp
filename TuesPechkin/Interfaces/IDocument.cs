using System.Collections.Generic;

namespace TuesPechkin
{
    public interface IDocument : ISettings
    {
        IEnumerable<IObject> GetObjects();
    }
}
