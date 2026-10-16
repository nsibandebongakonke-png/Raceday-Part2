namespace Raceday.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string UserName { get; init; } = string.Empty;

        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
