using Microsoft.Maui.Controls;
using DotNetProjectApp.Models;
using DotNetProjectApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DotNetProjectApp.Views
{
    public partial class ReceivedMessagesPage : ContentPage
    {
        public ObservableCollection<Message> Messages { get; set; }
        public User CurrentUser { get; set; }

        public ReceivedMessagesPage(User user)
        {
            InitializeComponent();
            CurrentUser = user;
            BindingContext = this;
            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadMessages();
        }

        private async Task LoadMessages()
        {
            using (var db = new ApplicationDbContext())
            {
                var currentUserId = CurrentUser.Id;
                var messages = await db.Messages
                                       .Where(m => m.ReceiverUserID == currentUserId) // Filter by ReceiverUserID
                                       .Include(m => m.SenderUser) // Include sender details
                                       .OrderByDescending(m => m.SentAt) // Order by sent date
                                       .ToListAsync();

                Messages = new ObservableCollection<Message>(messages);
                OnPropertyChanged(nameof(Messages));
            }
        }

        // Log out functionality
        private async void OnLogOutClicked(object sender, EventArgs e)
        {
            Preferences.Clear();  // Clear the saved user preferences
            await Navigation.PopToRootAsync();  // Navigate to the root page (e.g., Login page)
        }

        // Navigate to the profile page
        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage(CurrentUser));
        }

        // Navigate to the list of posts
        private async void OnViewPostClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PostsListPage(CurrentUser));
        }

        // Navigate to the list of followed users
        private async void OnViewFollowedUsersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FollowingListPage(CurrentUser));
        }
        private async void OnViewDashboardClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DashboardPage(CurrentUser));
        }
        private async void OnMessageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MessageListPage(CurrentUser));
        }

        private async void OnReceivedMessageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ReceivedMessagesPage(CurrentUser));
        }

    }
}
