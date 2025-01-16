using DotNetProjectApp.Models;
using DotNetProjectApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace DotNetProjectApp.Views
{
    public partial class PostsListPage : ContentPage
    {
        public ObservableCollection<Post> Posts { get; set; }
        public int CurrentUserId { get; set; }
        public User CurrentUser { get; set; }
        public PostsListPage(User user)
        {
            InitializeComponent();
            Posts = new ObservableCollection<Post>();
            CurrentUser = user;
            BindingContext = this;

            // Get the current user ID from preferences
            CurrentUserId = CurrentUser.Id; // Assuming user is logged in and UserID is saved in Preferences
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Clear the list before fetching new data
            Posts.Clear();

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    // Fetch posts by the current user only
                    var posts = await db.Posts
                                         .Where(p => p.UserId == CurrentUserId)  // Filter by UserId
                                         .OrderByDescending(p => p.CreatedAt)   // Optional: Order by created date
                                         .Take(1000)                            // Optional: Limit the number of posts
                                         .ToListAsync();

                    // Add the posts to the ObservableCollection
                    foreach (var post in posts)
                    {
                        Posts.Add(post);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load posts: {ex.Message}", "OK");
            }
        }


        private async void OnEditPostClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var post = button?.CommandParameter as Post;

            if (post != null)
            {
                // Navigate to the EditPostPage with the selected post
                await Navigation.PushAsync(new EditPostPage(post, CurrentUser));
            }
        }

        private async void OnDeletePostClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var post = button?.CommandParameter as Post;

            if (post != null)
            {
                var confirm = await DisplayAlert("Delete", $"Are you sure you want to delete this post: \"{post.Caption}\"?", "Yes", "No");
                if (confirm)
                {
                    try
                    {
                        using (var db = new ApplicationDbContext())
                        {
                            // Remove the post from the database
                            db.Posts.Remove(post);
                            await db.SaveChangesAsync();

                            // Remove the post from the ObservableCollection
                            Posts.Remove(post);

                            await DisplayAlert("Success", "Post deleted successfully!", "OK");
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"Failed to delete the post: {ex.Message}", "OK");
                    }
                }
            }
        }



        private async void OnCreatePostClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreatePostPage(CurrentUser));
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
