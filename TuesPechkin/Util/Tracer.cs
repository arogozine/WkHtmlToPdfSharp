using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TuesPechkin
{
    internal static class Tracer
    {
        private readonly static TraceSource source = new TraceSource("pechkin:default");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Trace(string message)
        {
            source.TraceInformation(
                "T:" + Thread.CurrentThread.Name + " " + message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message)
        {
            source.TraceEvent(TraceEventType.Warning, 0,
                "T:" + Thread.CurrentThread.Name + " " + message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warn(string message, Exception e)
        {
            source.TraceEvent(TraceEventType.Warning, 0,
                $"T:{ Thread.CurrentThread.Name } { message } { e }");
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Critical(string message, Exception e)
        {
            source.TraceEvent(TraceEventType.Critical, 0,
                $"T:{ Thread.CurrentThread.Name } { message } { e }");
        }
    }
}