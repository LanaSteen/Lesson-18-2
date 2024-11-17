using MiniBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Repository
{
    public class CustomerFileStreamRepository
    {
        private readonly string _filePath;
        private List<Customer> _customers;

        public CustomerFileStreamRepository(string filePath)
        {
            _filePath = filePath;
            _customers = LoadData();
        }

        public List<Customer> GetCustomers() => _customers;

        public Customer GetCustomer(int id) => _customers.FirstOrDefault(person => person.Id == id);

        public void Create(Customer customer)
        {
            customer.Id = _customers.Any() ? _customers.Max(c => c.Id) + 1 : 1;
            _customers.Add(customer);
            SaveData();
        }

        public void Update(Customer customer)
        {
            var index = _customers.FindIndex(c => c.Id == customer.Id);
            if (index >= 0)
            {
                _customers[index] = customer;
                SaveData();
            }
        }

        public void Delete(int id)
        {
            var customer = _customers.FirstOrDefault(person => person.Id == id);
            if (customer != null)
            {
                _customers.Remove(customer);
                SaveData();
            }
        }

        private void SaveData()
        {
        
            using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
             
                writer.WriteLine("Id,Name,IdentityNumber,PhoneNumber,Email,Type");

          
                foreach (var customer in _customers)
                {
                    writer.WriteLine($"{customer.Id},{customer.Name},{customer.IdentityNumber},{customer.PhoneNumber},{customer.Email},{customer.Type}");
                }
            }
        }

        private List<Customer> LoadData()
        {
           
            if (!File.Exists(_filePath))
                return new List<Customer>();

            var customers = new List<Customer>();

    
            using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream, Encoding.UTF8))
            {
           
                reader.ReadLine();

           
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');
                    var customer = new Customer
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        IdentityNumber = parts[2],
                        PhoneNumber = parts[3],
                        Email = parts[4],
                        Type = Enum.Parse<AccountType>(parts[5])
                    };
                    customers.Add(customer);
                }
            }

            return customers;
        }

    }

}
