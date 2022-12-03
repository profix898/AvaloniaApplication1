using AvaloniaApplication1.Maui;
using Microsoft.Maui.Graphics;

namespace AvaloniaApplication1.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public IDrawable DrawMe => new DrawMe();
    }
}