using System;

namespace ContosoUniversity.API.DTO
{
    public class Instructor
    {
        public int ID { get; set; }

        public required string LastName { get; set; }

        public required string FirstName { get; set; }

        public DateTime HireDate { get; set; }
    }
}