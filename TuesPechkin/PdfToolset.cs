﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace TuesPechkin
{
    public sealed class PdfToolset : MarshalByRefObject, IToolset
    {
        public event EventHandler Unloaded;

        public IDeployment Deployment { get; private set; }

        public bool Loaded { get; private set; }

        public PdfToolset()
        {
        }

        public PdfToolset(IDeployment deployment)
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
            WkhtmltoxBindings.wkhtmltopdf_init(0);

            Loaded = true;
        }

        public void Unload()
        {
            if (Loaded)
            {
                WkhtmltoxBindings.wkhtmltopdf_deinit();

                Unloaded?.Invoke(this, EventArgs.Empty);
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region Rest of IToolset stuff

        public unsafe string GetVersion()
        {
            Tracer.Trace("Getting WkHtmlToPdf version (" +
                nameof(WkhtmltoxBindings.wkhtmltopdf_version) +
                ")");

            IntPtr versionPtr = WkhtmltoxBindings.wkhtmltopdf_version();

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

        public IntPtr CreateGlobalSettings()
        {
            Tracer.Trace("Creating global settings (wkhtmltopdf_create_global_settings)");

            return WkhtmltoxBindings.wkhtmltopdf_create_global_settings();
        }

        public IntPtr CreateObjectSettings()
        {
            Tracer.Trace("Creating object settings (wkhtmltopdf_create_object_settings)");

            return WkhtmltoxBindings.wkhtmltopdf_create_object_settings();
        }

        public int SetGlobalSetting(IntPtr setting, string name, string value)
        {
            Tracer.Trace($"Setting global setting '{ name }' to '{ value }' for config { setting }");

            var success = WkhtmltoxBindings.wkhtmltopdf_set_global_setting(setting, name, value);

            Tracer.Trace(String.Format("...setting was {0}", success == 1 ? "successful" : "not successful"));

            return success;
        }

        public unsafe string GetGlobalSetting(IntPtr setting, string name)
        {
            Tracer.Trace("Getting global setting (wkhtmltopdf_get_global_setting)");

            byte[] buf = new byte[2048];

            fixed (byte* p = buf)
            {
                WkhtmltoxBindings.wkhtmltopdf_get_global_setting(setting, name, p, buf.Length);
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
            Tracer.Trace($"Setting object setting '{ name }' to '{ value }' for config { setting }");

            var success = WkhtmltoxBindings.wkhtmltopdf_set_object_setting(setting, name, value);

            Tracer.Trace(String.Format("...setting was {0}", success == 1 ? "successful" : "not successful"));

            return success;
        }

        public unsafe string GetObjectSetting(IntPtr setting, string name)
        {
            Tracer.Trace($"Getting object setting '{ name }' for config { setting }");

            byte[] buf = new byte[2048];

            fixed (byte* p = buf)
            {
                WkhtmltoxBindings.wkhtmltopdf_get_object_setting(setting, name, p, buf.Length);
            }

            int walk = 0;

            while (walk < buf.Length && buf[walk] != 0)
            {
                walk++;
            }

            return Encoding.UTF8.GetString(buf, 0, walk);
        }

        public IntPtr CreateConverter(IntPtr globalSettings)
        {
            Tracer.Trace("Creating converter (wkhtmltopdf_create_converter)");

            return WkhtmltoxBindings.wkhtmltopdf_create_converter(globalSettings);
        }

        public void DestroyConverter(IntPtr converter)
        {
            Tracer.Trace("Destroying converter (wkhtmltopdf_destroy_converter)");

            WkhtmltoxBindings.wkhtmltopdf_destroy_converter(converter);

            pinnedCallbacks.Unregister(converter);
        }

        public void SetWarningCallback(IntPtr converter, StringCallback callback)
        {
            Tracer.Trace("Setting warning callback (wkhtmltopdf_set_warning_callback)");
            
            WkhtmltoxBindings.wkhtmltopdf_set_warning_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public void SetErrorCallback(IntPtr converter, StringCallback callback)
        {
            Tracer.Trace("Setting error callback (wkhtmltopdf_set_error_callback)");
            
            WkhtmltoxBindings.wkhtmltopdf_set_error_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public void SetFinishedCallback(IntPtr converter, IntCallback callback)
        {
            Tracer.Trace("Setting finished callback (wkhtmltopdf_set_finished_callback)");

            WkhtmltoxBindings.wkhtmltopdf_set_finished_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public void SetPhaseChangedCallback(IntPtr converter, VoidCallback callback)
        {
            Tracer.Trace("Setting phase change callback (wkhtmltopdf_set_phase_changed_callback)");

            WkhtmltoxBindings.wkhtmltopdf_set_phase_changed_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public void SetProgressChangedCallback(IntPtr converter, IntCallback callback)
        {
            Tracer.Trace("Setting progress change callback (wkhtmltopdf_set_progress_changed_callback)");

            WkhtmltoxBindings.wkhtmltopdf_set_progress_changed_callback(converter, callback);

            pinnedCallbacks.Register(converter, callback);
        }

        public bool PerformConversion(IntPtr converter)
        {
            Tracer.Trace("Starting conversion (wkhtmltopdf_convert)");

            return WkhtmltoxBindings.wkhtmltopdf_convert(converter) != 0;
        }

        public void AddObject(IntPtr converter, IntPtr objectConfig, string html)
        {
            Tracer.Trace("Adding string object (wkhtmltopdf_add_object)");

            WkhtmltoxBindings.wkhtmltopdf_add_object(converter, objectConfig, html);
        }

        public void AddObject(IntPtr converter, IntPtr objectConfig, byte[] html)
        {
            Tracer.Trace("Adding byte[] object (wkhtmltopdf_add_object)");

            WkhtmltoxBindings.wkhtmltopdf_add_object(converter, objectConfig, html);
        }

        public int GetPhaseNumber(IntPtr converter)
        {
            Tracer.Trace("Requesting current phase (wkhtmltopdf_current_phase)");

            return WkhtmltoxBindings.wkhtmltopdf_current_phase(converter);
        }

        public int GetPhaseCount(IntPtr converter)
        {
            Tracer.Trace("Requesting phase count (wkhtmltopdf_phase_count)");

            return WkhtmltoxBindings.wkhtmltopdf_phase_count(converter);
        }

        public string GetPhaseDescription(IntPtr converter, int phase)
        {
            Tracer.Trace("Requesting phase description (wkhtmltopdf_phase_description)");

            return Marshal.PtrToStringAnsi(WkhtmltoxBindings.wkhtmltopdf_phase_description(converter, phase));
        }

        public string GetProgressDescription(IntPtr converter)
        {
            Tracer.Trace("Requesting progress string (wkhtmltopdf_progress_string)");

            return Marshal.PtrToStringAnsi(WkhtmltoxBindings.wkhtmltopdf_progress_string(converter));
        }

        public int GetHttpErrorCode(IntPtr converter)
        {
            Tracer.Trace("Requesting http error code (wkhtmltopdf_http_error_code)");

            return WkhtmltoxBindings.wkhtmltopdf_http_error_code(converter);
        }

        public byte[] GetConverterResult(IntPtr converter)
        {
            Tracer.Trace("Requesting converter result (wkhtmltopdf_get_output)");

            var len = WkhtmltoxBindings.wkhtmltopdf_get_output(converter, out IntPtr tmp);
            var output = new byte[len];
            Marshal.Copy(tmp, output, 0, output.Length);
            return output;
        }
        #endregion

        private readonly DelegateRegistry pinnedCallbacks = new DelegateRegistry();
    }
}
