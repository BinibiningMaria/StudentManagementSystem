namespace StudentManagementSystem.Features.Helpers;

public static class PersonNameHelper
{
    public static string BuildFullName(string firstName, string middleName, string surname, string suffix)
    {
        return string.Join(" ", new[] { firstName, middleName, surname, suffix }
            .Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    public static string BuildInstructorHonorific(string gender, string civilStatus)
    {
        if (string.Equals(gender, "Male", StringComparison.OrdinalIgnoreCase))
        {
            return "Mr";
        }

        return civilStatus switch
        {
            "Married" => "Mrs",
            _ => "Ms"
        };
    }
}
