namespace MerRazvojProjekt.Server.Models.Dto.MiscDto
{
    public class RequestLogDto
    {
        public int Id { get; set; }
        public string HttpMethod { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public long ResponseTimeMs { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
