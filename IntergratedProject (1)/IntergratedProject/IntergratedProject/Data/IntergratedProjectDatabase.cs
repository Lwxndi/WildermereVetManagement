using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using IntergratedProject.Models;

namespace IntergratedProject.Data
{
    public class IntergratedProjectDatabase : DbContext
    {
        public IntergratedProjectDatabase(DbContextOptions<IntergratedProjectDatabase> options)
            : base(options)
        {
        }

        public DbSet<PetOwner> PetOwner { get; set; } = default!;
        public DbSet<Veterinarian> Veterinarian { get; set; } = default!;
        public DbSet<Admin> Admin { get; set; } = default!;
        public DbSet<Pet> Pet { get; set; } = default!;
        public DbSet<Species> Species { get; set; } = default!;
        public DbSet<Breed> Breed { get; set; } = default!;
        public DbSet<IntergratedProject.Models.Appointment> Appointment { get; set; } = default!;
        public DbSet<AppointmentType> AppointmentTypes { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Species
            modelBuilder.Entity<Species>().HasData(
                new Species { SpeciesId = 1, Name = "Dog" },
                new Species { SpeciesId = 2, Name = "Cat" },
                new Species { SpeciesId = 3, Name = "Horse" },
                new Species { SpeciesId = 4, Name = "Bird" },
                new Species { SpeciesId = 5, Name = "Reptile" },
                new Species { SpeciesId = 6, Name = "Rabbit & Pocket Pets" }
            );

            // Seed Breeds (Dogs)
            modelBuilder.Entity<Breed>().HasData(
                new Breed { BreedId = 1, Name = "Labrador Retriever", SpeciesId = 1 },
                new Breed { BreedId = 2, Name = "German Shepherd", SpeciesId = 1 },
                new Breed { BreedId = 3, Name = "Golden Retriever", SpeciesId = 1 },
                new Breed { BreedId = 4, Name = "Yorkshire Terrier", SpeciesId = 1 },
                new Breed { BreedId = 5, Name = "Bulldog", SpeciesId = 1 },
                new Breed { BreedId = 6, Name = "Jack Russell Terrier", SpeciesId = 1 },
                new Breed { BreedId = 7, Name = "Pomeranian", SpeciesId = 1 },
                new Breed { BreedId = 8, Name = "Rottweiler", SpeciesId = 1 },
                new Breed { BreedId = 9, Name = "Dachshund", SpeciesId = 1 },
                new Breed { BreedId = 10, Name = "Boxer", SpeciesId = 1 },
                new Breed { BreedId = 11, Name = "Chihuahua", SpeciesId = 1 },
                new Breed { BreedId = 12, Name = "Great Dane", SpeciesId = 1 },
                new Breed { BreedId = 13, Name = "French Bulldog", SpeciesId = 1 },
                new Breed { BreedId = 14, Name = "Staffordshire Terrier", SpeciesId = 1 },
                new Breed { BreedId = 15, Name = "Border Collie", SpeciesId = 1 },
                new Breed { BreedId = 16, Name = "Miniature Schnauzer", SpeciesId = 1 },
                new Breed { BreedId = 17, Name = "Cavalier King Charles Spaniel", SpeciesId = 1 },
                new Breed { BreedId = 18, Name = "Rhodesian Ridgeback", SpeciesId = 1 },

                // Cats
                new Breed { BreedId = 19, Name = "Domestic Shorthair", SpeciesId = 2 },
                new Breed { BreedId = 20, Name = "Siamese", SpeciesId = 2 },
                new Breed { BreedId = 21, Name = "Persian", SpeciesId = 2 },
                new Breed { BreedId = 22, Name = "Maine Coon", SpeciesId = 2 },
                new Breed { BreedId = 23, Name = "Bengal", SpeciesId = 2 },
                new Breed { BreedId = 24, Name = "Ragdoll", SpeciesId = 2 },
                new Breed { BreedId = 25, Name = "Sphynx", SpeciesId = 2 },
                new Breed { BreedId = 26, Name = "Russian Blue", SpeciesId = 2 },

                // Horses
                new Breed { BreedId = 27, Name = "Thoroughbred", SpeciesId = 3 },
                new Breed { BreedId = 28, Name = "Arabian", SpeciesId = 3 },
                new Breed { BreedId = 29, Name = "Boerperd", SpeciesId = 3 },
                new Breed { BreedId = 30, Name = "Friesian", SpeciesId = 3 },
                new Breed { BreedId = 31, Name = "Quarter Horse", SpeciesId = 3 },
                new Breed { BreedId = 32, Name = "Warmblood", SpeciesId = 3 },

                // Birds
                new Breed { BreedId = 33, Name = "Budgerigar", SpeciesId = 4 },
                new Breed { BreedId = 34, Name = "Cockatiel", SpeciesId = 4 },
                new Breed { BreedId = 35, Name = "African Grey Parrot", SpeciesId = 4 },
                new Breed { BreedId = 36, Name = "Lovebirds", SpeciesId = 4 },
                new Breed { BreedId = 37, Name = "Canaries", SpeciesId = 4 },
                new Breed { BreedId = 38, Name = "Macaws", SpeciesId = 4 },
                new Breed { BreedId = 39, Name = "Finches", SpeciesId = 4 },

                // Reptiles
                new Breed { BreedId = 40, Name = "Ball Python", SpeciesId = 5 },
                new Breed { BreedId = 41, Name = "Corn Snake", SpeciesId = 5 },
                new Breed { BreedId = 42, Name = "Bearded Dragon", SpeciesId = 5 },
                new Breed { BreedId = 43, Name = "Leopard Gecko", SpeciesId = 5 },
                new Breed { BreedId = 44, Name = "Chameleon", SpeciesId = 5 },
                new Breed { BreedId = 45, Name = "Leopard Tortoise", SpeciesId = 5 },
                new Breed { BreedId = 46, Name = "Sulcata Tortoise", SpeciesId = 5 },

                // Rabbits & Pocket Pets
                new Breed { BreedId = 47, Name = "Mini Lop Rabbit", SpeciesId = 6 },
                new Breed { BreedId = 48, Name = "Netherland Dwarf Rabbit", SpeciesId = 6 },
                new Breed { BreedId = 49, Name = "Hamsters", SpeciesId = 6 },
                new Breed { BreedId = 50, Name = "Guinea Pigs", SpeciesId = 6 },
                new Breed { BreedId = 51, Name = "Gerbils", SpeciesId = 6 },
                new Breed { BreedId = 52, Name = "Rats", SpeciesId = 6 },
                new Breed { BreedId = 53, Name = "Mice", SpeciesId = 6 },

                // Unknown breed example (BreedId = 54)
                new Breed { BreedId = 54, Name = "Unknown", SpeciesId = 1 } 

            );

            modelBuilder.Entity<AppointmentType>().HasData(
           // General Care
           new AppointmentType { AppointmentTypeId = 1, Name = "Routine Check-up", Price = 475, DurationMinutes = 30, SpecializationId = 1 },
           new AppointmentType { AppointmentTypeId = 2, Name = "Vaccination (Initial Package)", Price = 1500, DurationMinutes = 45, SpecializationId = 1 },
           new AppointmentType { AppointmentTypeId = 3, Name = "Vaccination (Annual Booster)", Price = 470, DurationMinutes = 20, SpecializationId = 1 },
           new AppointmentType { AppointmentTypeId = 4, Name = "Deworming", Price = 375, DurationMinutes = 15, SpecializationId = 1 },
           new AppointmentType { AppointmentTypeId = 5, Name = "Microchipping", Price = 450, DurationMinutes = 20, SpecializationId = 1 },
           new AppointmentType { AppointmentTypeId = 6, Name = "Health Certificate Issuance", Price = 550, DurationMinutes = 15, SpecializationId = 1 },

           // Preventive & Wellness
           new AppointmentType { AppointmentTypeId = 7, Name = "Puppy/Kitten Wellness Visit", Price = 650, DurationMinutes = 30, SpecializationId = 2 },
           new AppointmentType { AppointmentTypeId = 8, Name = "Senior Pet Wellness Exam", Price = 750, DurationMinutes = 40, SpecializationId = 2 },
           new AppointmentType { AppointmentTypeId = 9, Name = "Weight Management Consultation", Price = 550, DurationMinutes = 25, SpecializationId = 2 },
           new AppointmentType { AppointmentTypeId = 10, Name = "Nutritional Counseling", Price = 475, DurationMinutes = 30, SpecializationId = 2 },

           // Medical & Diagnostic
           new AppointmentType { AppointmentTypeId = 11, Name = "Sick Pet Consultation", Price = 475, DurationMinutes = 30, SpecializationId = 3 },
           new AppointmentType { AppointmentTypeId = 12, Name = "Follow-up Visit", Price = 400, DurationMinutes = 15, SpecializationId = 3 },
           new AppointmentType { AppointmentTypeId = 13, Name = "Blood Tests", Price = 800, DurationMinutes = 30, SpecializationId = 3 },
           new AppointmentType { AppointmentTypeId = 14, Name = "X-rays / Imaging", Price = 950, DurationMinutes = 45, SpecializationId = 3 },
           new AppointmentType { AppointmentTypeId = 15, Name = "Ultrasound", Price = 1500, DurationMinutes = 60, SpecializationId = 3 },
           new AppointmentType { AppointmentTypeId = 16, Name = "Allergy Testing", Price = 1150, DurationMinutes = 45, SpecializationId = 3 },
           new AppointmentType { AppointmentTypeId = 17, Name = "Skin/Fur Condition Evaluation", Price = 550, DurationMinutes = 20, SpecializationId = 3 },

           // Dental
           new AppointmentType { AppointmentTypeId = 18, Name = "Dental Check-up", Price = 550, DurationMinutes = 30, SpecializationId = 4 },
           new AppointmentType { AppointmentTypeId = 19, Name = "Teeth Cleaning", Price = 1750, DurationMinutes = 60, SpecializationId = 4 },
           new AppointmentType { AppointmentTypeId = 20, Name = "Tooth Extraction", Price = 2250, DurationMinutes = 90, SpecializationId = 4 },

           // Surgical
           new AppointmentType { AppointmentTypeId = 21, Name = "Spay/Neuter Surgery", Price = 2375, DurationMinutes = 90, SpecializationId = 5 },
           new AppointmentType { AppointmentTypeId = 22, Name = "Soft Tissue Surgery", Price = 5000, DurationMinutes = 120, SpecializationId = 5 },
           new AppointmentType { AppointmentTypeId = 23, Name = "Orthopedic Surgery", Price = 20000, DurationMinutes = 180, SpecializationId = 5 },
           new AppointmentType { AppointmentTypeId = 24, Name = "Emergency Surgery", Price = 10000, DurationMinutes = 120, SpecializationId = 5 },

           // Behavioral
           new AppointmentType { AppointmentTypeId = 25, Name = "Behavioral Consultation", Price = 700, DurationMinutes = 30, SpecializationId = 6 },
           new AppointmentType { AppointmentTypeId = 26, Name = "Anxiety/Stress Evaluation", Price = 600, DurationMinutes = 30, SpecializationId = 6 },

           // Farm Visits
           new AppointmentType { AppointmentTypeId = 27, Name = "Equine Check-up", Price = 900, DurationMinutes = 60, SpecializationId = 7 },
           new AppointmentType { AppointmentTypeId = 28, Name = "Livestock Vaccination", Price = 750, DurationMinutes = 45, SpecializationId = 7 },
           new AppointmentType { AppointmentTypeId = 29, Name = "On-site Farm Visit", Price = 1750, DurationMinutes = 90, SpecializationId = 7 },

           // Exotic Care
           new AppointmentType { AppointmentTypeId = 30, Name = "Bird Health Check", Price = 600, DurationMinutes = 20, SpecializationId = 8 },
           new AppointmentType { AppointmentTypeId = 31, Name = "Reptile Wellness Exam", Price = 700, DurationMinutes = 25, SpecializationId = 8 },
           new AppointmentType { AppointmentTypeId = 32, Name = "Small Mammal Consultation", Price = 550, DurationMinutes = 20, SpecializationId = 8 },

           // Therapy
           new AppointmentType { AppointmentTypeId = 33, Name = "Wound Care", Price = 750, DurationMinutes = 30, SpecializationId = 9 },
           new AppointmentType { AppointmentTypeId = 34, Name = "IV Fluid Therapy", Price = 1150, DurationMinutes = 45, SpecializationId = 9 },
           new AppointmentType { AppointmentTypeId = 35, Name = "Pain Management", Price = 650, DurationMinutes = 30, SpecializationId = 9 },
           new AppointmentType { AppointmentTypeId = 36, Name = "Post-op Follow-up", Price = 450, DurationMinutes = 15, SpecializationId = 9 }
       );
                    modelBuilder.Entity<Specialization>().HasData(
            new Specialization { SpecializationId = 1, Name = "General Care" },
            new Specialization { SpecializationId = 2, Name = "Preventive & Wellness" },
            new Specialization { SpecializationId = 3, Name = "Medical & Diagnostic" },
            new Specialization { SpecializationId = 4, Name = "Dental" },
            new Specialization { SpecializationId = 5, Name = "Surgical" },
            new Specialization { SpecializationId = 6, Name = "Behavioral" },
            new Specialization { SpecializationId = 7, Name = "Farm Visits" },
            new Specialization { SpecializationId = 8, Name = "Exotic Care" },
            new Specialization { SpecializationId = 9, Name = "Therapy" }
        );





            modelBuilder.Entity<Pet>()
                .HasOne(p => p.PetOwner)
                .WithMany(o => o.Pets)
                .HasForeignKey(p => p.PetOwnerId);

            // Seed PetOwners
            modelBuilder.Entity<PetOwner>().HasData(
               new PetOwner { OwnerId = 1, Name = "John", MiddleName = "A.", LastName = "Doe", Email = "john.doe@example.com", Address = "123 Main Street", PhoneNumber = "123-456-7890", Password = "Th0r@2025!", Role = "PetOwner" },

               new PetOwner { OwnerId = 2, Name = "Alice", MiddleName = "C.", LastName = "Johnson", Email = "alice.johnson@example.com", Address = "789 Oak Street", PhoneNumber = "321-654-0987", Password = "S@f3Haven88", Role = "PetOwner" },
               


               new PetOwner { OwnerId = 3, Name = "George", MiddleName = "I.", LastName = "Miller", Email = "george.miller@example.com", Address = "606 Chestnut Street", PhoneNumber = "666-555-4444", Password = "Geo#M2025!", Role = "PetOwner" },
               new PetOwner { OwnerId = 4, Name = "Ian", MiddleName = "K.", LastName = "Moore", Email = "ian.moore@example.com", Address = "808 Poplar Street", PhoneNumber = "888-999-0000", Password = "Ian!2025Safe", Role = "PetOwner" },
               new PetOwner { OwnerId = 5, Name = "Kevin", MiddleName = "M.", LastName = "Anderson", Email = "kevin.anderson@example.com", Address = "111 Dogwood Street", PhoneNumber = "123-321-1234", Password = "K3v!nPass!", Role = "PetOwner" },
               new PetOwner { OwnerId = 6, Name = "Jade", MiddleName = "O.", LastName = "Connect", Email = "jadeconnect765@gmail.com", Address = "333 Palm Street", PhoneNumber = "213-213-2134", Password = "Welc0me#123", Role = "PetOwner" },
               new PetOwner { OwnerId = 17, Name = "Arnold", MiddleName = "A.", LastName = "Schwarzenegger", Email = "arnold@example.com", Address = "1 Muscle St, LA", PhoneNumber = "072-222-2001", Password = "Arn0ld@2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 18, Name = "Sylvester", MiddleName = "J.", LastName = "Stallone", Email = "sylvester@example.com", Address = "2 Action Rd, LA", PhoneNumber = "072-222-2002", Password = "Sylv3st3r#2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 19, Name = "Bruce", MiddleName = "W.", LastName = "Willis", Email = "bruce@example.com", Address = "3 DieHard St, LA", PhoneNumber = "072-222-2003", Password = "Bruc3!2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 20, Name = "Jason", MiddleName = "S.", LastName = "Statham", Email = "jason@example.com", Address = "4 Fast Ln, UK", PhoneNumber = "072-222-2004", Password = "J@son2025#", Role = "PetOwner" },
                new PetOwner { OwnerId = 21, Name = "Dwayne", MiddleName = "J.", LastName = "Johnson", Email = "dwayne@example.com", Address = "5 Rock Rd, LA", PhoneNumber = "072-222-2005", Password = "Dwayn3@2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 22, Name = "Keanu", MiddleName = "C.", LastName = "Reeves", Email = "keanu@example.com", Address = "6 Matrix St, LA", PhoneNumber = "072-222-2006", Password = "Ke@nu2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 23, Name = "Jackie", MiddleName = "C.", LastName = "Chan", Email = "jackie@example.com", Address = "7 KungFu Ln, HK", PhoneNumber = "072-222-2007", Password = "Jack!e2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 24, Name = "Tom", MiddleName = "C.", LastName = "Cruise", Email = "tom@example.com", Address = "8 Mission St, LA", PhoneNumber = "072-222-2008", Password = "TomCrui$e2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 25, Name = "Jean-Claude", MiddleName = "V.", LastName = "Van Damme", Email = "jean@example.com", Address = "9 Kick Rd, BE", PhoneNumber = "072-222-2009", Password = "JCV@2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 26, Name = "Vin", MiddleName = "D.", LastName = "Diesel", Email = "vin@example.com", Address = "10 FastSt, LA", PhoneNumber = "072-222-2010", Password = "VinD!3sel2025", Role = "PetOwner" },

                // American Rappers (27–36)
                new PetOwner { OwnerId = 27, Name = "Jay", MiddleName = "Z.", LastName = "Carter", Email = "jayz@example.com", Address = "1 Roc Ave, NY", PhoneNumber = "073-333-3001", Password = "JayZ@2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 28, Name = "Kanye", MiddleName = "O.", LastName = "West", Email = "kanye@example.com", Address = "2 Yeezy Ln, LA", PhoneNumber = "073-333-3002", Password = "K@ny3#2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 29, Name = "Drake", MiddleName = "A.", LastName = "Graham", Email = "drake@example.com", Address = "3 OVO Rd, TO", PhoneNumber = "073-333-3003", Password = "Dr@k3!2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 30, Name = "Kendrick", MiddleName = "L.", LastName = "Lamar", Email = "kendrick@example.com", Address = "4 Compton St, LA", PhoneNumber = "073-333-3004", Password = "K3ndr!ck2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 31, Name = "Eminem", MiddleName = "M.", LastName = "Mathers", Email = "eminem@example.com", Address = "5 8Mile Rd, Detroit", PhoneNumber = "073-333-3005", Password = "Em!nem2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 32, Name = "Lil", MiddleName = "Wayne", LastName = "Carter", Email = "lilwayne@example.com", Address = "6 NOLA St, LA", PhoneNumber = "073-333-3006", Password = "LilWayn3@2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 33, Name = "50", MiddleName = "C.", LastName = "Cent", Email = "50cent@example.com", Address = "7 Power Rd, NY", PhoneNumber = "073-333-3007", Password = "F!fty2025$", Role = "PetOwner" },
                new PetOwner { OwnerId = 34, Name = "Nas", MiddleName = "A.", LastName = "Jones", Email = "nas@example.com", Address = "8 Illmatic Ln, NY", PhoneNumber = "073-333-3008", Password = "N@s2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 35, Name = "Snoop", MiddleName = "D.", LastName = "Doggy", Email = "snoop@example.com", Address = "9 LongBeach St, CA", PhoneNumber = "073-333-3009", Password = "Sn00p@2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 36, Name = "Travis", MiddleName = "S.", LastName = "Scott", Email = "travis@example.com", Address = "10 Astro Rd, TX", PhoneNumber = "073-333-3010", Password = "Trav!s2025$", Role = "PetOwner" },

                // Soccer Players (37–50)
                new PetOwner { OwnerId = 37, Name = "Cristiano", MiddleName = "R.", LastName = "Ronaldo", Email = "cristiano@example.com", Address = "1 Portugal St, PT", PhoneNumber = "074-444-4001", Password = "CR7@2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 38, Name = "Lionel", MiddleName = "A.", LastName = "Messi", Email = "messi@example.com", Address = "2 Argentina Ln, AR", PhoneNumber = "074-444-4002", Password = "M3ssi@2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 39, Name = "Neymar", MiddleName = "J.", LastName = "da Silva", Email = "neymar@example.com", Address = "3 Brazil Rd, BR", PhoneNumber = "074-444-4003", Password = "Neym@r2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 40, Name = "Kylian", MiddleName = "M.", LastName = "Mbappe", Email = "mbappe@example.com", Address = "4 France Ln, FR", PhoneNumber = "074-444-4004", Password = "Mb@pp3!2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 41, Name = "Mohamed", MiddleName = "S.", LastName = "Salah", Email = "salah@example.com", Address = "5 Egypt Rd, EG", PhoneNumber = "074-444-4005", Password = "S@lah2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 42, Name = "Harry", MiddleName = "K.", LastName = "Kane", Email = "kane@example.com", Address = "6 UK St, UK", PhoneNumber = "074-444-4006", Password = "H@rryK2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 43, Name = "Kevin", MiddleName = "D.", LastName = "De Bruyne", Email = "debruyne@example.com", Address = "7 Belgium Ln, BE", PhoneNumber = "074-444-4007", Password = "KDBruyn3@2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 44, Name = "Sergio", MiddleName = "R.", LastName = "Ramos", Email = "ramos@example.com", Address = "8 Spain Rd, ES", PhoneNumber = "074-444-4008", Password = "Ram0s!2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 45, Name = "Robert", MiddleName = "L.", LastName = "Lewandowski", Email = "lewandowski@example.com", Address = "9 Poland St, PL", PhoneNumber = "074-444-4009", Password = "Lewy@2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 46, Name = "Paulo", MiddleName = "D.", LastName = "Dybala", Email = "dybala@example.com", Address = "10 Argentina Ln, AR", PhoneNumber = "074-444-4010", Password = "Dybal@2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 47, Name = "Erling", MiddleName = "H.", LastName = "Haaland", Email = "haaland@example.com", Address = "11 Norway Rd, NO", PhoneNumber = "074-444-4011", Password = "Ha@land2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 48, Name = "Sadio", MiddleName = "M.", LastName = "Mane", Email = "mane@example.com", Address = "12 Senegal Ln, SN", PhoneNumber = "074-444-4012", Password = "M@ne2025!", Role = "PetOwner" },
                new PetOwner { OwnerId = 49, Name = "Paul", MiddleName = "P.", LastName = "Pogba", Email = "pogba@example.com", Address = "13 France Rd, FR", PhoneNumber = "074-444-4013", Password = "P0gba@2025", Role = "PetOwner" },
                new PetOwner { OwnerId = 50, Name = "Luka", MiddleName = "M.", LastName = "Modric", Email = "modric@example.com", Address = "14 Croatia Ln, HR", PhoneNumber = "074-444-4014", Password = "Luk@2025!", Role = "PetOwner" }
                 );

            // Seed Veterinarians
            // Seed Veterinarians with SpecializationId
            modelBuilder.Entity<Veterinarian>().HasData(
                new Veterinarian { VeterinarianId = 1, Name = "Fiona", LastName = "Garcia", Email = "fiona.garcia@example.com", PhoneNumber = "777-666-5555", Password = "F!onaSecure9", Role = "Veterinarian", SpecializationId = 1 }, // General Care
                new Veterinarian { VeterinarianId = 2, Name = "Hannah", LastName = "Wilson", Email = "hannah.wilson@example.com", PhoneNumber = "111-222-3333", Password = "HanN@h88!", Role = "Veterinarian", SpecializationId = 4 }, // Dental
                new Veterinarian { VeterinarianId = 3, Name = "Laura", LastName = "Thomas", Email = "laura.thomas@example.com", PhoneNumber = "321-123-4321", Password = "Laur@987!", Role = "Veterinarian", SpecializationId = 5 }, // Surgical
                new Veterinarian { VeterinarianId = 4, Name = "Alice", LastName = "Moyo", Email = "alice.moyo@example.com", PhoneNumber = "0712345671", Password = "Password@1", Role = "Veterinarian", SpecializationId = 2 }, // Preventive & Wellness
                new Veterinarian { VeterinarianId = 5, Name = "Brian", LastName = "Dlamini", Email = "brian.dlamini@example.com", PhoneNumber = "0712345672", Password = "Password@1", Role = "Veterinarian", SpecializationId = 3 }, // Medical & Diagnostic
                new Veterinarian { VeterinarianId = 6, Name = "Clara", LastName = "Ngcobo", Email = "clara.ngcobo@example.com", PhoneNumber = "0712345673", Password = "Password@1", Role = "Veterinarian", SpecializationId = 6 }, // Behavioral
                new Veterinarian { VeterinarianId = 7, Name = "David", LastName = "Zulu", Email = "david.zulu@example.com", PhoneNumber = "0712345674", Password = "Password@1", Role = "Veterinarian", SpecializationId = 7 }, // Farm Visits
                new Veterinarian { VeterinarianId = 8, Name = "Evelyn", LastName = "Khumalo", Email = "evelyn.khumalo@example.com", PhoneNumber = "0712345675", Password = "Password@1", Role = "Veterinarian", SpecializationId = 8 }, // Exotic Care
                new Veterinarian { VeterinarianId = 9, Name = "Frank", LastName = "Mthethwa", Email = "frank.mthethwa@example.com", PhoneNumber = "0712345676", Password = "Password@1", Role = "Veterinarian", SpecializationId = 9 }, // Therapy
                new Veterinarian { VeterinarianId = 10, Name = "Grace", LastName = "Nkosi", Email = "grace.nkosi@example.com", PhoneNumber = "0712345677", Password = "Password@1", Role = "Veterinarian", SpecializationId = 1 }, // General Care
                new Veterinarian { VeterinarianId = 11, Name = "Henry", LastName = "Sithole", Email = "henry.sithole@example.com", PhoneNumber = "0712345678", Password = "Password@1", Role = "Veterinarian", SpecializationId = 5 }, // Surgical
                new Veterinarian { VeterinarianId = 12, Name = "Ivy", LastName = "Ndlovu", Email = "ivy.ndlovu@example.com", PhoneNumber = "0712345679", Password = "Password@1", Role = "Veterinarian", SpecializationId = 2 }, // Preventive & Wellness
                new Veterinarian { VeterinarianId = 13, Name = "Jacob", LastName = "Mabena", Email = "jacob.mabena@example.com", PhoneNumber = "0712345680", Password = "Password@1", Role = "Veterinarian", SpecializationId = 3 }, // Medical & Diagnostic
                new Veterinarian { VeterinarianId = 14, Name = "Lindiwe", LastName = "Mabaso", Email = "lindiwe.mabaso@example.com", PhoneNumber = "0712345682", Password = "Password@1", Role = "Veterinarian", SpecializationId = 4 }, // Dental
                new Veterinarian { VeterinarianId = 15, Name = "Musa", LastName = "Dube", Email = "musa.dube@example.com", PhoneNumber = "0712345683", Password = "Password@1", Role = "Veterinarian", SpecializationId = 6 }  // Behavioral
            );



            // Seed Admins
            modelBuilder.Entity<Admin>().HasData(
                new Admin { AdminId = 1, FullName = "Jane B. Smith", Email = "jane.smith@example.com", Password = "Welc0me#123", Role = "Admin" },
                new Admin { AdminId = 2, FullName = "Julia L. Taylor", Email = "Admin@gmail.com", Password = "Admin@01Pass", Role = "Admin" },
                new Admin { AdminId = 3, FullName = " Michael C. Jordan", Email = "admin@gmail.com", Password = "Admin@01Pass", Role = "Admin" }
            );

            // Configure precision for AppointmentType Price
            modelBuilder.Entity<AppointmentType>()
               .Property(a => a.Price)
               .HasPrecision(18, 2);

            // PetOwner → Pet (Cascade)
            modelBuilder.Entity<Pet>()
                .HasOne(p => p.PetOwner)
                .WithMany(o => o.Pets)
                .HasForeignKey(p => p.PetOwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // PetOwner → Appointment (Cascade)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.PetOwner)
                .WithMany(o => o.Appointments)
                .HasForeignKey(a => a.PetOwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Pet → Appointment (Restrict) - prevents cascade path error
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Pet)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            // Veterinarian → Appointment (Cascade or Restrict as per your needs)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Veterinarian)
                .WithMany(v => v.Appointments)
                .HasForeignKey(a => a.VeterinarianId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppointmentType → Appointment (Cascade or Restrict as per your needs)
            modelBuilder.Entity<Appointment>()
              .HasOne(a => a.AppointmentType)
              .WithMany(at => at.Appointments)
              .HasForeignKey(a => a.AppointmentTypeId)
              .OnDelete(DeleteBehavior.Cascade);


            ///
            modelBuilder.Entity<VeterinarianSpecialization>()
       .HasKey(vs => new { vs.VeterinarianId, vs.SpecializationId });

            modelBuilder.Entity<Veterinarian>()
     .HasOne(v => v.Specialization)
     .WithMany() 
     .HasForeignKey(v => v.SpecializationId)
     .OnDelete(DeleteBehavior.Restrict); // prevents multiple cascade paths


            modelBuilder.Entity<VeterinarianSpecialization>()
                .HasOne(vs => vs.Specialization)
                .WithMany(s => s.Veterinarians)
                .HasForeignKey(vs => vs.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict); // avoids multiple cascade paths

            // AppointmentType -> Specialization
            modelBuilder.Entity<AppointmentType>()
                .HasOne(at => at.Specialization)
                .WithMany(s => s.AppointmentTypes)
                .HasForeignKey(at => at.SpecializationId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MedicalRecord>()
                .HasMany(m => m.Files)
                .WithOne(f => f.MedicalRecord)
                .HasForeignKey(f => f.MedicalRecordId)
                .OnDelete(DeleteBehavior.Restrict);



        }
        public DbSet<IntergratedProject.Models.MedicalRecord> MedicalRecord { get; set; } = default!;
        public DbSet<IntergratedProject.Models.MedicalRecordAuditTrail> MedicalRecordAuditTrail { get; set; } = default!;
        public DbSet<IntergratedProject.Models.MedicalRecordFile> MedicalRecordFile { get; set; } = default!;
        public DbSet<IntergratedProject.Models.Notification> Notification { get; set; } = default!;
        public DbSet<IntergratedProject.Models.Payment> Payment { get; set; } = default!;
    }
}
