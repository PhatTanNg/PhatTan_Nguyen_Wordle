using Microsoft.Maui.Controls;
using System;

namespace PhatTan_Nguyen_Wordle
{
    public partial class InstructionsPage : ContentPage
    {
        public InstructionsPage()
        {
            InitializeComponent();
        }


        private async void LetGoButton_Clicked(object sender, EventArgs e)
        {
            if (!IsGameInProgress())
            {
                MainPage mainPage = new MainPage();
                await Navigation.PushAsync(mainPage);
            }
            else
            {
                await Navigation.PopAsync();
            }
        }


        private bool IsGameInProgress()
        {
            // You might need to adjust this condition based on your game state logic
            return GameState.CurrentWord != null; // Assuming CurrentWord being set indicates the game is in progress
        }
    }
}
