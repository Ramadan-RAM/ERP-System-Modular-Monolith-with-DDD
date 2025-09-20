namespace ERPSys.SharedKernel.Domain
{
    /// <summary>
    /// EN: Abstract person entity (HR base), inherits int Id base entity.
    /// </summary>
    public abstract class Person : BaseEntity<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string MiddelName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty; // Male / Female

        public string? PicturePath { get; set; } // relative path
        public string? CVPath { get; set; }      // relative path

        public bool IsDeleted { get; set; } = false;

        public void SetName(string firstName, string middelName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(middelName) ||
                string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentNullException("Invalid Name");

            FirstName = char.ToUpper(firstName[0]) + firstName.Substring(1);
            MiddelName = char.ToUpper(middelName[0]) + middelName.Substring(1);
            LastName = char.ToUpper(lastName[0]) + lastName.Substring(1);
        }

        public void SetBirthDate(DateTime birthDate)
        {
            if (birthDate < new DateTime(1950, 1, 1))
                throw new ArgumentNullException("Invalid BirthDate");
            DateOfBirth = birthDate;
        }
    }
}
