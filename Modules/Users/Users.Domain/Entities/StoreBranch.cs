namespace Users.Domain.Entities
{
    public class StoreBranch
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public bool IsDeleted { get; set; } = false; // ✅

        public ICollection<User> Users { get; set; }
    }

}
