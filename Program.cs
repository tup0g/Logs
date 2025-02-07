using System;
using System.Collections.Generic;
using Serilog;

class Program{
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("operation.log")
                    .CreateLogger();
        Log.Information("Додаток запущено");

        List<Goods> goodsList = new List<Goods>
        {
            new Goods(101, 2),
            new Goods(102, 5),
            new Goods(103, 10)
        };

        List<Customer> customers = new List<Customer>
        {
            new Customer("Клієнт 1"),
            new Customer("Клієнт 2"),
            new Customer("Клієнт 3")
        };
        
        try
        {
            foreach (var customer in customers)
            {
                foreach (var goods in goodsList)
                {
                    ProcessOrder(goods.OrderId, customer.Name, goods.Quantity);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Виникла критична помилка в додатку");
        }
        finally
        {
            Log.Information("Робота додатку завершена");
            Log.CloseAndFlush();
        }
    }

    static void ProcessOrder(int orderId, string customer, int quantity)
    {
        Log.Information("Прийом замовлення #{OrderId} від {Customer}", orderId, customer);
        if (quantity <= 0)
        {
            Log.Warning("Замовлення #{OrderId}: Некоректна кількість товару ({Quantity})", orderId, quantity);
            throw new ArgumentException("Кількість товару має бути більшою за нуль.");
        }

        try
        {
            Log.Information("Замовлення #{OrderId}: Перевірка валідності пройдена", orderId);
            ProcessPayment(orderId);
            Log.Information("Замовлення #{OrderId} підтверджене", orderId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Помилка під час обробки замовлення #{OrderId}", orderId);
        }
    }

    static void ProcessPayment(int orderId)
    {
        Log.Information("Обробка оплати для замовлення #{OrderId}", orderId);
        Random random = new Random();
        if (random.Next(1, 5) == 1) // 20% шанс на збій оплати
        {
            throw new Exception("Помилка оплати. Платіж не пройшов.");
        }
        Log.Information("Оплата для замовлення #{OrderId} успішно пройдена", orderId);
    }
}

class Goods{
    public int OrderId { get; set; }
    public int Quantity { get; set; }

    public Goods(int oI, int q){
        OrderId = oI;
        Quantity = q;
    }
}

class Customer{
    public string Name { get; set; }
    
    public Customer(string name)
    {
        Name = name;
    }
}
