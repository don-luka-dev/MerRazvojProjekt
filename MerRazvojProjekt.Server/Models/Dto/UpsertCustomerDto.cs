namespace MerRazvojProjekt.Server.Models.DTO
{
    public class UpsertCustomerDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}
