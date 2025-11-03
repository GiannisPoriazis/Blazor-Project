using BlazorApp.Models;

namespace BlazorApp.Utilities
{
    public static class EmployeeManagerTools
    {
        public static void PrintName(object person)
        {
            ArgumentNullException.ThrowIfNull(person);

            var name = person switch
            {
                Employee e => e.Name,
                Manager m => m.Name,
                _ => throw new ArgumentException("Parameter must be of type Employee or Manager", nameof(person))
            };

            Console.WriteLine(name);
        }
    }
}
