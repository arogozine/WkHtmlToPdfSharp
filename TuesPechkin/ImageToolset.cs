﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace TuesPechkin
{
    public sealed class ImageToolset : MarshalByRefObject, IToolset
    {
        public event EventHandler Unloaded;

        public IDeployment Deployment { get; private set; }

        public bool Loaded { get; private set; }

        public ImageToolset()
        {
        }

        public ImageToolset(IDeployment deployment)
        {
            Deployment = deployment ?? throw new ArgumentNullException(nameof(deployment));
        }

        public void Load(IDeployment deployment = null)
        {
            if (Loaded)
            {
                return;
            }

            if (deployment != null)
            {
                Deployment = deployment;
            }

            WinApiHelper.SetDllDirectory(Deployment.Path);
            WkhtmltoxBindings.wkhtmltoimage_init(0);

            Loaded = true;
        }

        public void Unload()
        {
            if (Loaded)
            {
                WkhtmltoxBindings.wkhtmltoimage_deinit();

                Unloaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region Rest of IToolset stuff
        public IntPtr CreateGlobalSettings()
        {
            Tracer.Trace("Creating global settings (wkhtmltoimage_create_global_settings)");

            return WkhtmltoxBindings.wkhtmltoimage_create_global_settings();
        }

        public IntPtr CreateObjectSettings()
        {
            throw new NotSupportedException();
        }

        public int SetGlobalSetting(IntPtr setting, string name, string value)
        {
            Tracer.Trace($"Setting global setting '{ name }' to '{ value }' for config { setting }");

            var success = WkhtmltoxBindings.wkhtmltoimage_set_global_setting(setting, name, value);

            Tracer.Trace(String.Format("...setting was {0}", success == 1 ? "successful" : "not successful"));

            return success;
        }

        public unsafe string GetGlobalSetting(IntPtr setting, string name)
        {
            Tracer.Trace("Getting global setting (wkhtmltoimage_get_global_setting)");

            byte[] buf = new byte[2048];

            fixed (byte* p = buf)
            {
                WkhtmltoxBindings.wkhtmltoimage_get_global_setting(setting, name, p, buf.Length);
            }

            int walk = 0;

            while (walk < buf.Length && buf[walk] != 0)
            {
                walk++;
            }

            return Encoding.UTF8.GetString(buf, 0, walk);
        }

        public int SetObjectSetting(IntPtr setting, string name, string value)
        {
            throw new NotSupportedException();
        }

        public unsafe string GetObjectSetting(IntPtr setting, string name)
        {
            throw new NotSupportedException();
        }

        public IntPtr CreateConverter(IntPtr globalSettings)
        {
            Tracer.Trace("Creating converter (wkhtmltoimage_create_converter)");

            return WkhtmltoxBindings.wkhtmltoimage_create_converter(globalSettings, null);
        }

        public void DestroyConverter(IntPtr converter)
        {
            Tracer.Trace("Destroying converter (wkhtmltoimage_destroy_converter)");

            WkhtmltoxBindings.wkhtmltoimage_destroy_converter(converter);

            pinnedCallbacks.Unregister(converter);
        }

        public void SetWarningCallback(IntPtr converter, StringCallback callback)
        {
            Tracer.Trace("Setting warning callback (wkhtmltoimage_set_warning_callback)");
            
            WkhtmltoxBindings.wkhtmltoimage_set_warning_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public void SetErrorCallback(IntPtr converter, StringCallback callback)
        {
            Tracer.Trace("Setting error callback (wkhtmltoimage_set_error_callback)");
            
            WkhtmltoxBindings.wkhtmltoimage_set_error_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public void SetFinishedCallback(IntPtr converter, IntCallback callback)
        {
            Tracer.Trace("Setting finished callback (wkhtmltoimage_set_finished_callback)");

            WkhtmltoxBindings.wkhtmltoimage_set_finished_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public void SetPhaseChangedCallback(IntPtr converter, VoidCallback callback)
        {
            Tracer.Trace("Setting phase change callback (wkhtmltoimage_set_phase_changed_callback)");

            WkhtmltoxBindings.wkhtmltoimage_set_phase_changed_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public void SetProgressChangedCallback(IntPtr converter, IntCallback callback)
        {
            Tracer.Trace("Setting progress change callback (wkhtmltoimage_set_progress_changed_callback)");

            WkhtmltoxBindings.wkhtmltoimage_set_progress_changed_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public bool PerformConversion(IntPtr converter)
        {
            Tracer.Trace("Starting conversion (wkhtmltoimage_convert)");

            return WkhtmltoxBindings.wkhtmltoimage_convert(converter) != 0;
        }

        public void AddObject(IntPtr converter, IntPtr objectConfig, string html)
        {
            throw new NotSupportedException();
        }

        public void AddObject(IntPtr converter, IntPtr objectConfig, byte[] html)
        {
            throw new NotSupportedException();
        }

        public int GetPhaseNumber(IntPtr converter)
        {
            Tracer.Trace("Requesting current phase (wkhtmltoimage_current_phase)");

            return WkhtmltoxBindings.wkhtmltoimage_current_phase(converter);
        }

        public int GetPhaseCount(IntPtr converter)
        {
            Tracer.Trace("Requesting phase count (wkhtmltoimage_phase_count)");

            return WkhtmltoxBindings.wkhtmltoimage_phase_count(converter);
        }

        public string GetPhaseDescription(IntPtr converter, int phase)
        {
            Tracer.Trace("Requesting phase description (wkhtmltoimage_phase_description)");

            return Marshal.PtrToStringAnsi(WkhtmltoxBindings.wkhtmltoimage_phase_description(converter, phase));
        }

        public string GetProgressDescription(IntPtr converter)
        {
            Tracer.Trace("Requesting progress string (wkhtmltoimage_progress_string)");

            return Marshal.PtrToStringAnsi(WkhtmltoxBindings.wkhtmltoimage_progress_string(converter));
        }

        public int GetHttpErrorCode(IntPtr converter)
        {
            Tracer.Trace("Requesting http error code (wkhtmltoimage_http_error_code)");

            return WkhtmltoxBindings.wkhtmltoimage_http_error_code(converter);
        }

        public byte[] GetConverterResult(IntPtr converter)
        {
            Tracer.Trace("Requesting converter result (wkhtmltoimage_get_output)");

            var len = WkhtmltoxBindings.wkhtmltoimage_get_output(converter, out IntPtr tmp);
            var output = new byte[len];
            Marshal.Copy(tmp, output, 0, output.Length);
            return output;
        }

        public unsafe string GetVersion()
        {
            Tracer.Trace("Getting WkHtmlToPdf version (" +
                nameof(WkhtmltoxBindings.wkhtmltoimage_version) +
                ")");

            IntPtr versionPtr = WkhtmltoxBindings.wkhtmltoimage_version();

            byte* bytes = (byte*)versionPtr;
            int size = 0;
            while (bytes[size] != 0)
            {
                ++size;
            }
            byte[] buffer = new byte[size];
            Marshal.Copy(versionPtr, buffer, 0, size);

            return Encoding.UTF8.GetString(buffer);
        }

        #endregion

        private readonly DelegateRegistry pinnedCallbacks = new DelegateRegistry();
    }
}
