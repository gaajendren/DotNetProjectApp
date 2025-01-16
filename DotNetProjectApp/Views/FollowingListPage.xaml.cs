using DotNetProjectApp.Models;
using DotNetProjectApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace DotNetProjectApp.Views
{
    public partial class FollowingListPage : ContentPage
    {
        public ObservableCollection<User> FollowingUsers { get; set; }
        public int CurrentUserId { get; set; }
        public User CurrentUser { get; set; }

        public FollowingListPage(User user)
        {
            InitializeComponent();
            FollowingUsers = new ObservableCollection<User>();
            CurrentUser = user;
            BindingContext = this;

            // Get the current user ID from preferences
            CurrentUserId = CurrentUser.Id;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Clear the list before fetching new data
            FollowingUsers.Clear();

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    // Get the list of users the current user is following
                    var followedUserIds = await db.Follows
                        .Where(f => f.FollowerUserID == CurrentUserId)
                        .Select(f => f.FollowingUserID)
                        .ToListAsync();

                    // Get the user details for each followed user
                    var followedUsers = await db.Users
                        .Where(u => followedUserIds.Contains(u.Id))
                        .ToListAsync();

                    // Add each followed user to the collection
                    foreach (var user in followedUsers)
                    {
                        FollowingUsers.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load followed users: {ex.Message}", "OK");
            }
        }

        // Unfollow user logic
        private async void OnUnfollowClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var userToUnfollow = button.BindingContext as User;

            if (userToUnfollow == null)
                return;

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    // Find the follow relationship in the database
                    var follow = await db.Follows
                        .FirstOrDefaultAsync(f => f.FollowerUserID == CurrentUserId && f.FollowingUserID == userToUnfollow.Id);

                    if (follow != null)
                    {
                        // Remove the follow relationship from the database
                        db.Follows.Remove(follow);
                        await db.SaveChangesAsync();

                        // Remove the user from the displayed list
                        FollowingUsers.Remove(userToUnfollow);
                        await DisplayAlert("Success", $"You have unfollowed {userToUnfollow.Name}.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to unfollow the user: {ex.Message}", "OK");
            }
        }

        private async void OnViewFollowClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            await Navigation.PushAsync(new FollowPage(CurrentUser));
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
