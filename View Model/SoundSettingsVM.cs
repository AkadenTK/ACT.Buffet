using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buffet
{
    public class SoundSettingsVM : ViewModelBase
    {
        private ViewModelWindowManager _WindowManager;
        [YamlDotNet.Serialization.YamlIgnore]
        public ViewModelWindowManager WindowManager
        {
            get { return _WindowManager; }
            set { if(SetProperty(ref _WindowManager, value)) BrowseForBuffEndedCommand.InvalidateCanExecute(); }
        }

        private string _BuffName;
        public string BuffName
        {
            get { return _BuffName; }
            set { SetProperty(ref _BuffName, value); }
        }

        private string _BuffGrantedPath;
        public string BuffGrantedPath
        {
            get { return _BuffGrantedPath; }
            set { if (SetProperty(ref _BuffGrantedPath, value)) ClearBuffGrantedCommand.InvalidateCanExecute(); }
        }

        private int _BuffGrantedVolume = 100;
        public int BuffGrantedVolume
        {
            get { return _BuffGrantedVolume; }
            set { if (SetProperty(ref _BuffGrantedVolume, value)) ClearBuffGrantedCommand.InvalidateCanExecute(); }
        }

        private string _BuffEndedPath;
        public string BuffEndedPath
        {
            get { return _BuffEndedPath; }
            set { if (SetProperty(ref _BuffEndedPath, value)) ClearBuffEndedCommand.InvalidateCanExecute(); }
        }

        private int _BuffEndedVolume = 100;
        public int BuffEndedVolume
        {
            get { return _BuffEndedVolume; }
            set { if (SetProperty(ref _BuffEndedVolume, value)) ClearBuffEndedCommand.InvalidateCanExecute(); }
        }

        private bool _PlayBuffGrantedSound;
        public bool PlayBuffGrantedSound
        {
            get { return _PlayBuffGrantedSound; }
            set { SetProperty(ref _PlayBuffGrantedSound, value); }
        }
        private bool _PlayBuffEndedSound;
        public bool PlayBuffEndedSound
        {
            get { return _PlayBuffEndedSound; }
            set { SetProperty(ref _PlayBuffEndedSound, value); }
        }

        #region BrowseForBuffGrantedCommand

        [YamlDotNet.Serialization.YamlIgnore]
        public RelayCommand BrowseForBuffGrantedCommand
        { get; private set; }

        private bool CanExecute_BrowseForBuffGrantedCommand(object obj)
        {
            return WindowManager != null;
        }

        private void Execute_BrowseForBuffGrantedCommand(object obj)
        {
            var newPath = WindowManager.RequestOpenFile("", "Sound files (*.wav, *.mp3)|*.wav;*.mp3|Wave files (*.wav)|*.wav|MP3 files (*.mp3)|*.mp3", false).FirstOrDefault();
            if (!string.IsNullOrEmpty(newPath)) BuffGrantedPath = newPath;
        }

        #endregion

        #region BrowseForBuffEndedCommand

        [YamlDotNet.Serialization.YamlIgnore]
        public RelayCommand BrowseForBuffEndedCommand
        { get; private set; }

        private bool CanExecute_BrowseForBuffEndedCommand(object obj)
        {
            return WindowManager != null;
        }

        private void Execute_BrowseForBuffEndedCommand(object obj)
        {
            var newPath = WindowManager.RequestOpenFile("", "Wave files (*.wav)|*.wav|MP3 files (*.mp3)|*.mp3|All supported (*.wav, *.mp3)|*.wav;*.mp3", false).FirstOrDefault();
            if (!string.IsNullOrEmpty(newPath)) BuffEndedPath = newPath;
        }

        #endregion

        #region ClearBuffGrantedCommand

        [YamlDotNet.Serialization.YamlIgnore]
        public RelayCommand ClearBuffGrantedCommand
        { get; private set; }

        private bool CanExecute_ClearBuffGrantedCommand(object obj)
        {
            return !string.IsNullOrEmpty(BuffGrantedPath);
        }

        private void Execute_ClearBuffGrantedCommand(object obj)
        {
            BuffGrantedPath = "";
        }

        #endregion

        #region ClearBuffEndedCommand

        [YamlDotNet.Serialization.YamlIgnore]
        public RelayCommand ClearBuffEndedCommand
        { get; private set; }

        private bool CanExecute_ClearBuffEndedCommand(object obj)
        {
            return !string.IsNullOrEmpty(BuffEndedPath);
        }

        private void Execute_ClearBuffEndedCommand(object obj)
        {
            BuffEndedPath = "";
        }

        #endregion

        public SoundSettingsVM()
        {
            BrowseForBuffGrantedCommand = new RelayCommand(Execute_BrowseForBuffGrantedCommand, CanExecute_BrowseForBuffGrantedCommand);
            BrowseForBuffEndedCommand = new RelayCommand(Execute_BrowseForBuffEndedCommand, CanExecute_BrowseForBuffEndedCommand);
            ClearBuffGrantedCommand = new RelayCommand(Execute_ClearBuffGrantedCommand, CanExecute_ClearBuffGrantedCommand);
            ClearBuffEndedCommand = new RelayCommand(Execute_ClearBuffEndedCommand, CanExecute_ClearBuffEndedCommand);
        }

        public void InitializeWindowManager(ViewModelWindowManager windowManager)
        {
            this.WindowManager = windowManager;
        }
    }
}
