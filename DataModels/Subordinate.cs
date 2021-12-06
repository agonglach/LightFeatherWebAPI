namespace LightFeatherWebAPI.DataModels
{
    public class Subordinate
    {
        public string? FirstName { get; set; }   
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Supervisor? Supervisor { get; set; }

        public override string ToString()
        {
            return $"Subordinate Information\n" +
                                $"First Name:\t{FirstName}\n" +
                                $"Last Name:\t{LastName}\n" +
                                $"Email:\t\t{Email}\n" +
                                $"Phone Number:\t{PhoneNumber}\n" +
                                $"Supervisor Information\n" +
                                $"{Supervisor.ToString()}";
        }
    }
}
