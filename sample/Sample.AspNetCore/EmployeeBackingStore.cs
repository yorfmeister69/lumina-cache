using LuminaCache.Core;

namespace Sample.AspNetCore;

public class EmployeeBackingStore : IBackingStore<Employee>
{
    private static readonly Random _random = new();

    private static readonly List<string> Names = new()
    {
        "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Heidi", "Ivan", "Judy", "Kevin", "Linda",
        "Mallory", "Niaj", "Oscar", "Peggy", "Quentin", "Rob", "Sybil", "Trent", "Ursula", "Victor", "Walter", "Xavier",
        "Yvonne", "Zoe", "Abe", "Beth", "Carl", "Dawn", "Ed", "Fay", "Gus", "Helen", "Ike", "Jan", "Karl", "Lucy",
        "Mary", "Nate", "Olive", "Paul", "Quinn", "Rose", "Sam", "Tina", "Uma", "Vince", "Wendy", "Xander", "Yosef",
        "Zelda", "Amy", "Ben", "Cara", "Dan", "Elise", "Fred", "Gina", "Hank", "Ingrid", "Jack", "Kathy", "Luke",
        "Megan", "Noah", "Olivia", "Peter", "Quincy", "Rachel", "Steve", "Tara", "Ulysses", "Violet", "Will", "Xavier",
        "Yvonne", "Zach", "Ava", "Bill", "Cathy", "Drew", "Emma", "Gabe", "Holly", "Ivan", "Jill", "Kyle", "Liz"
    };

    private static readonly List<string> JobTitles = new()
    {
        "Software Engineer", "Data Analyst", "HR Specialist", "Manager",
        "Marketing Specialist", "Accountant", "Product Manager", "Sales Manager", "CEO", "CTO", "CFO", "COO", "CIO"
    };

    private static readonly List<string> Departments = new()
    {
        "Engineering", "HR", "Marketing", "Finance", "Product"
    };

    public Task<IEnumerable<Employee>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(GenerateEmployees(50));
    }

    private static IEnumerable<Employee> GenerateEmployees(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var id = "E0" + i;
            var name = Names[_random.Next(Names.Count)];
            var jobTitle = JobTitles[_random.Next(JobTitles.Count)];
            var department = Departments[_random.Next(Departments.Count)];

            yield return new Employee(id, name, jobTitle, department);
        }
    }
}