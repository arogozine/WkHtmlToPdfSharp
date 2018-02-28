using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TuesPechkin
{
    internal static class Tracer
    {
        private readonly static TraceSource source = new TraceSource("pechkin:default");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Trace(string message)
        {
            source.TraceInformation(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message)
        {
            source.TraceEvent(TraceEventType.Warning, 0, message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message, Exception e)
        {
            source.TraceEvent(TraceEventType.Warning, 0, string.Format(message + "{0}", e));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Critical(string message, Exception e)
        {
            source.TraceEvent(TraceEventType.Critical, 0, string.Format(message + "{0}", e));
        }
    }
}