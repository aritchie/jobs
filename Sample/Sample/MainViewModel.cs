using System;
using System.Windows.Input;
using ReactiveUI;


namespace Sample
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            this.RunTask = ReactiveCommand.Create(() =>
            {

            });
        }


        public ICommand RunTask { get; }

    }
}
