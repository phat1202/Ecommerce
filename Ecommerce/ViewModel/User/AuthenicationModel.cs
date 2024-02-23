namespace Ecommerce.ViewModel.User
{
    public class AuthenicationModel
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public int? Gender { get; set; }
        public int? Role { get; set; }
        public string? ErrorMessage { get; set; }
        public string? CartId { get; set; }
    }
}
