using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using StudentManagementSystem.Features.Data.Enums;
using StudentManagementSystem.Features.Data.Models;

namespace StudentManagementSystem.Features.Helpers;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal? _currentUser;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _currentUser ?? new ClaimsPrincipal(new ClaimsIdentity());
        return Task.FromResult(new AuthenticationState(user));
    }

    public Task MarkUserAsAuthenticated(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("UserId", user.Id.ToString())
        };

        if (user.StudentId.HasValue)
            claims.Add(new Claim("StudentId", user.StudentId.Value.ToString()));

        var identity = new ClaimsIdentity(claims, "CustomAuth");
        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        return Task.CompletedTask;
    }

    public Task MarkUserAsLoggedOut()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        return Task.CompletedTask;
    }

    public string? GetCurrentUsername()
    {
        return _currentUser?.Identity?.Name;
    }

    public string? GetCurrentRole()
    {
        return _currentUser?.FindFirst(ClaimTypes.Role)?.Value;
    }

    public int? GetCurrentUserId()
    {
        var userIdClaim = _currentUser?.FindFirst("UserId")?.Value;
        return int.TryParse(userIdClaim, out var id) ? id : null;
    }

    public int? GetCurrentStudentId()
    {
        var studentIdClaim = _currentUser?.FindFirst("StudentId")?.Value;
        return int.TryParse(studentIdClaim, out var id) ? id : null;
    }

    public bool IsInRole(UserRole role)
    {
        return _currentUser?.IsInRole(role.ToString()) ?? false;
    }
}
