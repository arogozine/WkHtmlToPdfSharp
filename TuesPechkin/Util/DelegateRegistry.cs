using System;
using System.Collections.Generic;

namespace TuesPechkin
{
    internal class DelegateRegistry
    {
        private readonly Dictionary<IntPtr, List<Delegate>> registry = new Dictionary<IntPtr, List<Delegate>>();

        public void Register(IntPtr converter, Delegate callback)
        {
            if (!registry.TryGetValue(converter, out List<Delegate> delegates))
            {
                delegates = new List<Delegate>();
                registry.Add(converter, delegates);
            }

            delegates.Add(callback);
        }

        public void Unregister(IntPtr converter)
        {
            registry.Remove(converter);
        }
    }
}
