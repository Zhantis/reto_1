namespace ScrapperEy.Models
{
    public class OffShoreEntity
    {
        public int OffShoreEntityId { get; set; }
        public string EntityName { get; set; }
        public string LinkedTo { get; set; }
        public string DataFrom { get; set; }
        public DateTime Created { get; set; }
    }
}
