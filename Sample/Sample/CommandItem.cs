using System;
using System.Windows.Input;


namespace Sample
{
    public class CommandItem
    {
        public string Text { get; set; }
        public string Detail { get; set; }
        public ICommand Command { get; set; }
    }
}
