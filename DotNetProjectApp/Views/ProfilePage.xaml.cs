
using DotNetProjectApp.Models;

using DotNetProjectApp.Data;

namespace DotNetProjectApp.Views;

public partial class ProfilePage : ContentPage
{
    public User CurrentUser { get; set; }
    public ProfilePage(User user)
    {
        InitializeComponent();
        CurrentUser = user;
        BindingContext = this;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

       
        var userName = Preferences.Get("UserName", string.Empty);

        if (!string.IsNullOrEmpty(userName))
        {

            CurrentUser.Name = userName;
            BindingContext = this;
        }
    }


    private async void OnEditProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditProfilePage(CurrentUser));
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