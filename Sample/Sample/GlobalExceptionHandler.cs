using System;
using Acr.UserDialogs;
using Autofac;
using ReactiveUI;


namespace Sample
{
    public class GlobalExceptionHandler : IObserver<Exception>, IStartable
    {
        readonly IUserDialogs dialogs;
        public GlobalExceptionHandler(IUserDialogs dialogs) => this.dialogs = dialogs;


        public void OnNext(Exception exception)
        {
            Console.WriteLine(exception);
            this.dialogs.Alert(exception.ToString());
        }


        public void OnCompleted() {}
        public void OnError(Exception error) {}
        public void Start()  => RxApp.DefaultExceptionHandler = this;
    }
}
