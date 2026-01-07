using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using my_shop.App.Services;
using System;
using System.Threading.Tasks;

namespace my_shop.App.ViewModels;

public partial class SignupViewModel : ObservableObject
{
    private readonly ISignupService _signupService;
    public XamlRoot XamlRoot { get; set; }

    [ObservableProperty] 
    private string email;
    
    [ObservableProperty] 
    private string password;
    
    [ObservableProperty] 
    private string name;
    
    [ObservableProperty] 
    private bool acceptTerms;

    public IAsyncRelayCommand SignupCommand { get; }

    public SignupViewModel(ISignupService signupService)
    {
        _signupService = signupService;
        SignupCommand = new AsyncRelayCommand(ExecuteSignup, CanExecuteSignup);
    }

    private bool CanExecuteSignup()
    {
        return !string.IsNullOrWhiteSpace(Email) &&
               !string.IsNullOrWhiteSpace(Password) &&
               !string.IsNullOrWhiteSpace(Name) &&
               AcceptTerms;
    }

    partial void OnEmailChanged(string value) => SignupCommand.NotifyCanExecuteChanged();
    partial void OnPasswordChanged(string value) => SignupCommand.NotifyCanExecuteChanged();
    partial void OnNameChanged(string value) => SignupCommand.NotifyCanExecuteChanged();
    partial void OnAcceptTermsChanged(bool value) => SignupCommand.NotifyCanExecuteChanged();

    private async Task ExecuteSignup()
    {
        await _signupService.SignupAsync(Email, Password, Name);
    }
}
