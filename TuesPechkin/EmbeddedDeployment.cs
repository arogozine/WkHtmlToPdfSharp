using System;
using System.IO;

namespace TuesPechkin
{
    [Serializable]
    public abstract class EmbeddedDeployment : IDeployment
    {
        public virtual string Path
        {
            get
            {
                var path = System.IO.Path.Combine(physical.Path, PathModifier ?? string.Empty);

                if (!deployed)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var (name, stream) = GetContents();
                    
                    var filename = System.IO.Path.Combine(path, name);

                    if (!File.Exists(filename))
                    {
                        WriteStreamToFile(filename, stream);
                    }
                    

                    deployed = true;
                }

                // In 2.2.0 needs to return path
                return physical.Path;
            }
        }

        // Embedded deployments need to override this instead in 2.2.0
        protected virtual string PathModifier
        {
            get
            {
                return this.GetType().Assembly.GetName().Version.ToString();
            }
        }

        public EmbeddedDeployment(IDeployment physical)
        {
            this.physical = physical ?? throw new ArgumentException(nameof(physical));
        }

        protected IDeployment physical;

        protected abstract (string, Stream) GetContents();

        private bool deployed;

        private void WriteStreamToFile(string fileName, Stream stream)
        {
            if (!File.Exists(fileName))
            {
                var writeBuffer = new byte[8192];
                var writeLength = 0;

                using (var newFile = File.Open(fileName, FileMode.Create))
                {
                    while ((writeLength = stream.Read(writeBuffer, 0, writeBuffer.Length)) > 0)
                    {
                        newFile.Write(writeBuffer, 0, writeLength);
                    }
                }
            }
        }
    }
}
