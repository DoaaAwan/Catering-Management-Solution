using System.ComponentModel.DataAnnotations;
//Done by: Doaa Awan

namespace CateringManagement.Models
{
    public class FunctionRoom
    {
        [Display(Name = "Function")]
        [Required(ErrorMessage = "You must select a Function")]
        public int FunctionID { get; set; } //FunctionID (PK)

        [Display(Name = "Function")]
        public Function Function { get; set; } //Function (FK)


        [Display(Name = "Room")]
        [Required(ErrorMessage = "You must select a Room")]
        public int RoomID { get; set; } //RoomID (PK)

        [Display(Name = "Room")]
        public Room Room { get; set; } //Room (FK)
    }
}
