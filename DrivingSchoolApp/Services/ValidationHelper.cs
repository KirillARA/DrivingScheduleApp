using System.Text.RegularExpressions;

namespace DrivingSchoolApp.Services
{
    public static class ValidationHelper
    {
        
        private static readonly Regex RussianTextRegex = new(@"^[а-яА-Я\s\-\.]+$", RegexOptions.Compiled);

        
        private static readonly Regex NameWithDigitsRegex = new(@"^[a-zA-Zа-яА-Я0-9\s\-\.]+$", RegexOptions.Compiled);

        private static readonly Regex PassportRegex = new(@"^\d{4} \d{6}$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^(\+7|8)[\s\-\(]?\d{3}[\s\-\)]?\d{3}[\s\-]?\d{2}[\s\-]?\d{2}$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        
        public static readonly HashSet<string> ValidEmployeeRoles = new() { "администратор", "преподаватель", "инструктор" };
        
        public static readonly HashSet<string> ValidExamTypes = new() { "теория", "вождение" };
        
        public static readonly HashSet<string> ValidGroupStatuses = new() { "forming", "studying", "graduated" };

        public static bool IsValidRussianText(string? input) => !string.IsNullOrWhiteSpace(input) && RussianTextRegex.IsMatch(input);
        public static bool IsValidNameWithDigits(string? input) => !string.IsNullOrWhiteSpace(input) && NameWithDigitsRegex.IsMatch(input);
        public static bool IsValidPassport(string input) => !string.IsNullOrWhiteSpace(input) && PassportRegex.IsMatch(input);
        public static bool IsValidPhone(string input) => !string.IsNullOrWhiteSpace(input) && PhoneRegex.IsMatch(input);
        public static bool IsValidEmail(string input) => !string.IsNullOrWhiteSpace(input) && EmailRegex.IsMatch(input);
    }
}
