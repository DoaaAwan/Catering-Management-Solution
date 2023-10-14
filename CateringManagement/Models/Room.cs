using System.ComponentModel.DataAnnotations;
//Done by: Doaa Awan

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
        [Display(Name = "Capacity")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero.")] //int > 0
        public int Capacity { get; set; } = 1;

        public ICollection<FunctionRoom> FunctionRooms { get; set; } = new HashSet<FunctionRoom>(); //collection of FunctionRooms
    }
}
