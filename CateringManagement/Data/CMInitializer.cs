using CateringManagement.Models;
using Microsoft.EntityFrameworkCore;
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
                //context.Database.Migrate();

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
                // Seed MealTypes if there aren't any.
                if (!context.MealTypes.Any())
                {
                    context.MealTypes.AddRange(
                        new MealType
                        {
                            Name = "None"
                        },
                        new MealType
                        {
                            Name = "Vegetarian"
                        },
                        new MealType
                        {
                            Name = "Vegan"
                        },
                        new MealType
                        {
                            Name = "Pescatarian"
                        },
                        new MealType
                        {
                            Name = "Gluten-Free"
                        },
                        new MealType
                        {
                            Name = "Halal"
                        },
                        new MealType
                        {
                            Name = "Kosher"
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
                            EndTime = new DateTime(2023, 11, 12),
                            //DurationDays = 2,
                            BaseCharge = 22000.00,
                            PerPersonCharge = 125.00,
                            GuaranteedNumber = 200,
                            SOCAN = 50.00,
                            Deposit = 50000.00,
                            DepositPaid = true,
                            NoHST = false,
                            NoGratuity = false,
                            Alcohol = true,
                            MealTypeID = context.MealTypes.FirstOrDefault(d => d.Name == "Vegan").ID,
                            CustomerID = context.Customers.FirstOrDefault(d => d.FirstName == "Gregory" && d.LastName == "House").ID,
                            FunctionTypeID = context.FunctionTypes.FirstOrDefault(f => f.Name == "Meeting").ID
                        },
                        new Function
                        {
                            Name = "Xavier Birthday Party",
                            LobbySign = "Happy Birthday Mom!",
                            StartTime = new DateTime(2023, 12, 12),
                            EndTime = new DateTime(2023, 12, 13),
                            //DurationDays = 1,
                            BaseCharge = 1000.00,
                            PerPersonCharge = 20.00,
                            GuaranteedNumber = 50,
                            SOCAN = 50.00,
                            Deposit = 500.00,
                            DepositPaid = true,
                            NoHST = false,
                            NoGratuity = false,
                            Alcohol = false,
                            MealTypeID = context.MealTypes.FirstOrDefault(d => d.Name == "None").ID,
                            CustomerID = context.Customers.FirstOrDefault(c => c.FirstName == "Charles" && c.LastName == "Xavier").ID,
                            FunctionTypeID = context.FunctionTypes.FirstOrDefault(f => f.Name == "Birthday").ID
                        },
                        new Function
                        {
                            Name = "Behind the Numbers: What’s Causing Growth in Food Prices",
                            LobbySign = "Food Price Inflation",
                            StartTime = new DateTime(2023, 12, 25),
                            EndTime = new DateTime(2023, 12, 26),
                            //DurationDays = 1,
                            BaseCharge = 2000.00,
                            PerPersonCharge = 50.00,
                            GuaranteedNumber = 40,
                            SOCAN = 50.00,
                            Deposit = 500.00,
                            DepositPaid = false,
                            NoHST = true,
                            NoGratuity = true,
                            Alcohol = false,
                            MealTypeID = context.MealTypes.FirstOrDefault(d => d.Name == "Gluten-Free").ID,
                            CustomerID = context.Customers.FirstOrDefault(c => c.FirstName == "Doogie" && c.LastName == "Houser").ID,
                            FunctionTypeID = context.FunctionTypes.FirstOrDefault(f => f.Name == "Presentation").ID
                        });

                    context.SaveChanges();
                }
                // Seed Rooms if there aren't any.
                if (!context.Rooms.Any())
                {
                    context.Rooms.AddRange(
                        new Room
                        {
                            Name = "Master Ballroom"
                        },
                        new Room
                        {
                            Name = "Large Banquet Hall"
                        },
                        new Room
                        {
                            Name = "Small Banquet Hall"
                        },
                        new Room
                        {
                            Name = "Boardroom"
                        },
                        new Room
                        {
                            Name = "Theatre Room"
                        },
                        new Room
                        {
                            Name = "Media Room"
                        });

                    context.SaveChanges();
                }
                //FunctionRooms
                if (!context.FunctionRooms.Any())
                {
                    context.FunctionRooms.AddRange(
                        new FunctionRoom
                        {
                            RoomID = context.Rooms.FirstOrDefault(c => c.Name == "Theatre Room").ID,
                            FunctionID = context.Functions.FirstOrDefault(p => p.Name == "Behind the Numbers: What’s Causing Growth in Food Prices").ID
                        },
                        new FunctionRoom
                        {
                            RoomID = context.Rooms.FirstOrDefault(c => c.Name == "Media Room").ID,
                            FunctionID = context.Functions.FirstOrDefault(p => p.Name == "Xavier Birthday Party").ID
                        },
                        new FunctionRoom
                        {
                            RoomID = context.Rooms.FirstOrDefault(c => c.Name == "Boardroom").ID,
                            FunctionID = context.Functions.FirstOrDefault(p => p.Name == "JPMorgan Chase Shareholders Meeting").ID
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
