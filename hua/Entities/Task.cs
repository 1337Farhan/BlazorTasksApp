using System.ComponentModel.DataAnnotations;

namespace hua.Entities
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(120)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, In Progress, Done
        public virtual User? AssignedToUser { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? CompletedDate { get; set; }

    }
}