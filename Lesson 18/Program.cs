using MiniBank.Models;
using MiniBank.Repository;

namespace Lesson_18
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string filePath = "../../../test.txt";

            string filePath = @"C:\\Users\\l4nst\\source\\repos\\Lesson 18\\MiniBank.Repository\\Data\\Customers.csv";

            var repo = new CustomerFileStreamRepository(filePath, "CSV");
            var customer1 = new Customer
            {
                Name = "Someone Someone",
                IdentityNumber = "123456789",
                PhoneNumber = "599010101",
                Email = "some@gmail.com",
                Type = AccountType.Phyisical
            };
            repo.Create(customer1);

            
            var customers = repo.GetCustomers();
            foreach (var customer in customers)
            {
                Console.WriteLine($"Id: {customer.Id}, Name: {customer.Name}, Email: {customer.Email}, Type: {customer.Type}");
            }


            var customerToUpdate = customers[10];
            customerToUpdate.Name = "SOMEONE UPDATED";
            repo.Update(customerToUpdate);

           
            Console.WriteLine("Updated customer:");
            var updatedCustomer = repo.GetCustomer(customerToUpdate.Id);
            Console.WriteLine($"Id: {updatedCustomer.Id}, Name: {updatedCustomer.Name}, Email: {updatedCustomer.Email}, Type: {updatedCustomer.Type}");

         
            var customerToDelete = customers[1];
            repo.Delete(customerToDelete.Id);

          
            Console.WriteLine("Customers after update and delete:");
            var restData = repo.GetCustomers();
            foreach (var customer in restData)
            {
                Console.WriteLine($"Id: {customer.Id}, Name: {customer.Name}, Email: {customer.Email}, Type: {customer.Type}");
            }

            //StreamWriter ფაილში ჩაწერა
            //using (StreamWriter sw = new StreamWriter(filePath, append: true))
            //{
            //    sw.WriteLine("Hello Lana");
            //}


            ////StreamReader ფაიილს წაკითხვა
            //using (StreamReader sr = new(filePath))
            //{
            //    var content = sr.ReadToEnd();
            //}


            //FileStream ფაილში ჩაწერა
            //try
            //{
            //    using (FileStream fileStreamWriter = new FileStream(filePath, FileMode.Append, FileAccess.Write))
            //    {
            //        byte[] data = Encoding.UTF8.GetBytes("Hello World");
            //        fileStreamWriter.Write(data, 0, data.Length);

            //        Console.WriteLine("Data written in file.");

            //    }// თავისით გამოიძახოს Dispose
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}



            //FileStream ფაილის წაკითხვა
            //try
            //{
            //    using (FileStream fileStreamReader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            //    {
            //        byte[] buffer = new byte[fileStreamReader.Length];
            //        fileStreamReader.Read(buffer, 0, buffer.Length);
            //        string realData = Encoding.UTF8.GetString(buffer);

            //        Console.WriteLine(realData);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
        }
    }
}
