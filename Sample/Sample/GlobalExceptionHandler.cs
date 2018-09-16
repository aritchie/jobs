using System;
using Acr.UserDialogs;
using ReactiveUI;


namespace Sample
{
    public class GlobalExceptionHandler : IObserver<Exception>
    {
        public static void Register()
        {
            RxApp.DefaultExceptionHandler = new GlobalExceptionHandler();
        }


        public void OnNext(Exception exception)
        {
            Console.WriteLine(exception);
            UserDialogs.Instance.Alert(exception.ToString());
        }


        public void OnCompleted()
        {
        }


        public void OnError(Exception error)
        {
        }
    }
}
