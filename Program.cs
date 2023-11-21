// See https://aka.ms/new-console-template for more information

using Catworx.BadgeMaker;
using CatWorx.BadgeMaker;

Console.WriteLine("Enter Y to get data from api, or N to enter data yourself: ");
string answer = Console.ReadLine() ?? "";
List<Employee> employees;
if (answer == "Y") 
{
   employees = await PeopleFetcher.GetFromApi("https://randomuser.me/api/?results=10&nat=us&inc=name,id,picture");
} else {
    employees = PeopleFetcher.GetEmployees();
}

Util.PrintEmployees(employees);
Util.MakeCSV(employees);
await Util.MakeBadges(employees);