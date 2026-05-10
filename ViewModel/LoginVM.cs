using Boardgame.Data.Entities;
using Boardgame.Services;
using Boardgame.Utilities;
using System;
using System.Windows.Input;

namespace Boardgame.ViewModel
{
    internal class LoginVM : ViewModelBase
    {
        private readonly Action<UserEntity> _onSuccess;

        // --- Tab state ---
        private bool _isLoginMode = true;
        public bool IsLoginMode
        {
            get => _isLoginMode;
            set { _isLoginMode = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsRegisterMode)); ClearMessages(); }
        }
        public bool IsRegisterMode => !_isLoginMode;

        // --- Fields ---
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _displayName = string.Empty;
        public string DisplayName
        {
            get => _displayName;
            set { _displayName = value; OnPropertyChanged(); }
        }

        internal string Password { get; set; } = string.Empty;
        internal string ConfirmPassword { get; set; } = string.Empty;

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
        }

        private string _successMessage = string.Empty;
        public string SuccessMessage
        {
            get => _successMessage;
            set { _successMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasSuccess)); }
        }

        public bool HasError => !string.IsNullOrEmpty(_errorMessage);
        public bool HasSuccess => !string.IsNullOrEmpty(_successMessage);

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        // --- Commands ---
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand SwitchToLoginCommand { get; }
        public ICommand SwitchToRegisterCommand { get; }

        public LoginVM(Action<UserEntity> onSuccess)
        {
            _onSuccess = onSuccess;
            LoginCommand = new RelayCommand(DoLogin, _ => !string.IsNullOrWhiteSpace(Email) && !IsLoading);
            RegisterCommand = new RelayCommand(DoRegister, _ =>
                !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(DisplayName) && !IsLoading);
            SwitchToLoginCommand = new RelayCommand(_ => IsLoginMode = true);
            SwitchToRegisterCommand = new RelayCommand(_ => IsLoginMode = false);
        }

        private async void DoLogin(object? _)
        {
            if (string.IsNullOrWhiteSpace(Password)) { ErrorMessage = "Podaj haslo!"; return; }
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await Db.I.LoginAsync(Email, Password);
                if (user is null) { ErrorMessage = "Nieprawidlowy email lub haslo."; return; }
                _onSuccess(user);
            }
            catch (Exception ex) { ErrorMessage = $"Blad polaczenia: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private async void DoRegister(object? _)
        {
            if (string.IsNullOrWhiteSpace(Password)) { ErrorMessage = "Podaj haslo!"; return; }
            if (Password.Length < 6) { ErrorMessage = "Haslo musi miec minimum 6 znakow."; return; }
            if (Password != ConfirmPassword) { ErrorMessage = "Hasla nie sa identyczne!"; return; }
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var (user, error) = await Db.I.RegisterAsync(Email, Password, DisplayName);
                if (error is not null) { ErrorMessage = error; return; }

                SuccessMessage = $"Konto \"{user!.DisplayName}\" utworzone! Mozesz sie teraz zalogowac.";
                IsLoginMode = true;
                Email = user.Email;
                DisplayName = string.Empty;
                Password = string.Empty;
                ConfirmPassword = string.Empty;
            }
            catch (Exception ex) { ErrorMessage = $"Blad: {ex.Message}"; }
            finally { IsLoading = false; }
        }

        private void ClearMessages() { ErrorMessage = string.Empty; SuccessMessage = string.Empty; }
    }
}
