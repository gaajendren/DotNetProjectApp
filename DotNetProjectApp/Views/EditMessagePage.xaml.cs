using Microsoft.Maui.Controls;
using DotNetProjectApp.Models;
using DotNetProjectApp.Data;

namespace DotNetProjectApp.Views
{
    public partial class EditMessagePage : ContentPage
    {
        public Message Message { get; set; }
        public User CurrentUser { get; set; }
        public EditMessagePage(Message message, User user)
        {
            InitializeComponent();
            CurrentUser = user;
            Message = message;
            BindingContext = this;

            MessageEditor.Text = Message.Content;
        }

        private async void OnSaveChangesClicked(object sender, EventArgs e)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Messages.Update(Message);
                await db.SaveChangesAsync();
            }

            await DisplayAlert("Success", "Message updated successfully!", "OK");
            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); 
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
