using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartGarden.App.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected async Task ExecuteAsync(Func<Task> operation, string? errorMessage = null)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                await operation();
            }
            catch (Exception ex)
            {
                ErrorMessage = errorMessage ?? ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, string? errorMessage = null)
        {
            if (IsBusy)
                return default;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                return await operation();
            }
            catch (Exception ex)
            {
                ErrorMessage = errorMessage ?? ex.Message;
                return default;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
