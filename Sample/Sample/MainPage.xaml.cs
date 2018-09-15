using System;
using Xamarin.Forms;


namespace Sample
{
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            (this.BindingContext as MainViewModel)?.OnAppearing();
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            (this.BindingContext as MainViewModel)?.OnDisappearing();
        }
    }
}
