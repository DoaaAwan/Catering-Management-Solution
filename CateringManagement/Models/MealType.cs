using System.ComponentModel.DataAnnotations;

namespace CateringManagement.Models
{
    public class MealType
    {
        public int ID { get; set; }

        [Display(Name = "Meal Type")]
        [Required(ErrorMessage = "You cannot leave the meal type blank.")]
        [StringLength(80, ErrorMessage = "Meal type cannot be more than 80 characters long.")] //max length = 80
        public string Name { get; set; }

        public ICollection<Function> Functions { get; set; } = new HashSet<Function>();
    }
}
