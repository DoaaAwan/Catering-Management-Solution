using System.ComponentModel.DataAnnotations;

namespace CateringManagement.Models
{
    public class Room
    {
        public int ID { get; set; }

        [Display(Name = "Room")]
        [Required(ErrorMessage = "You cannot leave the Room blank.")]
        [StringLength(80, ErrorMessage = "Room cannot be more than 80 characters long.")] //max length = 80
        public string Name { get; set; }

        [Required(ErrorMessage = "You cannot leave the Capacity blank.")] //required
        public int Capacity { get {  return Capacity; } set { if (value > 0) Capacity = value; } } //must be > 0

        public ICollection<FunctionRoom> FunctionRooms { get; set; } = new HashSet<FunctionRoom>();
    }
}
