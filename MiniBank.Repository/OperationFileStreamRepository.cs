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
    public class OperationFileStreamRepository
    {
        private readonly string _filePath;
        private readonly string _fileType;
        private List<Operation> _operations;

        public OperationFileStreamRepository(string filePath, string fileType)
        {
            _filePath = filePath;
            _fileType = fileType;
            _operations = LoadData();
        }

        public List<Operation> GetOperations() => _operations;

        public Operation GetOperation(int id) => _operations.FirstOrDefault(op => op.Id == id);

        public void Create(Operation operation)
        {
            operation.Id = _operations.Any() ? _operations.Max(o => o.Id) + 1 : 1;
            _operations.Add(operation);
            SaveData();
        }

        public void Update(Operation operation)
        {
            var index = _operations.FindIndex(o => o.Id == operation.Id);
            if (index >= 0)
            {
                _operations[index] = operation;
                SaveData();
            }
        }

        public void Delete(int id)
        {
            var operation = _operations.FirstOrDefault(op => op.Id == id);
            if (operation != null)
            {
                _operations.Remove(operation);
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
                    writer.WriteLine("Id,OperationType,Currency,Amount,AccountId,CustomerId,HappendAt");
                    foreach (var operation in _operations)
                    {
                        writer.WriteLine($"{operation.Id},{operation.OperationType},{operation.Currency},{operation.Amount},{operation.AccountId},{operation.CustomerId},{operation.HappendAt:yyyy-MM-dd HH:mm:ss}");
                    }
                }
                else if (_fileType == "JSON")
                {
                    var json = JsonSerializer.Serialize(_operations, new JsonSerializerOptions { WriteIndented = true });
                    writer.Write(json);
                }
                else if (_fileType == "XML")
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<Operation>));
                    using (var xmlWriter = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        xmlSerializer.Serialize(xmlWriter, _operations);
                    }
                }
            }
        }

        private List<Operation> LoadData()
        {
            if (!File.Exists(_filePath))
                return new List<Operation>();

            var operations = new List<Operation>();

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
                        var operation = new Operation
                        {
                            Id = int.Parse(parts[0]),
                            OperationType = Enum.Parse<OperationType>(parts[1]),
                            Currency = parts[2],
                            Amount = decimal.Parse(parts[3]),
                            AccountId = int.Parse(parts[4]),
                            CustomerId = int.Parse(parts[5]),
                            HappendAt = DateTime.Parse(parts[6])
                        };
                        operations.Add(operation);
                    }
                }
                else if (_fileType == "JSON")
                {
                    var json = reader.ReadToEnd();
                    operations = JsonSerializer.Deserialize<List<Operation>>(json);
                }
                else if (_fileType == "XML")
                {
                    var xmlSerializer = new XmlSerializer(typeof(List<Operation>));
                    using (var xmlReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        operations = (List<Operation>)xmlSerializer.Deserialize(xmlReader);
                    }
                }
            }

            return operations;
        }
    }
}
