using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MinecraftServerWrapper
{
    public class ConsoleContent : INotifyPropertyChanged
    {
        private string consoleInput = string.Empty;
        ObservableCollection<string> consoleOutput = new ObservableCollection<string>() { };

        public string ConsoleInput
        {
            get
            {
                return consoleInput;
            }
            set
            {
                consoleInput = value;
                OnPropertyChanged(nameof(ConsoleInput));
            }
        }

        public ObservableCollection<string> ConsoleOutput
        {
            get
            {
                return consoleOutput;
            }
            set
            {
                consoleOutput = value;
                OnPropertyChanged(nameof(ConsoleOutput));
            }
        }

        public void RunCommand()
        {
            ConsoleOutput.Add(ConsoleInput);
            ConsoleInput = string.Empty;
        }

        public void AddOutput(string data)
        {
            ConsoleOutput.Add(data);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}