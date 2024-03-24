namespace PersonalSiteMVCConversion.Models
{
    //The CredentialSettings class models the values in appsettings.json
    //We can name this class whatever we'd like -- CredentialSettings
    //is just a good descriptor for what this class does.
    public class CredentialSettings
    {
        //The property here must have a name that matches the 
        //key in the "Credentials" object in appsettings.json.
        //If we had more sets of credentials beyond Email, we could
        //add properties here for those as well.
        public Email Email { get; set; } = null!;
    }

    //This class below will be used to store the Values retrieved from
    //their corresponding Keys in the "Email" object in appsettings.json
    public class Email
    {
        //The property names here MUST match the keys
        public string Server { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Recipient { get; set; } = null!;
    }
}
