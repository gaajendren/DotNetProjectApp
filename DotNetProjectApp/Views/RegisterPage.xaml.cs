
using DotNetProjectApp.Data;
using DotNetProjectApp.Models;
using Microsoft.EntityFrameworkCore;


namespace DotNetProjectApp.Views;

public partial class RegisterPage : ContentPage
{
    private ApplicationDbContext _dbContext;
    public RegisterPage()
	{
		InitializeComponent();
        _dbContext = new ApplicationDbContext();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var name = NameEntry.Text;
        var email = EmailEntry.Text;
        var phone = PhoneEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please fill all fields", "OK");
            return;
        }

        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            await DisplayAlert("Error", "Email is already registered", "OK");
            return;
        }

        var user = new User
        {
            Name = name,
            Email = email,
            Phone = phone,
            Password = password
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        await DisplayAlert("Success", "Registration successful", "OK");
        await Navigation.PopAsync();
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }


   
}