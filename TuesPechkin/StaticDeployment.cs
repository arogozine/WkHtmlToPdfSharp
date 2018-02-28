using System;

namespace TuesPechkin
{
    [Serializable]
    public sealed class StaticDeployment : IDeployment
    {
        public string Path { get; private set; }

        public StaticDeployment(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
    }
}
