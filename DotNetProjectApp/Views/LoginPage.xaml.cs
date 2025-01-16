
using DotNetProjectApp.Data;
using DotNetProjectApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetProjectApp.Views;

public partial class LoginPage : ContentPage
{
    private ApplicationDbContext _dbContext;
    public LoginPage()
	{
		InitializeComponent();
        _dbContext = new ApplicationDbContext();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please enter both email and password", "OK");
            return;
        }

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

        if (user == null)
        {
            await DisplayAlert("Error", "Invalid credentials", "OK");
        }
        else
        {
            Preferences.Set("UserID", user.Id);
            Preferences.Set("UserName", user.Name);
           
            await Navigation.PushAsync(new DashboardPage(user));
        }
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }



}