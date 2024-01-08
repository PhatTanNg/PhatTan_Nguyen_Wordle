namespace PhatTan_Nguyen_Wordle
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new InstructionsPage()); // Set InstructionsPage as the initial page
        }

    }
}