using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace TuesPechkin
{
    public class StandardConverter : MarshalByRefObject, IConverter
    {
        protected IToolset Toolset { get; private set; }

        protected IDocument ProcessingDocument { get; private set; }

        public StandardConverter(IToolset toolset)
        {
            Toolset = toolset ?? throw new ArgumentNullException(nameof(toolset));

            Tracer.Trace("Created StandardConverter");
        }

        public event EventHandler<BeginEventArgs> Begin;

        public event EventHandler<ErrorEventArgs> Error;

        public event EventHandler<FinishEventArgs> Finish;

        public event EventHandler<PhaseChangeEventArgs> PhaseChange;

        public event EventHandler<ProgressChangeEventArgs> ProgressChange;

        public event EventHandler<WarningEventArgs> Warning;

        public virtual byte[] Convert(IDocument document)
        {
            Toolset.Load();

            ProcessingDocument = document;
            var converter = CreateConverter(document);

            Tracer.Trace("Created converter");
            
            Toolset.SetErrorCallback(converter, OnError);
            Toolset.SetWarningCallback(converter, OnWarning);
            Toolset.SetPhaseChangedCallback(converter, OnPhaseChanged);
            Toolset.SetProgressChangedCallback(converter, OnProgressChanged);
            Toolset.SetFinishedCallback(converter, OnFinished);

            Tracer.Trace("Added callbacks to converter");

            // run OnBegin
            OnBegin(converter);

            byte[] result = null;

            // run conversion process
            if (!Toolset.PerformConversion(converter))
            {
                Tracer.Trace("Conversion failed, null returned");
            }
            else
            {
                // get output
                result = Toolset.GetConverterResult(converter);
            }

            Tracer.Trace("Releasing unmanaged converter");
            Toolset.DestroyConverter(converter);
            ProcessingDocument = null;
            return result;
        }

        private void OnBegin(IntPtr converter)
        {
            int expectedPhaseCount = Toolset.GetPhaseCount(converter);

            Tracer.Trace("Conversion started, {1} phases awaiting");

            try
            {
                if (Begin != null)
                {
                    var args = new BeginEventArgs
                    {
                        Document = ProcessingDocument,
                        ExpectedPhaseCount = expectedPhaseCount
                    };

                    Begin(this, args);
                }
            }
            catch (Exception e)
            {
                Tracer.Warn("Exception in Begin event handler", e);
            }
        }

        private void OnError(IntPtr converter, string errorText)
        {
            Tracer.Warn("Conversion Error: "+ errorText);

            try
            {
                if (Error != null)
                {
                    var args = new ErrorEventArgs
                    {
                        Document = ProcessingDocument,
                        ErrorMessage = errorText
                    };

                    Error(this, args);
                }
            }
            catch (Exception e)
            {
                Tracer.Warn("Exception in Error event handler");
            }
        }

        private void OnFinished(IntPtr converter, int success)
        {
            Tracer.Trace("Conversion Finished: " + (success != 0 ? "Succeeded" : "Failed"));

            try
            {
                if (Finish != null)
                {
                    var args = new FinishEventArgs
                    {
                        Document = ProcessingDocument,
                        Success = success != 0
                    };

                    Finish(this, args);
                }
            }
            catch (Exception e)
            {
                Tracer.Warn("Exception in Finish event handler");
            }
        }

        private void OnPhaseChanged(IntPtr converter)
        {
            int phaseNumber = Toolset.GetPhaseNumber(converter);
            string phaseDescription = Toolset.GetPhaseDescription(converter, phaseNumber);
            Tracer.Trace($"Conversion Phase Changed: #{ phaseNumber } { phaseDescription }");

            try
            {
                if (PhaseChange != null)
                {
                    var args = new PhaseChangeEventArgs
                    {
                        Document = ProcessingDocument,
                        PhaseNumber = phaseNumber,
                        PhaseDescription = phaseDescription
                    };

                    PhaseChange(this, args);
                }
            }
            catch (Exception e)
            {
                Tracer.Warn("Exception in PhaseChange event handler");
            }
        }

        private void OnProgressChanged(IntPtr converter, int progress)
        {
            string progressDescription = Toolset.GetProgressDescription(converter);

            Tracer.Trace($"Conversion Progress Changed: ({ progress }) { progressDescription }");

            try
            {
                var args = new ProgressChangeEventArgs
                {
                    Document = ProcessingDocument,
                    Progress = progress,
                    ProgressDescription = progressDescription
                };

                ProgressChange?.Invoke(this, args);
            }
            catch (Exception e)
            {
                Tracer.Warn("Exception in Progress event handler");
            }
        }

        private void OnWarning(IntPtr converter, string warningText)
        {
            Tracer.Warn("Conversion Warning: " + warningText);

            try
            {
                if (Warning != null)
                {
                    var args = new WarningEventArgs
                    {
                        Document = ProcessingDocument,
                        WarningMessage = warningText
                    };

                    Warning(this, args);
                }
            }
            catch (Exception e)
            {
                Tracer.Warn("Exception in Warning event handler");
            }
        }

        private IntPtr CreateConverter(IDocument document)
        {
            var converter = IntPtr.Zero;

            {
                IntPtr config = Toolset.CreateGlobalSettings();

                ApplySettingsToConfig(config, document, true);

                converter = Toolset.CreateConverter(config);
            }

            foreach (IObject setting in document.GetObjects())
            {
                if (setting != null)
                {
                    IntPtr config = Toolset.CreateObjectSettings();

                    ApplySettingsToConfig(config, setting, false);

                    Toolset.AddObject(converter, config, setting.GetHtmlInputBinary());
                }
            }

            return converter;
        }

        private void ApplySettingsToConfig(IntPtr config, ISettings settings, bool isGlobal)
        {
            if (settings == null)
            {
                return;
            }

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var property in settings.GetType().GetProperties(bindingFlags))
            {
                object[] attributes = property.GetCustomAttributes(true);
                object rawValue = property.GetValue(settings, null);

                if (rawValue == null)
                {
                    continue;
                }
                else if (attributes.Length > 0 && attributes[0] is WkhtmltoxSettingAttribute attribute)
                {
                    Apply(config, attribute.Name, rawValue, isGlobal);
                }
                else if (rawValue is ISettings iSettings)
                {
                    ApplySettingsToConfig(config, iSettings, isGlobal);
                }
            }
        }
        
        private void Apply(IntPtr config, string name, object value, bool isGlobal)
        {
            var type = value.GetType();
            var apply = isGlobal 
                ? (Func<string, string, int>)((k, v) => Toolset.SetGlobalSetting(config, k, v))
                : (Func<string, string, int>)((k, v) => Toolset.SetObjectSetting(config, k, v));

            if (type == typeof(double))
            {
                apply(name, ((double)value).ToString("0.##", CultureInfo.InvariantCulture));
            }
            else if (type == typeof(bool))
            {
                apply(name, ((bool)value) ? "true" : "false");
            }
            else if (typeof(IEnumerable<KeyValuePair<string, string>>).IsAssignableFrom(type))
            {
                var dictionary = (IEnumerable<KeyValuePair<string, string>>)value;
                var counter = 0;

                foreach (var entry in dictionary)
                {
                    if (entry.Key == null || entry.Value == null)
                    {
                        continue;
                    }

                    apply(name + ".append", null);
                    apply(string.Format("{0}[{1}]", name, counter), entry.Key + "\n" + entry.Value);

                    counter++;
                }
            }
            else if (typeof(IEnumerable<PostItem>).IsAssignableFrom(type))
            {
                var list = (IEnumerable<PostItem>)value;
                var counter = 0;

                foreach (var item in list)
                {
                    if (string.IsNullOrEmpty(item.Name) || string.IsNullOrEmpty(item.Value))
                    {
                        continue;
                    }

                    apply(name + ".append", null);
                    apply(string.Format("{0}[{1}].name", name, counter), item.Name);
                    apply(string.Format("{0}[{1}].value", name, counter), item.Value);
                    apply(string.Format("{0}[{1}].file", name, counter), item.IsFile ? "true" : "false");

                    counter++;
                }
            }
            else
            {
                apply(name, value.ToString());
            }
        }
    }
}