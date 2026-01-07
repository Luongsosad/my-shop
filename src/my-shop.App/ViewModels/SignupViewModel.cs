using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using my_shop.App.Services.LoginService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace my_shop.App.ViewModels;

public partial class SignupViewModel : ObservableObject
{
    private readonly ISignupService _signupService;
    public XamlRoot XamlRoot { get; set; }

    [ObservableProperty] private string email;
    [ObservableProperty] private string password;
    [ObservableProperty] private string name;

    public IAsyncRelayCommand SignupCommand { get; }

    public SignupViewModel(ISignupService signupService)
    {
        _signupService = signupService;
        SignupCommand = new AsyncRelayCommand(ExecuteSignup);
    }

    private async Task ExecuteSignup()
    {
        await _signupService.SignupAsync(Email, Password, Name);
    }
}
