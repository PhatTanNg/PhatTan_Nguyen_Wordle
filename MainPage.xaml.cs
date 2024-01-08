using System.Text;

namespace PhatTan_Nguyen_Wordle
{
    public partial class MainPage : ContentPage
    {
        private List<LeaderboardItem> leaderboardData = new List<LeaderboardItem>();
        private List<string> wordList = new List<string>();
        private int currentRow = 0;
        private bool wordSelected = false;
        private string userGuess = string.Empty;
        private string currentWord;
        private const int MaxGuesses = 6;
        private int guessesMade = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private bool isTimerRunning = false;
        private string playerName;
       
        public MainPage()
        {
            InitializeComponent();
            FetchAndSaveWordList();
           
        }

        private async void FetchAndSaveWordList()
        {
            try
            {
                // Attempt to fetch the word list
                List<string> fetchedWordList = await FetchWordListAsync();

                // Check if the fetched list is not null or empty
                if (fetchedWordList != null && fetchedWordList.Any())
                {
                    // Save the word list to a local field
                    wordList = fetchedWordList;

                
                }
               
            }
            catch (Exception ex)
            {
              
            }
        }



        private async Task<List<string>> FetchWordListAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Append a timestamp parameter to the URL to avoid caching
                    string url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt?" + DateTime.Now.Ticks;
                    string wordListString = await client.GetStringAsync(url);

                    // Split the fetched string into a list of words
                    List<string> wordList = wordListString.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    return wordList;
                }
            }
            catch (Exception ex)
            {
                
                return new List<string>();
            }
        }


        private void SaveWordListLocally(string wordList)
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "words.txt");

                // Save the word list to a local file
                File.WriteAllText(path, wordList);
            }
            catch (Exception ex)
            {
               
            }
        }

        private List<string> ReadWordListFromLocalFile()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "words.txt");

                // Read the word list from the local file
                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);
                    return lines.ToList();
                }
                else
                {
                  
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
               
              
                return new List<string>();
            }
        }


        //pick a random word when start or renew game
        private string GetRandomWord()
        {
           // if (wordList != null && wordList.Any())
           // {
                Random random = new Random();
                return wordList[random.Next(0, wordList.Count)];
            //}

            // Handle the case where wordList is null or empty
            //return string.Empty;
        }


        private void startGame(object sender, EventArgs e)
        {
            currentRow = 0;
            guessesMade = 0;
            elapsedTime = TimeSpan.Zero;
            isTimerRunning = true;
            currentWord = GetRandomWord();
           GameState.CurrentWord = GetRandomWord();
            ClearGuessesAndFeedback();
                SetEntryBoxColors(Colors.LightGray, Colors.Black); // Set initial colors
                EnableEntryBoxes(true, currentRow); // Enable entry boxes for the current row

            FeedbackLabel.Text = GetPartialWord(currentWord, 2);




            Device.StartTimer(TimeSpan.FromSeconds(1), UpdateTimer);
            TimerLabel.Text = string.Empty;


        }
        

   
        private string GetPartialWord(string fullWord, int length)
        {
            if (fullWord.Length <= length)
            {
                // The word is shorter than the specified length, return the full word
                return fullWord;
            }
            else
            {
                // Get a substring of the word with the specified length
                Random random = new Random();
                int startIndex = random.Next(0, fullWord.Length - length);
                return fullWord.Substring(startIndex, length);
            }
        }



        private void EnableEntryBoxes(bool isEnabled, int row)
        {
            try
            {
                foreach (var entry in GetAllEntryBoxes())
                {
                    int entryRow = Grid.GetRow(entry);
                    entry.IsEnabled = isEnabled && entryRow == row;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        //clear the user input
        private void ClearGuessesAndFeedback()
        {
            try
            {
                // Clear the text in all the Entry boxes
                foreach (var entry in GetAllEntryBoxes())
                {
                    entry.Text = string.Empty;
                }

                // Clear the feedback label
                FeedbackLabel.Text = string.Empty;
            }
            catch (Exception ex)
            {
               
            }
        }

        private IEnumerable<Entry> GetAllEntryBoxes()
        {
            try
            {
                // Use LINQ to get all Entry boxes in the Grid
                return GuessGrid.Children
                    .Where(child => child is Entry)
                    .Cast<Entry>();
            }
            catch (Exception ex)
            {
              
                return Enumerable.Empty<Entry>();
            }
        }

        private async void submitGuess(object sender, EventArgs e)
        {
            userGuess = GetUserGuess();

           

            if (!string.IsNullOrEmpty(userGuess) && userGuess.Length == 5)
            {
                ProcessUserGuess(userGuess);
                guessesMade++;

                if (IsCurrentRowCorrect(userGuess))
                {
                    // User guessed correctly, game ends
                    FeedbackLabel.Text = "Congratulations! You guessed the word!";
                    EnableEntryBoxes(false, currentRow);
                    isTimerRunning = false; // Stop the timer
                                            // Disable entry boxes
                                            // Player guessed correctly, ask for their name
                    string playerName = await PromptForPlayerName();

                    // Update the leaderboard with the new player's information
                    UpdateLeaderboard(playerName, guessesMade, elapsedTime);
                }
                else if (guessesMade >= MaxGuesses)
                {
                    // Maximum number of guesses reached, end the game
                    FeedbackLabel.Text = $"Sorry! You've reached the maximum number of guesses. The word was {currentWord}.";
                    EnableEntryBoxes(false, currentRow); // Disable entry boxes
                    isTimerRunning = false; // Stop the timer
                }
                else
                {
                    // Continue the game, move to the next row
                    currentRow++;
                    EnableEntryBoxes(true, currentRow);
                }
            }
            else
            {
                FeedbackLabel.Text = $"Invalid guess: {userGuess}. Please enter a 5-letter word.";
            }

          
        }







        private bool IsCurrentRowCorrect(string userGuess)
        {
            if (currentWord == null)
            {
                // Handle the case where currentWord is not set
                FeedbackLabel.Text = "Please start a new game first.";
                return false;
            }

            return userGuess == currentWord;
        }

        private string GetUserGuess()
        {
            try
            {
                // Concatenate the text from Entry boxes of the current row to get the user's guess
                StringBuilder userGuessBuilder = new StringBuilder();

                foreach (var entry in GetAllEntryBoxes().Where(entry => Grid.GetRow(entry) == currentRow))
                {
                    userGuessBuilder.Append(entry.Text.Trim());
                }

                return userGuessBuilder.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserGuess: {ex.Message}");
                return string.Empty;
            }
        }





        private void ProcessUserGuess(string userGuess)
        {
            // Check if currentWord is set
            if (currentWord == null)
            {
                FeedbackLabel.Text = "Please start a new game first.";
                return;
            }

            // Find the Entry boxes of the current row
            var entryBoxesInCurrentRow = GetAllEntryBoxes().Where(entry => Grid.GetRow(entry) == currentRow).ToList();

            // Iterate through each Entry box in the current row
            for (int i = 0; i < entryBoxesInCurrentRow.Count && i < userGuess.Length; i++)
            {
                char userChar = userGuess[i];
                char randomChar = currentWord[i];

                // Check if the letter is in the correct position (green)
                if (userChar == randomChar)
                {
                    SetEntryBoxColor(entryBoxesInCurrentRow[i], Colors.Green, Colors.Black); // Black text for correct position
                }
                else if (currentWord.Contains(userChar.ToString()))
                {
                    // Check if the letter is in the random word but in the wrong position (yellow)
                    SetEntryBoxColor(entryBoxesInCurrentRow[i], Colors.Yellow, Colors.Black); // Black text for wrong position
                }
                else
                {
                    // Letter is not in the random word (red)
                    SetEntryBoxColor(entryBoxesInCurrentRow[i], Colors.Red, Colors.Black); // Black text for incorrect letter
                }
            }
        }


        private Color GetColorForComparison(char userChar, char randomChar)
        {
            if (userChar == randomChar)
            {
                return Colors.Green;
            }
            else if (currentWord.Contains(userChar.ToString()))
            {
                return Colors.Yellow;
            }
            else
            {
                return Colors.Red;
            }
        }





        private void SetEntryBoxColor(Entry entryBox, Color backgroundColor, Color textColor)
        {
            try
            {
                // Set the background color and text color of the Entry box
                entryBox.BackgroundColor = backgroundColor;
                entryBox.TextColor = textColor;
            }
            catch (Exception ex)
            {
             
            }
        }


        private void SetEntryBoxColors(Color backgroundColor, Color textColor)
        {
            try
            {
                foreach (var entry in GetAllEntryBoxes())
                {
                    entry.BackgroundColor = backgroundColor;
                    entry.TextColor = textColor;
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        private bool UpdateTimer()
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));

            // Update the TimerLabel to display the elapsed time
            TimerLabel.Text = $"Time: {elapsedTime:mm\\:ss}";

            return isTimerRunning; // Return false to stop the timer
        }

        private void GoToInstructions(object sender, EventArgs e)
        {
            // Check if MainPage is already in the navigation stack
            
                Navigation.PushAsync(new InstructionsPage());
            
        }

        private async void ShowLeaderboard(object sender, EventArgs e)
        {
            // Pass the current leaderboard data to the LeaderboardPage constructor
            await Navigation.PushAsync(new LeaderboardPage(leaderboardData));
        }


        private async Task<string> PromptForPlayerName()
        {
            // You can use a custom input dialog or navigate to a new page to collect the player's name
            // In this example, using a simple DisplayPrompt dialog
            string playerName = await DisplayPromptAsync("Congratulations!", "You won!\nEnter your name:");
            return playerName;
        }
        private void UpdateLeaderboard(string playerName, int guesses, TimeSpan time)
        {
            // Create a new leaderboard item with the player's information
            LeaderboardItem newPlayerItem = new LeaderboardItem
            {
                Rank = 0, // Set the initial rank to 0
                Name = playerName,
                Guesses = guesses,
                Time = time
            };

            // Add the new player to the leaderboard data
            leaderboardData.Add(newPlayerItem);

            // Update the rank based on the number of guesses and time
            UpdateRanks();

            // Navigate to the LeaderboardPage and pass the updated data
            ShowLeaderboardPage(leaderboardData);
        }

        private async void ShowLeaderboardPage(List<LeaderboardItem> updatedData)
        {
            // Navigate to the LeaderboardPage and pass the updated data
            await Navigation.PushAsync(new LeaderboardPage(updatedData));
        }

        private void UpdateRanks()
        {
            // Order the leaderboard data by number of guesses (ascending) and then by time (ascending)
            var orderedLeaderboard = leaderboardData.OrderBy(item => item.Guesses).ThenBy(item => item.Time);

            // Update the rank based on the sorted order
            int rank = 1;
            foreach (var item in orderedLeaderboard)
            {
                item.Rank = rank++;
            }
        }
       

    }
}
