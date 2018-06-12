using Bogus;
using ExampleFunctions.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExampleFunctions.Services
{
    public class DataService<T>
        : IDataService<T>
        where T : Models.Person
    {
        private readonly IEnumerable<T> data;

        public DataService()
        {
            data = new Faker<T>()
                .RuleFor(p => p.FirstName, p => p.Person.FirstName)
                .RuleFor(p => p.LastName, p => p.Person.LastName)
                .RuleFor(p => p.BirthDate, p => p.Person.DateOfBirth)
                .RuleFor(p => p.Biography, p => p.Lorem.Paragraphs())
                .RuleFor(p => p.ImageUrl, p => p.Image.People())
                .Generate(100);
        }

        public async Task<IEnumerable<T>> FetchAllAsync()
        {
            return await Task.FromResult(data);
        }
    }
}
