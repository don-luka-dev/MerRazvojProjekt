namespace MerRazvojProjekt.Server.Models.Dto.CustomerDto
{
    public class CustomerFilterDto
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; } = "id"; 
        public string? SortDirection { get; set; } = "asc"; 
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10;
    }
}
