using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using DotNetProjectApp.Models;
using DotNetProjectApp.Data;

namespace DotNetProjectApp.Views
{
    public partial class EditPostPage : ContentPage
    {
        private readonly Post _post; // The post being edited
        public User CurrentUser { get; set; }
        public EditPostPage(Post post, User user)
        {
            InitializeComponent();
            _post = post;
            CurrentUser = user;
            BindingContext = this;

            CaptionEntry.Text = _post.Caption;
            if (!string.IsNullOrEmpty(_post.ImagePath)){ 
                ImagePreview.Source = _post.ImagePath; 
            }
        }

        private async void OnPickImageClicked(object sender, EventArgs e)
        {
            try
            {
                // Open the file picker for images
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select an image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null && File.Exists(result.FullPath))
                {
                    // Preview and save the image path
                    ImagePreview.Source = result.FullPath;
                    _post.ImagePath = result.FullPath;
                }
                else
                {
                    await DisplayAlert("Error", "Selected file does not exist.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick an image: {ex.Message}", "OK");
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    // Find and update the post in the database
                    var postToUpdate = await db.Posts.FindAsync(_post.Id);
                    if (postToUpdate != null)
                    {
                        postToUpdate.Caption = CaptionEntry.Text;
                        postToUpdate.ImagePath = _post.ImagePath;

                        await db.SaveChangesAsync();

                        await DisplayAlert("Success", "Post updated successfully!", "OK");
                        await Navigation.PopAsync(); // Go back
                    }
                    else
                    {
                        await DisplayAlert("Error", "Post not found in the database.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to update the post: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Navigate back without changes
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
