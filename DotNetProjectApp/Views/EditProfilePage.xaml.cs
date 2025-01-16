using DotNetProjectApp.Models;
using DotNetProjectApp.Data;
namespace DotNetProjectApp.Views;

public partial class EditProfilePage : ContentPage
{
    public User CurrentUser { get; set; }

    public EditProfilePage(User user)
    {
        InitializeComponent();  
        CurrentUser = user;
        BindingContext = this;
    }

    private async void OnSaveChangesClicked(object sender, EventArgs e)
    {
        // Update user details
        CurrentUser.Name = NameEntry.Text;
        CurrentUser.Email = EmailEntry.Text;
        CurrentUser.Phone = PhoneEntry.Text;

        try
        {
            using (var db = new ApplicationDbContext())
            {
                db.Users.Update(CurrentUser);
                await db.SaveChangesAsync();
            }

            Preferences.Set("UserID", CurrentUser.Id);
            Preferences.Set("UserName", CurrentUser.Name);

            await DisplayAlert("Success", "Profile updated successfully!", "OK");

            
            await Navigation.PushAsync(new ProfilePage(CurrentUser));
         
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to update profile: {ex.Message}", "OK");
        }

        // Handle button click events
       
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
