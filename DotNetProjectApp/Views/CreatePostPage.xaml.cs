using DotNetProjectApp.Models;
using DotNetProjectApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage; // For FilePicker
using System.IO;
using System.Collections.ObjectModel;

namespace DotNetProjectApp.Views
{
    public partial class CreatePostPage : ContentPage
    {
        private string _selectedImagePath; // Variable to store the selected image path

        public User CurrentUser { get; set; }

        public CreatePostPage(User user)
        {
            InitializeComponent();
            CurrentUser = user;
            BindingContext = this;
        }

        // This method handles the image selection when the button is clicked
        private async void OnSelectImageClicked(object sender, EventArgs e)
        {
            var fileResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images
            });

            if (fileResult != null)
            {
                // Display the selected image in the Image control
                var imageSource = ImageSource.FromFile(fileResult.FullPath);
                SelectedImage.Source = imageSource;

                // Store the file path in the class variable
                _selectedImagePath = fileResult.FullPath;
            }
            else
            {
                await DisplayAlert("Error", "No image selected", "OK");
            }
        }

        private async void OnPostClicked(object sender, EventArgs e)
        {
            var caption = CaptionEntry.Text;
            var userId = CurrentUser.Id; // Assuming the user is logged in and their ID is stored in Preferences

            if (string.IsNullOrEmpty(caption))
            {
                await DisplayAlert("Error", "Caption cannot be empty", "OK");
                return;
            }

            // Make sure the image is selected before posting
            if (string.IsNullOrEmpty(_selectedImagePath))
            {
                await DisplayAlert("Error", "Please select an image", "OK");
                return;
            }

            // Step 2: Copy the file to a folder in the app's file system
            var appFolderPath = FileSystem.AppDataDirectory; // Folder for storing the image
            var newFilePath = Path.Combine(appFolderPath, Path.GetFileName(_selectedImagePath));

            try
            {
                // Copy the selected file to the new location
                using (var stream = File.OpenRead(_selectedImagePath))
                {
                    using (var newFileStream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await stream.CopyToAsync(newFileStream);
                    }
                }

                // Step 3: Save the new file path in the database
                var post = new Post
                {
                    Caption = caption,
                    ImagePath = newFilePath,  // Store the image path
                    UserId = userId,
                    CreatedAt = DateTime.Now
                };

                using (var db = new ApplicationDbContext())
                {
                    db.Posts.Add(post);
                    await db.SaveChangesAsync();
                }

                await DisplayAlert("Success", "Post created successfully!", "OK");

                // Navigate to another page (e.g., Dashboard or Post List)
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save the image: {ex.Message}", "OK");
            }
        }

        // Handle button click events
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
