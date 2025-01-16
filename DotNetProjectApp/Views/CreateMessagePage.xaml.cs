using Microsoft.Maui.Controls;
using DotNetProjectApp.Models;
using DotNetProjectApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;  


namespace DotNetProjectApp.Views
{
    public partial class CreateMessagePage : ContentPage
    {
        public User CurrentUser { get; set; }
        public List<User> FollowedUsers { get; set; }
        public User SelectedReceiver { get; set; }
        public CreateMessagePage(User user)
        {
            InitializeComponent();
            CurrentUser = user;
            BindingContext = this;

            // Load followed users
            LoadFollowedUsers();
           
        }



        private async void LoadFollowedUsers()
        {
            using (var db = new ApplicationDbContext())
            {
                var currentUserId = CurrentUser.Id;
                FollowedUsers = await db.Follows
                                         .Where(f => f.FollowerUserID == currentUserId)
                                         .Select(f => f.FollowingUser)
                                         .ToListAsync();
            }

           
            ReceiverPicker.ItemsSource = FollowedUsers;
        }


        private async void OnSendMessageClicked(object sender, EventArgs e)
        {
            if (SelectedReceiver == null || string.IsNullOrWhiteSpace(MessageEditor.Text))
            {
                await DisplayAlert("Error", "Please select a receiver and write a message.", "OK");
                return;
            }

            // Create and save the message
            var message = new Message
            {
                SenderUserID = CurrentUser.Id, // Replace with current user ID
                ReceiverUserID = SelectedReceiver.Id,
                Content = MessageEditor.Text,
                SentAt = DateTime.Now
            };

            using (var db = new ApplicationDbContext())
            {
                db.Messages.Add(message);
                await db.SaveChangesAsync();
            }

            await DisplayAlert("Success", "Message sent successfully!", "OK");
            MessageEditor.Text = string.Empty;

            await Navigation.PushAsync(new MessageListPage(CurrentUser));
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
