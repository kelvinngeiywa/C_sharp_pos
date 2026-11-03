namespace my_pos.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Double price { get; set; }
        public int Count { get; set; }
        public string AddedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsDeleted { get; set; }
    }
}
