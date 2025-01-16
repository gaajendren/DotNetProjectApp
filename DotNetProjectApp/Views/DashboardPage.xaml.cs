using DotNetProjectApp.Models;
using DotNetProjectApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace DotNetProjectApp.Views
{
    public partial class DashboardPage : ContentPage
    {
        public User CurrentUser { get; set; }
        public ObservableCollection<Post> Posts { get; set; }
        public int CurrentUserId { get; set; }

        public DashboardPage(User user) 
        {
            InitializeComponent();
            CurrentUser = user;
            Posts = new ObservableCollection<Post>();
            BindingContext = this;
            CurrentUserId = CurrentUser.Id;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Fetch and display the current user's info
            if (CurrentUserId != 0)
            {
                using (var db = new ApplicationDbContext())
                {
                    CurrentUser = await db.Users.FindAsync(CurrentUserId);
                    BindingContext = this;  // Update the binding context with the current user

                    // Fetch the list of users the current user is following (excluding the current user)
                    var followedUserIds = await db.Follows
                        .Where(f => f.FollowerUserID == CurrentUserId)
                        .Select(f => f.FollowingUserID)
                        .ToListAsync();

                    // Fetch posts of followed users, excluding the current user's own posts
                    var posts = await db.Posts
                        .Where(p => followedUserIds.Contains(p.UserId) && p.UserId != CurrentUserId)
                        .OrderByDescending(p => p.CreatedAt)
                        .Take(100)  // Optional: Limit the number of posts
                        .ToListAsync();

                    // Add the posts to the ObservableCollection
                    Posts.Clear();
                    foreach (var post in posts)
                    {
                        Posts.Add(post);
                    }
                }
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
