using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using JuliusSweetland.ETTA.Enums;
using JuliusSweetland.ETTA.Properties;
using log4net;
using Microsoft.Practices.Prism.Mvvm;

namespace JuliusSweetland.ETTA.UI.ViewModels.Management
{
    public class VisualsViewModel : BindableBase
    {
        #region Private Member Vars

        private const string RobotoUrl = "/Resources/Fonts/#Roboto";

        private readonly static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion
        
        #region Ctor

        public VisualsViewModel()
        {
            LoadSettings();
        }
        
        #endregion
        
        #region Properties

        public List<KeyValuePair<string, string>> Themes
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Android Dark", "/Resources/Themes/Android_Dark.xaml"),
                    new KeyValuePair<string, string>("Android Light", "/Resources/Themes/Android_Light.xaml")
                };
            }
        }

        public List<KeyValuePair<string, string>> FontFamilies
        {
            get
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Roboto", "/Resources/Fonts/#Roboto")
                };
            }
        }

        public List<string> FontStretches
        {
            get
            {
                switch (FontFamily)
                {
                    case RobotoUrl:
                        return new List<string> { "Normal", "Condensed" };
                }

                return null;
            }
        }

        public List<string> FontWeights
        {
            get
            {
                switch (FontFamily)
                {
                    case RobotoUrl:
                        switch (FontStretch)
                        {
                            case "Normal":
                                return new List<string> { "Thin", "Light", "Regular", "Medium", "Bold", "Black" };

                            case "Condensed":
                                return new List<string> { "Light", "Regular", "Bold" };
                        }
                        break;
                }

                return null;
            }
        }

        public List<KeyValuePair<string, VisualModes>> VisualModes
        {
            get
            {
                return new List<KeyValuePair<string, VisualModes>>
                {
                    new KeyValuePair<string, VisualModes>("Standard", Enums.VisualModes.Standard),
                    new KeyValuePair<string, VisualModes>("Speech Only", Enums.VisualModes.SpeechOnly)
                };
            }
        }

        private string theme;
        public string Theme
        {
            get { return theme; }
            set { SetProperty(ref theme, value); }
        }

        private string fontFamily;
        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                SetProperty(ref fontFamily, value);
                OnPropertyChanged(() => FontStretches);
                OnPropertyChanged(() => FontWeights);
            }
        }

        private string fontStretch;
        public string FontStretch
        {
            get { return fontStretch; }
            set
            {
                SetProperty(ref fontStretch, value);
                OnPropertyChanged(() => FontWeights);
            }
        }

        private string fontWeight;
        public string FontWeight
        {
            get { return fontWeight; }
            set { SetProperty(ref fontWeight, value); }
        }

        private int scratchpadNumberOfLines;
        public int ScratchpadNumberOfLines
        {
            get { return scratchpadNumberOfLines; }
            set { SetProperty(ref scratchpadNumberOfLines, value); }
        }

        private VisualModes visualMode;
        public VisualModes VisualMode
        {
            get { return visualMode; }
            set { SetProperty(ref visualMode, value); }
        }

        public bool ChangesRequireRestart
        {
            get { return false; }
        }
        
        #endregion
        
        #region Methods

        private void LoadSettings()
        {
            Theme = Settings.Default.Theme;
            FontFamily = Settings.Default.FontFamily;
            FontStretch = Settings.Default.FontStretch;
            FontWeight = Settings.Default.FontWeight;
            ScratchpadNumberOfLines = Settings.Default.ScratchpadNumberOfLines;
            VisualMode = Settings.Default.VisualMode;
        }

        public void ApplyChanges()
        {
            Settings.Default.Theme = Theme;
            Settings.Default.FontFamily = FontFamily;
            Settings.Default.FontStretch = FontStretch;
            Settings.Default.FontWeight = fontWeight;
            Settings.Default.ScratchpadNumberOfLines = ScratchpadNumberOfLines;
            Settings.Default.VisualMode = VisualMode;
            Settings.Default.Save();
        }

        #endregion
    }
}