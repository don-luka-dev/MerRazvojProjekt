namespace MerRazvojProjekt.Server.Models.Dto.CarDto
{
    public class UpsertCarDto
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year { get; set; }
        public string Color { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
