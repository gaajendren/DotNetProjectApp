using DotNetProjectApp.Models;
using DotNetProjectApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace DotNetProjectApp.Views
{
    public partial class FollowPage : ContentPage
    {
        public ObservableCollection<User> Users { get; set; }
        public int CurrentUserId { get; set; }
         public User CurrentUser { get; set; }
        public FollowPage(User user1)
        {
            InitializeComponent();
            Users = new ObservableCollection<User>();
            CurrentUser = user1;
            BindingContext = this;

         
            CurrentUserId = CurrentUser.Id; 
        }

       
        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue?.ToLower() ?? string.Empty;

            if (!string.IsNullOrEmpty(searchText))
            {
               
                Users.Clear();

                try
                {
                    using (var db = new ApplicationDbContext())
                    {
                        // Query to find users based on the search input, excluding the current user
                        var users = await db.Users
                            .Where(u => u.Name.ToLower().Contains(searchText) && u.Id != CurrentUserId)  // Exclude current user
                            .ToListAsync();

                        // Check if the current user is following each user
                        foreach (var user in users)
                        {
                            var isFollowing = await db.Follows
                                .AnyAsync(f => f.FollowerUserID == CurrentUserId && f.FollowingUserID == user.Id);

                          
                            if (!isFollowing)
                            {
                                Users.Add(user);
                            }



                        }
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to load users: {ex.Message}", "OK");
                }
            }
        }

        // Follow/Unfollow button clicked
        private async void OnFollowButtonClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var userId = (int)button.CommandParameter;

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    // Check if the current user is already following the selected user
                    var isFollowing = await db.Follows
                        .AnyAsync(f => f.FollowerUserID == CurrentUserId && f.FollowingUserID == userId);

                    if (isFollowing)
                    {
                        // Unfollow logic: Remove the follow relationship
                        var follow = await db.Follows
                            .FirstOrDefaultAsync(f => f.FollowerUserID == CurrentUserId && f.FollowingUserID == userId);

                        if (follow != null)
                        {
                            db.Follows.Remove(follow);
                            button.Text = "Follow";  // Change button text to "Follow"
                        }
                    }
                    else
                    {
                        // Follow logic: Add a new follow relationship
                        var newFollow = new Follow
                        {
                            FollowerUserID = CurrentUserId,
                            FollowingUserID = userId
                        };

                        db.Follows.Add(newFollow);
                        button.Text = "Unfollow";  // Change button text to "Unfollow"
                    }

                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                await DisplayAlert("Error", $"Failed to follow/unfollow: {ex.Message}\nInner Exception: {innerExceptionMessage} {userId} {CurrentUserId}", "OK");
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
