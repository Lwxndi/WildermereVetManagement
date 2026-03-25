using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IntergratedProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.AdminId);
                });

            migrationBuilder.CreateTable(
                name: "PetOwner",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetOwner", x => x.OwnerId);
                });

            migrationBuilder.CreateTable(
                name: "Specialization",
                columns: table => new
                {
                    SpecializationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialization", x => x.SpecializationId);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.SpeciesId);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentTypes",
                columns: table => new
                {
                    AppointmentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentTypes", x => x.AppointmentTypeId);
                    table.ForeignKey(
                        name: "FK_AppointmentTypes_Specialization_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specialization",
                        principalColumn: "SpecializationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Veterinarian",
                columns: table => new
                {
                    VeterinarianId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veterinarian", x => x.VeterinarianId);
                    table.ForeignKey(
                        name: "FK_Veterinarian_Specialization_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specialization",
                        principalColumn: "SpecializationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Breed",
                columns: table => new
                {
                    BreedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpeciesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breed", x => x.BreedId);
                    table.ForeignKey(
                        name: "FK_Breed_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VeterinarianSpecialization",
                columns: table => new
                {
                    VeterinarianId = table.Column<int>(type: "int", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeterinarianSpecialization", x => new { x.VeterinarianId, x.SpecializationId });
                    table.ForeignKey(
                        name: "FK_VeterinarianSpecialization_Specialization_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specialization",
                        principalColumn: "SpecializationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VeterinarianSpecialization_Veterinarian_VeterinarianId",
                        column: x => x.VeterinarianId,
                        principalTable: "Veterinarian",
                        principalColumn: "VeterinarianId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VetShift",
                columns: table => new
                {
                    VetShiftId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeterinarianId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VetShift", x => x.VetShiftId);
                    table.ForeignKey(
                        name: "FK_VetShift_Veterinarian_VeterinarianId",
                        column: x => x.VeterinarianId,
                        principalTable: "Veterinarian",
                        principalColumn: "VeterinarianId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pet",
                columns: table => new
                {
                    PetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PetName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpeciesId = table.Column<int>(type: "int", nullable: false),
                    BreedId = table.Column<int>(type: "int", nullable: true),
                    AgeYears = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeightKg = table.Column<double>(type: "float", nullable: true),
                    HeightCm = table.Column<double>(type: "float", nullable: true),
                    ColorMarkings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PetOwnerId = table.Column<int>(type: "int", nullable: true),
                    PrimaryVeterinarianId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pet", x => x.PetId);
                    table.ForeignKey(
                        name: "FK_Pet_Breed_BreedId",
                        column: x => x.BreedId,
                        principalTable: "Breed",
                        principalColumn: "BreedId");
                    table.ForeignKey(
                        name: "FK_Pet_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pet_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pet_Veterinarian_PrimaryVeterinarianId",
                        column: x => x.PrimaryVeterinarianId,
                        principalTable: "Veterinarian",
                        principalColumn: "VeterinarianId");
                });

            migrationBuilder.CreateTable(
                name: "VetBreak",
                columns: table => new
                {
                    VetBreakId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VeterinarianId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    VetShiftId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VetBreak", x => x.VetBreakId);
                    table.ForeignKey(
                        name: "FK_VetBreak_VetShift_VetShiftId",
                        column: x => x.VetShiftId,
                        principalTable: "VetShift",
                        principalColumn: "VetShiftId");
                    table.ForeignKey(
                        name: "FK_VetBreak_Veterinarian_VeterinarianId",
                        column: x => x.VeterinarianId,
                        principalTable: "Veterinarian",
                        principalColumn: "VeterinarianId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    VeterinarianId = table.Column<int>(type: "int", nullable: true),
                    AppointmentTypeId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.AppointmentId);
                    table.ForeignKey(
                        name: "FK_Appointment_AppointmentTypes_AppointmentTypeId",
                        column: x => x.AppointmentTypeId,
                        principalTable: "AppointmentTypes",
                        principalColumn: "AppointmentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointment_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointment_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointment_Veterinarian_VeterinarianId",
                        column: x => x.VeterinarianId,
                        principalTable: "Veterinarian",
                        principalColumn: "VeterinarianId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    VeterinarianId = table.Column<int>(type: "int", nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: true),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Treatment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentId");
                    table.ForeignKey(
                        name: "FK_MedicalRecord_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_Veterinarian_VeterinarianId",
                        column: x => x.VeterinarianId,
                        principalTable: "Veterinarian",
                        principalColumn: "VeterinarianId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecordAuditTrail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalRecordId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedByUserId = table.Column<int>(type: "int", nullable: false),
                    PerformedByRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeData = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecordAuditTrail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecordAuditTrail_MedicalRecord_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecordFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicalRecordId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadedByVetId = table.Column<int>(type: "int", nullable: false),
                    UploadedByName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecordFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalRecordFile_MedicalRecord_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    RecipientRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: true),
                    VeterinarianId = table.Column<int>(type: "int", nullable: true),
                    AdminId = table.Column<int>(type: "int", nullable: true),
                    AppointmentId = table.Column<int>(type: "int", nullable: true),
                    MedicalRecordId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointment",
                        principalColumn: "AppointmentId");
                    table.ForeignKey(
                        name: "FK_Notification_MedicalRecord_MedicalRecordId",
                        column: x => x.MedicalRecordId,
                        principalTable: "MedicalRecord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notification_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "OwnerId");
                    table.ForeignKey(
                        name: "FK_Notification_Veterinarian_VeterinarianId",
                        column: x => x.VeterinarianId,
                        principalTable: "Veterinarian",
                        principalColumn: "VeterinarianId");
                });

            migrationBuilder.InsertData(
                table: "Admin",
                columns: new[] { "AdminId", "Email", "FullName", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "jane.smith@example.com", "Jane B. Smith", "Welc0me#123", "Admin" },
                    { 2, "Admin@gmail.com", "Julia L. Taylor", "Admin@01Pass", "Admin" },
                    { 3, "admin@gmail.com", " Michael C. Jordan", "Admin@01Pass", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "PetOwner",
                columns: new[] { "OwnerId", "Address", "Email", "LastName", "MiddleName", "Name", "Password", "PhoneNumber", "Role" },
                values: new object[,]
                {
                    { 1, "123 Main Street", "john.doe@example.com", "Doe", "A.", "John", "Th0r@2025!", "123-456-7890", "PetOwner" },
                    { 2, "789 Oak Street", "alice.johnson@example.com", "Johnson", "C.", "Alice", "S@f3Haven88", "321-654-0987", "PetOwner" },
                    { 3, "606 Chestnut Street", "george.miller@example.com", "Miller", "I.", "George", "Geo#M2025!", "666-555-4444", "PetOwner" },
                    { 4, "808 Poplar Street", "ian.moore@example.com", "Moore", "K.", "Ian", "Ian!2025Safe", "888-999-0000", "PetOwner" },
                    { 5, "111 Dogwood Street", "kevin.anderson@example.com", "Anderson", "M.", "Kevin", "K3v!nPass!", "123-321-1234", "PetOwner" },
                    { 6, "333 Palm Street", "jadeconnect765@gmail.com", "Connect", "O.", "Jade", "Welc0me#123", "213-213-2134", "PetOwner" },
                    { 17, "1 Muscle St, LA", "arnold@example.com", "Schwarzenegger", "A.", "Arnold", "Arn0ld@2025", "072-222-2001", "PetOwner" },
                    { 18, "2 Action Rd, LA", "sylvester@example.com", "Stallone", "J.", "Sylvester", "Sylv3st3r#2025", "072-222-2002", "PetOwner" },
                    { 19, "3 DieHard St, LA", "bruce@example.com", "Willis", "W.", "Bruce", "Bruc3!2025", "072-222-2003", "PetOwner" },
                    { 20, "4 Fast Ln, UK", "jason@example.com", "Statham", "S.", "Jason", "J@son2025#", "072-222-2004", "PetOwner" },
                    { 21, "5 Rock Rd, LA", "dwayne@example.com", "Johnson", "J.", "Dwayne", "Dwayn3@2025", "072-222-2005", "PetOwner" },
                    { 22, "6 Matrix St, LA", "keanu@example.com", "Reeves", "C.", "Keanu", "Ke@nu2025!", "072-222-2006", "PetOwner" },
                    { 23, "7 KungFu Ln, HK", "jackie@example.com", "Chan", "C.", "Jackie", "Jack!e2025", "072-222-2007", "PetOwner" },
                    { 24, "8 Mission St, LA", "tom@example.com", "Cruise", "C.", "Tom", "TomCrui$e2025", "072-222-2008", "PetOwner" },
                    { 25, "9 Kick Rd, BE", "jean@example.com", "Van Damme", "V.", "Jean-Claude", "JCV@2025!", "072-222-2009", "PetOwner" },
                    { 26, "10 FastSt, LA", "vin@example.com", "Diesel", "D.", "Vin", "VinD!3sel2025", "072-222-2010", "PetOwner" },
                    { 27, "1 Roc Ave, NY", "jayz@example.com", "Carter", "Z.", "Jay", "JayZ@2025!", "073-333-3001", "PetOwner" },
                    { 28, "2 Yeezy Ln, LA", "kanye@example.com", "West", "O.", "Kanye", "K@ny3#2025", "073-333-3002", "PetOwner" },
                    { 29, "3 OVO Rd, TO", "drake@example.com", "Graham", "A.", "Drake", "Dr@k3!2025", "073-333-3003", "PetOwner" },
                    { 30, "4 Compton St, LA", "kendrick@example.com", "Lamar", "L.", "Kendrick", "K3ndr!ck2025", "073-333-3004", "PetOwner" },
                    { 31, "5 8Mile Rd, Detroit", "eminem@example.com", "Mathers", "M.", "Eminem", "Em!nem2025", "073-333-3005", "PetOwner" },
                    { 32, "6 NOLA St, LA", "lilwayne@example.com", "Carter", "Wayne", "Lil", "LilWayn3@2025", "073-333-3006", "PetOwner" },
                    { 33, "7 Power Rd, NY", "50cent@example.com", "Cent", "C.", "50", "F!fty2025$", "073-333-3007", "PetOwner" },
                    { 34, "8 Illmatic Ln, NY", "nas@example.com", "Jones", "A.", "Nas", "N@s2025!", "073-333-3008", "PetOwner" },
                    { 35, "9 LongBeach St, CA", "snoop@example.com", "Doggy", "D.", "Snoop", "Sn00p@2025", "073-333-3009", "PetOwner" },
                    { 36, "10 Astro Rd, TX", "travis@example.com", "Scott", "S.", "Travis", "Trav!s2025$", "073-333-3010", "PetOwner" },
                    { 37, "1 Portugal St, PT", "cristiano@example.com", "Ronaldo", "R.", "Cristiano", "CR7@2025!", "074-444-4001", "PetOwner" },
                    { 38, "2 Argentina Ln, AR", "messi@example.com", "Messi", "A.", "Lionel", "M3ssi@2025", "074-444-4002", "PetOwner" },
                    { 39, "3 Brazil Rd, BR", "neymar@example.com", "da Silva", "J.", "Neymar", "Neym@r2025!", "074-444-4003", "PetOwner" },
                    { 40, "4 France Ln, FR", "mbappe@example.com", "Mbappe", "M.", "Kylian", "Mb@pp3!2025", "074-444-4004", "PetOwner" },
                    { 41, "5 Egypt Rd, EG", "salah@example.com", "Salah", "S.", "Mohamed", "S@lah2025!", "074-444-4005", "PetOwner" },
                    { 42, "6 UK St, UK", "kane@example.com", "Kane", "K.", "Harry", "H@rryK2025", "074-444-4006", "PetOwner" },
                    { 43, "7 Belgium Ln, BE", "debruyne@example.com", "De Bruyne", "D.", "Kevin", "KDBruyn3@2025", "074-444-4007", "PetOwner" },
                    { 44, "8 Spain Rd, ES", "ramos@example.com", "Ramos", "R.", "Sergio", "Ram0s!2025", "074-444-4008", "PetOwner" },
                    { 45, "9 Poland St, PL", "lewandowski@example.com", "Lewandowski", "L.", "Robert", "Lewy@2025!", "074-444-4009", "PetOwner" },
                    { 46, "10 Argentina Ln, AR", "dybala@example.com", "Dybala", "D.", "Paulo", "Dybal@2025!", "074-444-4010", "PetOwner" },
                    { 47, "11 Norway Rd, NO", "haaland@example.com", "Haaland", "H.", "Erling", "Ha@land2025", "074-444-4011", "PetOwner" },
                    { 48, "12 Senegal Ln, SN", "mane@example.com", "Mane", "M.", "Sadio", "M@ne2025!", "074-444-4012", "PetOwner" },
                    { 49, "13 France Rd, FR", "pogba@example.com", "Pogba", "P.", "Paul", "P0gba@2025", "074-444-4013", "PetOwner" },
                    { 50, "14 Croatia Ln, HR", "modric@example.com", "Modric", "M.", "Luka", "Luk@2025!", "074-444-4014", "PetOwner" }
                });

            migrationBuilder.InsertData(
                table: "Specialization",
                columns: new[] { "SpecializationId", "Name" },
                values: new object[,]
                {
                    { 1, "General Care" },
                    { 2, "Preventive & Wellness" },
                    { 3, "Medical & Diagnostic" },
                    { 4, "Dental" },
                    { 5, "Surgical" },
                    { 6, "Behavioral" },
                    { 7, "Farm Visits" },
                    { 8, "Exotic Care" },
                    { 9, "Therapy" }
                });

            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "SpeciesId", "Name" },
                values: new object[,]
                {
                    { 1, "Dog" },
                    { 2, "Cat" },
                    { 3, "Horse" },
                    { 4, "Bird" },
                    { 5, "Reptile" },
                    { 6, "Rabbit & Pocket Pets" }
                });

            migrationBuilder.InsertData(
                table: "AppointmentTypes",
                columns: new[] { "AppointmentTypeId", "DurationMinutes", "Name", "Price", "SpecializationId" },
                values: new object[,]
                {
                    { 1, 30, "Routine Check-up", 475m, 1 },
                    { 2, 45, "Vaccination (Initial Package)", 1500m, 1 },
                    { 3, 20, "Vaccination (Annual Booster)", 470m, 1 },
                    { 4, 15, "Deworming", 375m, 1 },
                    { 5, 20, "Microchipping", 450m, 1 },
                    { 6, 15, "Health Certificate Issuance", 550m, 1 },
                    { 7, 30, "Puppy/Kitten Wellness Visit", 650m, 2 },
                    { 8, 40, "Senior Pet Wellness Exam", 750m, 2 },
                    { 9, 25, "Weight Management Consultation", 550m, 2 },
                    { 10, 30, "Nutritional Counseling", 475m, 2 },
                    { 11, 30, "Sick Pet Consultation", 475m, 3 },
                    { 12, 15, "Follow-up Visit", 400m, 3 },
                    { 13, 30, "Blood Tests", 800m, 3 },
                    { 14, 45, "X-rays / Imaging", 950m, 3 },
                    { 15, 60, "Ultrasound", 1500m, 3 },
                    { 16, 45, "Allergy Testing", 1150m, 3 },
                    { 17, 20, "Skin/Fur Condition Evaluation", 550m, 3 },
                    { 18, 30, "Dental Check-up", 550m, 4 },
                    { 19, 60, "Teeth Cleaning", 1750m, 4 },
                    { 20, 90, "Tooth Extraction", 2250m, 4 },
                    { 21, 90, "Spay/Neuter Surgery", 2375m, 5 },
                    { 22, 120, "Soft Tissue Surgery", 5000m, 5 },
                    { 23, 180, "Orthopedic Surgery", 20000m, 5 },
                    { 24, 120, "Emergency Surgery", 10000m, 5 },
                    { 25, 30, "Behavioral Consultation", 700m, 6 },
                    { 26, 30, "Anxiety/Stress Evaluation", 600m, 6 },
                    { 27, 60, "Equine Check-up", 900m, 7 },
                    { 28, 45, "Livestock Vaccination", 750m, 7 },
                    { 29, 90, "On-site Farm Visit", 1750m, 7 },
                    { 30, 20, "Bird Health Check", 600m, 8 },
                    { 31, 25, "Reptile Wellness Exam", 700m, 8 },
                    { 32, 20, "Small Mammal Consultation", 550m, 8 },
                    { 33, 30, "Wound Care", 750m, 9 },
                    { 34, 45, "IV Fluid Therapy", 1150m, 9 },
                    { 35, 30, "Pain Management", 650m, 9 },
                    { 36, 15, "Post-op Follow-up", 450m, 9 }
                });

            migrationBuilder.InsertData(
                table: "Breed",
                columns: new[] { "BreedId", "Name", "SpeciesId" },
                values: new object[,]
                {
                    { 1, "Labrador Retriever", 1 },
                    { 2, "German Shepherd", 1 },
                    { 3, "Golden Retriever", 1 },
                    { 4, "Yorkshire Terrier", 1 },
                    { 5, "Bulldog", 1 },
                    { 6, "Jack Russell Terrier", 1 },
                    { 7, "Pomeranian", 1 },
                    { 8, "Rottweiler", 1 },
                    { 9, "Dachshund", 1 },
                    { 10, "Boxer", 1 },
                    { 11, "Chihuahua", 1 },
                    { 12, "Great Dane", 1 },
                    { 13, "French Bulldog", 1 },
                    { 14, "Staffordshire Terrier", 1 },
                    { 15, "Border Collie", 1 },
                    { 16, "Miniature Schnauzer", 1 },
                    { 17, "Cavalier King Charles Spaniel", 1 },
                    { 18, "Rhodesian Ridgeback", 1 },
                    { 19, "Domestic Shorthair", 2 },
                    { 20, "Siamese", 2 },
                    { 21, "Persian", 2 },
                    { 22, "Maine Coon", 2 },
                    { 23, "Bengal", 2 },
                    { 24, "Ragdoll", 2 },
                    { 25, "Sphynx", 2 },
                    { 26, "Russian Blue", 2 },
                    { 27, "Thoroughbred", 3 },
                    { 28, "Arabian", 3 },
                    { 29, "Boerperd", 3 },
                    { 30, "Friesian", 3 },
                    { 31, "Quarter Horse", 3 },
                    { 32, "Warmblood", 3 },
                    { 33, "Budgerigar", 4 },
                    { 34, "Cockatiel", 4 },
                    { 35, "African Grey Parrot", 4 },
                    { 36, "Lovebirds", 4 },
                    { 37, "Canaries", 4 },
                    { 38, "Macaws", 4 },
                    { 39, "Finches", 4 },
                    { 40, "Ball Python", 5 },
                    { 41, "Corn Snake", 5 },
                    { 42, "Bearded Dragon", 5 },
                    { 43, "Leopard Gecko", 5 },
                    { 44, "Chameleon", 5 },
                    { 45, "Leopard Tortoise", 5 },
                    { 46, "Sulcata Tortoise", 5 },
                    { 47, "Mini Lop Rabbit", 6 },
                    { 48, "Netherland Dwarf Rabbit", 6 },
                    { 49, "Hamsters", 6 },
                    { 50, "Guinea Pigs", 6 },
                    { 51, "Gerbils", 6 },
                    { 52, "Rats", 6 },
                    { 53, "Mice", 6 },
                    { 54, "Unknown", 1 }
                });

            migrationBuilder.InsertData(
                table: "Veterinarian",
                columns: new[] { "VeterinarianId", "Email", "LastName", "Name", "Password", "PhoneNumber", "Role", "SpecializationId" },
                values: new object[,]
                {
                    { 1, "fiona.garcia@example.com", "Garcia", "Fiona", "F!onaSecure9", "777-666-5555", "Veterinarian", 1 },
                    { 2, "hannah.wilson@example.com", "Wilson", "Hannah", "HanN@h88!", "111-222-3333", "Veterinarian", 4 },
                    { 3, "laura.thomas@example.com", "Thomas", "Laura", "Laur@987!", "321-123-4321", "Veterinarian", 5 },
                    { 4, "alice.moyo@example.com", "Moyo", "Alice", "Password@1", "0712345671", "Veterinarian", 2 },
                    { 5, "brian.dlamini@example.com", "Dlamini", "Brian", "Password@1", "0712345672", "Veterinarian", 3 },
                    { 6, "clara.ngcobo@example.com", "Ngcobo", "Clara", "Password@1", "0712345673", "Veterinarian", 6 },
                    { 7, "david.zulu@example.com", "Zulu", "David", "Password@1", "0712345674", "Veterinarian", 7 },
                    { 8, "evelyn.khumalo@example.com", "Khumalo", "Evelyn", "Password@1", "0712345675", "Veterinarian", 8 },
                    { 9, "frank.mthethwa@example.com", "Mthethwa", "Frank", "Password@1", "0712345676", "Veterinarian", 9 },
                    { 10, "grace.nkosi@example.com", "Nkosi", "Grace", "Password@1", "0712345677", "Veterinarian", 1 },
                    { 11, "henry.sithole@example.com", "Sithole", "Henry", "Password@1", "0712345678", "Veterinarian", 5 },
                    { 12, "ivy.ndlovu@example.com", "Ndlovu", "Ivy", "Password@1", "0712345679", "Veterinarian", 2 },
                    { 13, "jacob.mabena@example.com", "Mabena", "Jacob", "Password@1", "0712345680", "Veterinarian", 3 },
                    { 14, "lindiwe.mabaso@example.com", "Mabaso", "Lindiwe", "Password@1", "0712345682", "Veterinarian", 4 },
                    { 15, "musa.dube@example.com", "Dube", "Musa", "Password@1", "0712345683", "Veterinarian", 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_AppointmentTypeId",
                table: "Appointment",
                column: "AppointmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PetId",
                table: "Appointment",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PetOwnerId",
                table: "Appointment",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_VeterinarianId",
                table: "Appointment",
                column: "VeterinarianId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentTypes_SpecializationId",
                table: "AppointmentTypes",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Breed_SpeciesId",
                table: "Breed",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_AppointmentId",
                table: "MedicalRecord",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_PetId",
                table: "MedicalRecord",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_VeterinarianId",
                table: "MedicalRecord",
                column: "VeterinarianId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecordAuditTrail_MedicalRecordId",
                table: "MedicalRecordAuditTrail",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecordFile_MedicalRecordId",
                table: "MedicalRecordFile",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AppointmentId",
                table: "Notification",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_MedicalRecordId",
                table: "Notification",
                column: "MedicalRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_PetOwnerId",
                table: "Notification",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_VeterinarianId",
                table: "Notification",
                column: "VeterinarianId");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_BreedId",
                table: "Pet",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_PetOwnerId",
                table: "Pet",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_PrimaryVeterinarianId",
                table: "Pet",
                column: "PrimaryVeterinarianId");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_SpeciesId",
                table: "Pet",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_VetBreak_VeterinarianId",
                table: "VetBreak",
                column: "VeterinarianId");

            migrationBuilder.CreateIndex(
                name: "IX_VetBreak_VetShiftId",
                table: "VetBreak",
                column: "VetShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Veterinarian_SpecializationId",
                table: "Veterinarian",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_VeterinarianSpecialization_SpecializationId",
                table: "VeterinarianSpecialization",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_VetShift_VeterinarianId",
                table: "VetShift",
                column: "VeterinarianId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "MedicalRecordAuditTrail");

            migrationBuilder.DropTable(
                name: "MedicalRecordFile");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "VetBreak");

            migrationBuilder.DropTable(
                name: "VeterinarianSpecialization");

            migrationBuilder.DropTable(
                name: "MedicalRecord");

            migrationBuilder.DropTable(
                name: "VetShift");

            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "AppointmentTypes");

            migrationBuilder.DropTable(
                name: "Pet");

            migrationBuilder.DropTable(
                name: "Breed");

            migrationBuilder.DropTable(
                name: "PetOwner");

            migrationBuilder.DropTable(
                name: "Veterinarian");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropTable(
                name: "Specialization");
        }
    }
}
