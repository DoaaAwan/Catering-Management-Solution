using System.ComponentModel.DataAnnotations;

namespace CateringManagement.Models
{
    public class Room
    {
        public int ID { get; set; }

        [Display(Name = "Room")]
        [Required(ErrorMessage = "You cannot leave the room blank.")]
        [StringLength(80, ErrorMessage = "Room cannot be more than 80 characters long.")] //max length = 80
        public string Name { get; set; }

        [Required(ErrorMessage = "You cannot leave the capacity blank.")] //required
        public int Capacity { get; set; } //must be > 0

        //public ICollection<Function> Functions { get; set; } = new HashSet<Function>();
    }
}
