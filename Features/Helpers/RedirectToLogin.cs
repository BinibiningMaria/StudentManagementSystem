using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace StudentManagementSystem.Features.Helpers;

public class RedirectToLogin : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; } = default!;
    private string? _loginUrl;

    protected override async Task OnParametersSetAsync()
    {
        var authState = await AuthenticationState;
        if (authState.User.Identity is null || !authState.User.Identity.IsAuthenticated)
        {
            var returnUrl = Uri.EscapeDataString(NavigationManager.Uri);
            _loginUrl = $"/login?returnUrl={returnUrl}";
        }
        else
        {
            _loginUrl = null;
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && !string.IsNullOrWhiteSpace(_loginUrl))
        {
            NavigationManager.NavigateTo(_loginUrl, replace: true);
        }
    }
}
