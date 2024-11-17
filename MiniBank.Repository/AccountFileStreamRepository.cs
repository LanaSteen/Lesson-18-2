using MiniBank.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace MiniBank.Repository
{
    public class AccountFileStreamRepository
    {
        private readonly string _filePath;
        private readonly string _fileType;
        private List<Account> _accounts;

        public AccountFileStreamRepository(string filePath, string fileType)
        {
            _filePath = filePath;
            _fileType = fileType;
            _accounts = LoadData();
        }

        public List<Account> GetAccounts() => _accounts;

        public Account GetAccount(int id) => _accounts.FirstOrDefault(acc => acc.Id == id);

        public void Create(Account account)
        {
            account.Id = _accounts.Any() ? _accounts.Max(a => a.Id) + 1 : 1;
            _accounts.Add(account);
            SaveData();
        }

        public void Update(Account account)
        {
            var index = _accounts.FindIndex(a => a.Id == account.Id);
            if (index >= 0)
            {
                _accounts[index] = account;
                SaveData();
            }
        }

        public void Delete(int id)
        {
            var account = _accounts.FirstOrDefault(acc => acc.Id == id);
            if (account != null)
            {
                _accounts.Remove(account);
                SaveData();
            }
        }

        private void SaveData()
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fileStream, Encoding.UTF8))
            {
                if (_fileType == "CSV")
                {
                    writer.WriteLine("Id,Iban,Currency,Balance,CustomerId,Name");
                    foreach (var account in _accounts)
                    {
                        writer.WriteLine($"{account.Id},{account.Iban},{account.Currency},{account.Balance},{account.CustomerId},{account.Name}");
                    }
                }
                else if (_fileType == "JSON")
                {
                    var json = JsonSerializer.Serialize(_accounts, new JsonSerializerOptions { WriteIndented = true });
                    writer.Write(json);
                }
                else if (_fileType == "XML")
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<Account>));
                    using (var xmlWriter = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        xmlSerializer.Serialize(xmlWriter, _accounts);
                    }
                }
            }
        }

        private List<Account> LoadData()
        {
            if (!File.Exists(_filePath))
                return new List<Account>();

            var accounts = new List<Account>();

            using (var fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fileStream, Encoding.UTF8))
            {
                if (_fileType == "CSV")
                {
                    reader.ReadLine();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        var account = new Account
                        {
                            Id = int.Parse(parts[0]),
                            Iban = parts[1],
                            Currency = parts[2],
                            Balance = decimal.Parse(parts[3]),
                            CustomerId = int.Parse(parts[4]),
                            Name = parts[5]
                        };
                        accounts.Add(account);
                    }
                }
                else if (_fileType == "JSON")
                {
                    var json = reader.ReadToEnd();
                    accounts = JsonSerializer.Deserialize<List<Account>>(json);
                }
                else if (_fileType == "XML")
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<Account>));
                    using (var xmlReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        accounts = (List<Account>)xmlSerializer.Deserialize(xmlReader);
                    }
                }
            }

            return accounts;
        }
    }
}
