using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class SQLEmployeeRepository : IEmployeeRepository
    {

        // Depentency injection with using constructor
        private readonly AppDbContext context;

        public SQLEmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }

        // IEmployeeRepository Interface implementations
        public Employee Add(Employee employee)
        {
            context.Employees.Add(employee);
            context.SaveChanges();
            return employee;
        }

        public Employee Delete(int id)
        {
            // Check if the desired employee presents
            Employee employee = context.Employees.Find(id);
            // If so, delete it.
            if (employee != null)
            {
                context.Employees.Remove(employee);
                context.SaveChanges();
            }
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployee()
        {
            return context.Employees;
        }

        public Employee GetEmployee(int Id)
        {
            return context.Employees.Find(Id);
        }

        public Employee Update(Employee employeeChanges)
        {
            // First attach the employeeChanges to the DbSet
            var employee =  context.Employees.Attach(employeeChanges);
            // Then inform the Entity Framework that the entity that we have attached is modified by changing the state of the entity
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return employeeChanges;
        }
    }
}
