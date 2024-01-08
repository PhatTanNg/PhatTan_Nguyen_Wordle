using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhatTan_Nguyen_Wordle
{
    public class LeaderboardItem
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public int Guesses { get; set; }
        public TimeSpan Time { get; set; }
    }

    public partial class LeaderboardPage : ContentPage
    {
        private List<LeaderboardItem> leaderboardData;

        public LeaderboardPage(List<LeaderboardItem> data)
        {
            InitializeComponent();
            leaderboardData = data;
            LeaderboardListView.ItemsSource = leaderboardData;
        }

        private List<LeaderboardItem> GetLeaderboardData()
        {
            // Replace this with your logic to fetch data from your data source
            // Example: return a list of dummy data
            return new List<LeaderboardItem>
        {
            new LeaderboardItem { Rank = 1, Name = "Player1", Guesses = 5, Time = TimeSpan.FromMinutes(3) },
            new LeaderboardItem { Rank = 2, Name = "Player2", Guesses = 7, Time = TimeSpan.FromMinutes(4) },
            // Add more items as needed
        };
        }
    }

}
