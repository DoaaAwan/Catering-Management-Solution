using System.ComponentModel.DataAnnotations;

namespace CateringManagement.Models
{
    public class Function : IValidatableObject
    {
        public int ID { get; set; }

        #region Summary Properties

        [Display(Name = "Function")]
        public string Summary
        {
            get //removed: " (" + DurationDays + " day" + (DurationDays > 1 ? "s) - " : ") - ")
            {
                string summary = StartTime.ToString("yyyy-MM-dd") +
                    (string.IsNullOrEmpty(Name) ? (!string.IsNullOrEmpty(LobbySign) ? LobbySign : "TBA") : Name);

                return summary;
            }
        }

        [Display(Name = "Estimated Value")]
        public string EstimatedValue
        {
            get
            {
                // Returns the function's Estimated Value (BaseCharge plus SOCAN fee plus the Guaranteed Number times the PerPersonCharge.) formatted as currency
                return (BaseCharge + SOCAN + (GuaranteedNumber * PerPersonCharge)).ToString("c");
            }
        }

        #endregion

        [StringLength(120, ErrorMessage = "Name cannot be more than 120 characters long.")]
        public string Name { get; set; }

        [Display(Name = "Lobby Sign")]
        [StringLength(60, ErrorMessage = "Lobby sign cannot be more than 60 characters long.")]
        public string LobbySign { get; set; }

        //Date will become StartTime

        [Required(ErrorMessage = "You cannot leave the date blank.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; } = DateTime.Today;

        //Eliminate the DurationDays property from the Function class

            //[Required(ErrorMessage = "You must enter the duration.")]
            //[Display(Name = "Duration Days")]
            //[Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than zero.")]
            //public int DurationDays { get; set; } = 1;

        [Required(ErrorMessage = "You must enter the Base Charge.")]
        [Display(Name = "Base Charge")]
        [DataType(DataType.Currency)]
        public double BaseCharge { get; set; }

        [Required(ErrorMessage = "You must enter the Charge Per Person.")]
        [Display(Name = "Per Person")]
        [DataType(DataType.Currency)]
        public double PerPersonCharge { get; set; }

        [Required(ErrorMessage = "The Guaranteed Number is required.")]
        [Display(Name = "Guaranteed Number")]
        [Range(1, int.MaxValue, ErrorMessage = "Guaranteed number must be greater than zero")]
        public int GuaranteedNumber { get; set; } = 1;

        [Required(ErrorMessage = "You must enter a value for the SOCAN fee.  Use 0.00 if no fee is applicable.")]
        [Display(Name = "SOCAN")]
        [DataType(DataType.Currency)]
        public double SOCAN { get; set; } = 50.00;

        [Required(ErrorMessage = "Amount for the Deposit is required.")]
        [DataType(DataType.Currency)]
        public double Deposit { get; set; }

        [Display(Name = "Deposit Paid")]
        public bool DepositPaid { get; set; } = false;

        [Display(Name = "No HST")]
        public bool NoHST { get; set; } = false;

        [Display(Name = "No Gratuity")]
        public bool NoGratuity { get; set; } = false;

        // foreign keys
        [Display(Name = "Customer")]
        [Required(ErrorMessage = "You must select a Customer")]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        [Display(Name = "Function Type")]
        [Required(ErrorMessage = "You must select a Function Type")]
        public int FunctionTypeID { get; set; }

        [Display(Name = "Function Type")]
        public FunctionType FunctionType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Function Date cannot be before January 1st, 2018 because that is when the Hotel opened.
            if (StartTime < DateTime.Parse("2018-01-01"))
            {
                yield return new ValidationResult("Date cannot be before January 1st, 2018.", new[] { "Date" });
            }

            // Function Date cannot be more than 10 years in the future from the current date.
            if (StartTime > DateTime.Now.AddYears(10))
            {
                yield return new ValidationResult("Date cannot be more than 10 years in the future.", new[] { "Date" });
            }
        }
    }
}
