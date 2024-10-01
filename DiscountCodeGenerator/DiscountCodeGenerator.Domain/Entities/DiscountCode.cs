namespace DiscountCodeGenerator.Domain.Entities
{
    public class DiscountCode
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public bool IsUsed { get; set; }
    }
}
