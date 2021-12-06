namespace LightFeatherWebAPI.DataModels
{
    public class Supervisor
    {
        public string? FirstName { get; set; }   
        public string? LastName { get; set; }
        public string? Jurisdiction { get; set; }
        public override string ToString()
        {
            return $"FirstName:\t{FirstName}\n" +
                $"LastName:\t{LastName}\n" +
                $"Jurisdiction:\t{Jurisdiction}";
        }
    }
}
