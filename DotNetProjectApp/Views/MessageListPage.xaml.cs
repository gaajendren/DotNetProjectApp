using Microsoft.Maui.Controls;
using DotNetProjectApp.Models;
using DotNetProjectApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel; // Add this for ObservableCollection

namespace DotNetProjectApp.Views
{
    public partial class MessageListPage : ContentPage
    {
        // Change List<Message> to ObservableCollection<Message>
        public ObservableCollection<Message> Messages { get; set; }
        public User CurrentUser { get; set; }

        public MessageListPage(User user)
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
                                       .Where(m => m.SenderUserID == currentUserId)
                                       .Include(m => m.SenderUser)
                                       .Include(m => m.ReceiverUser)
                                       .OrderByDescending(m => m.SentAt)
                                       .ToListAsync();

                Messages = new ObservableCollection<Message>(messages); 
                OnPropertyChanged(nameof(Messages));
            }
        }

        private async void OnEditMessageClicked(object sender, EventArgs e)
        {
            var message = (Message)((Button)sender).CommandParameter;
            await Navigation.PushAsync(new EditMessagePage(message, CurrentUser));
        }

        private async void OnCreateMessageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateMessagePage(CurrentUser));
        }

        private async void OnDeleteMessageClicked(object sender, EventArgs e)
        {
            var message = (Message)((Button)sender).CommandParameter;

            using (var db = new ApplicationDbContext())
            {
                db.Messages.Remove(message);
                await db.SaveChangesAsync();

                // Remove message from ObservableCollection
                Messages.Remove(message);

                await DisplayAlert("Success", "Message deleted successfully!", "OK");
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
