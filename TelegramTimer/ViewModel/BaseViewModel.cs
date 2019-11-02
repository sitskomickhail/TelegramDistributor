using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TelegramTimer.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void AddRule(Func<string> p1, Func<bool> p2, string v)
        {
            throw new NotImplementedException();
        }
    }
}
