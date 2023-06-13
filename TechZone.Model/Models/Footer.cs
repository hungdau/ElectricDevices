using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechZone.Model.Models
{
    [Table("Footers")]
    public class Footer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MaxLength(50)]
        public string ID { get; set; }

        [Required]
        public string Content { get; set; }
    }
}