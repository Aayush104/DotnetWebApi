namespace Backend.Models
{
    public class EmailSetting
    {
        internal object _emailSetting;

        public string Email { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string DisplayName { get; set; }
        public int  Port { get; set; }
    }
}
