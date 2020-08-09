using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MinecraftServerWrapper
{
    public class ConsoleContent : INotifyPropertyChanged
    {
        private string consoleInput = string.Empty;
        LimitedSizeObservableCollection<string> consoleOutput = new LimitedSizeObservableCollection<string>(1000) { };

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

        public LimitedSizeObservableCollection<string> ConsoleOutput
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

    public class LimitedSizeObservableCollection<T> : ObservableCollection<T>
    {
        public int Capacity { get; }

        public LimitedSizeObservableCollection(int capacity)
        {
            Capacity = capacity;
        }

        public new void Add(T item)
        {
            if (Count >= Capacity)
            {
                RemoveAt(0);
            }
            base.Add(item);
        }
    }
}