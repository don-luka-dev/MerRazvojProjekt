namespace MerRazvojProjekt.Server.Models.Dto.StatisticDto
{
    public class StatsDto
    {
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public List<CountDto> TopCities { get; set; } = new();
    }
}
