using CateringManagement.Models;
using System.Diagnostics;

namespace CateringManagement.Data
{
    public static class CMInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            CateringContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<CateringContext>();

            try
            {
                //We can use this to delete the database and start fresh.
                //context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Look for any Customers.  Since we can't have Functions without Customers.
                if (!context.Customers.Any())
                {
                    context.Customers.AddRange(
                        new Customer
                        {
                            FirstName = "Gregory",
                            MiddleName = "A",
                            LastName = "House",
                            CompanyName = "JPMorgan Chase",
                            Phone = "4165551234",
                            CustomerCode = "C6555123"
                        },
                        new Customer
                        {
                            FirstName = "Doogie",
                            MiddleName = "R",
                            LastName = "Houser",
                            CompanyName = "Agriculture and Agri-Food Canada",
                            Phone = "5195551212",
                            CustomerCode = "G9555121"
                        },
                        new Customer
                        {
                            FirstName = "Charles",
                            LastName = "Xavier",
                            CompanyName = null,
                            Phone = "9055552121",
                            CustomerCode = "I5555212"
                        });

                    context.SaveChanges();
                }
                // Seed FunctionTypes if there aren't any.
                if (!context.FunctionTypes.Any())
                {
                    context.FunctionTypes.AddRange(
                        new FunctionType
                        {
                            Name = "Meeting"
                        },
                        new FunctionType
                        {
                            Name = "Hospitality Room"
                        },
                        new FunctionType
                        {
                            Name = "Wedding"
                        },
                        new FunctionType
                        {
                            Name = "Dance"
                        },
                        new FunctionType
                        {
                            Name = "Exhibits"
                        },
                        new FunctionType
                        {
                            Name = "Birthday"
                        },
                        new FunctionType
                        {
                            Name = "Presentation"
                        });

                    context.SaveChanges();
                }
                // Seed Functions if there aren't any.
                if (!context.Functions.Any())
                {
                    context.Functions.AddRange(
                        new Function
                        {
                            Name = "JPMorgan Chase Shareholders Meeting",
                            LobbySign = "JPMorgan Chase",
                            StartTime = new DateTime(2023, 11, 11),
                            //DurationDays = 2,
                            BaseCharge = 22000.00,
                            PerPersonCharge = 125.00,
                            GuaranteedNumber = 200,
                            SOCAN = 50.00,
                            Deposit = 50000.00,
                            DepositPaid = true,
                            NoHST = false,
                            NoGratuity = false,
                            CustomerID = context.Customers.FirstOrDefault(d => d.FirstName == "Gregory" && d.LastName == "House").ID,
                            FunctionTypeID = context.FunctionTypes.FirstOrDefault(f => f.Name == "Meeting").ID
                        },
                        new Function
                        {
                            Name = "Xavier Birthday Party",
                            LobbySign = "Happy Birthday Mom!",
                            StartTime = new DateTime(2023, 12, 12),
                            //DurationDays = 1,
                            BaseCharge = 1000.00,
                            PerPersonCharge = 20.00,
                            GuaranteedNumber = 50,
                            SOCAN = 50.00,
                            Deposit = 500.00,
                            DepositPaid = true,
                            NoHST = false,
                            NoGratuity = false,
                            CustomerID = context.Customers.FirstOrDefault(c => c.FirstName == "Charles" && c.LastName == "Xavier").ID,
                            FunctionTypeID = context.FunctionTypes.FirstOrDefault(f => f.Name == "Birthday").ID
                        },
                        new Function
                        {
                            Name = "Behind the Numbers: What’s Causing Growth in Food Prices",
                            LobbySign = "Food Price Inflation",
                            StartTime = new DateTime(2023, 12, 25),
                            //DurationDays = 1,
                            BaseCharge = 2000.00,
                            PerPersonCharge = 50.00,
                            GuaranteedNumber = 40,
                            SOCAN = 50.00,
                            Deposit = 500.00,
                            DepositPaid = false,
                            NoHST = true,
                            NoGratuity = true,
                            CustomerID = context.Customers.FirstOrDefault(c => c.FirstName == "Doogie" && c.LastName == "Houser").ID,
                            FunctionTypeID = context.FunctionTypes.FirstOrDefault(f => f.Name == "Presentation").ID
                        });

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
