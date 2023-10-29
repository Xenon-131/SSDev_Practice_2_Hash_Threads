using System.Text;
using System.Text.RegularExpressions;

using System.Security.Cryptography;
using System.Collections.Generic;

namespace Practice_2
{
  public class Hash
  {

    private readonly char[] Dictionary =
    {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
            'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
            's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
    };

    private List<string> hashStrings;
    public Hash()
    {
      this.hashStrings = new List<string>();
    }
    public List<string> getHashStrings()
    {
      return this.hashStrings;
    }
    public bool setHash()
    {
      while (true)
      {
        if (this.hashStrings != null)
        {
          this.hashStrings.Clear();
        }

        Console.Clear();
        Console.WriteLine("Выберите команду:");
        Console.WriteLine("1. Ввести хэш");
        Console.WriteLine("2. Считать хэши из файла");
        Console.WriteLine("3. Закончить работу");
        string command = Console.ReadLine();
        if (command == "1")
        {
          getHashFromConsole();
          return true;
        }
        else if (command == "2")
        {
          getHashesFromFile();
          return true;
        }
        else if (command == "3")
        {
          return false;
        }
        else
        {
          Console.WriteLine("Неверное значение");
        }

        Console.WriteLine("Нажмите любую клавишу, чтобы ввести команду еще раз.");
        Console.ReadKey();
      }
    }
    public void getHashFromConsole()
    {
      while (true)
      {
        Console.Clear();
        Console.Write("Введите хэш: ");
        string hash = Console.ReadLine();
        while (string.IsNullOrEmpty(hash) || hash.Trim() == string.Empty)
        {
          Console.Clear();
          Console.Write("Введите хэш: ");
          hash = Console.ReadLine();
        }

        if (Regex.IsMatch(hash, @"^[A-Za-z0-9]*$"))
        {
          this.hashStrings.Add(hash);
          return;
        }

        Console.WriteLine("Хэш может содержать только цифры и буквы от A до F в любом регистре");
        Console.WriteLine("Нажмите любую клавишу, чтобы ввести хэш снова");
        Console.ReadKey();
      }
    }
    public void getHashesFromFile()
    {
      while (true)
      {
        Console.Clear();
        Console.WriteLine("Введите название файла с хэшами");
        //string path = Console.ReadLine()?.Trim('"');
        string name = Console.ReadLine();
        while (string.IsNullOrEmpty(name) || name.Trim() == string.Empty)
        {
          Console.Clear();
          Console.WriteLine("Введите название файла с хэшами");
          name = Console.ReadLine();
        }

        string dirName = @"D:\university\Курс_4\Разработка безопасного ПО\Practice_2_Directory";
        string path = dirName + $@"\{name}";

        if (File.Exists(path))
        {
          foreach (string line in File.ReadLines(path, Encoding.Default))
          {
            this.hashStrings.Add(line);
          }
          return;
        }

        Console.WriteLine("Такого файла не существует");
        Console.WriteLine("Нажмите любую клавишу для повторного ввода");
        Console.ReadKey();
      }
    }

    static string GetStringSha256Hash(string password)
    {
      if (string.IsNullOrEmpty(password))
        return string.Empty;

      using (var sha = new SHA256Managed())
      {
        byte[] textData = Encoding.UTF8.GetBytes(password);
        byte[] hash = sha.ComputeHash(textData);
        return BitConverter.ToString(hash).Replace("-", string.Empty);
      }
    }

    public void SingleThreadBruteHash()
    {
      bool flag = false;
      DateTime start = DateTime.Now;
      int length = Dictionary.Length;
      int count = 0;
      for (int ch1 = 0; ch1 < length; ch1++)
      {
        string a = Convert.ToString(Dictionary[ch1]);
        for (int ch2 = 0; ch2 < length; ch2++)
        {
          string b = Convert.ToString(Dictionary[ch2]);
          for (int ch3 = 0; ch3 < length; ch3++)
          {
            string c = Convert.ToString(Dictionary[ch3]);
            for (int ch4 = 0; ch4 < length; ch4++)
            {
              string d = Convert.ToString(Dictionary[ch4]);
              for (int ch5 = 0; ch5 < length; ch5++)
              {
                string e = Convert.ToString(Dictionary[ch5]);
                string password = a + b + c + d + e;
                string hash = Hash.GetStringSha256Hash(password);
                foreach (string line in this.hashStrings)
                {
                  if (!line.ToUpper().Contains(hash)) continue;

                  Console.WriteLine($"Найден пароль {password}, hash {hash}");
                  Console.WriteLine(DateTime.Now - start);
                  count++;
                  if (count == this.hashStrings.Count) flag = true;
                  break;
                }
                if (flag) break;
              }
              if (flag) break;
            }
            if (flag) break;
          }
          if (flag) break;
        }
        if (flag) break;
      }
    }

    public void MultiThreadBruteHash(int threadCount)
    {
      bool flag = false;
      DateTime start = DateTime.Now;
      int delta = 26 / threadCount;
      for (int i = 0; i < threadCount; i++)
      {
        int threadNum = i;
        new Thread(() =>
        {
          byte[] password = new byte[5];
          int count = 0;
          password[0] = (byte)(97 + threadNum * delta);
          int max = 97 + (threadNum + 1)*delta;
          if (max + delta > 123 && max < 123) max = 123;
          for (; password[0] < max; password[0]++)
          {
            
            for (password[1] = 97; password[1] < 123; password[1]++)
            {
              for (password[2] = 97; password[2] < 123; password[2]++)
              {
                for (password[3] = 97; password[3] < 123; password[3]++)
                {
                  for (password[4] = 97; password[4] < 123; password[4]++)
                  {
                    string passwordString = Encoding.ASCII.GetString(password);
                    string hash = Hash.GetStringSha256Hash(passwordString);
                    foreach (string line in this.hashStrings)
                    {
                      if (!line.ToUpper().Contains(hash)) continue;

                      Console.WriteLine($"Найден пароль {passwordString}, hash {hash}, thread :{threadNum}");
                      Console.WriteLine(DateTime.Now - start);
                      count++;
                      if (count == this.hashStrings.Count) flag = true;
                      break;
                    }

                    if (flag) break;
                  }

                  if (flag) break;
                }

                if (flag) break;
              }

              if (flag) break;
            }
            if (flag) break;
          }
        }).Start();
      }
    }
  }
  internal static class Program
  {
    
    public static void Main()
    {

      ShowThreadCommands();                                


    }
    
    public static void ShowThreadCommands()
    {
      while (true)
      {
        Hash hash = new Hash();
        if (!hash.setHash())
        {
          Console.WriteLine("Работа закончена.");
          break;
        }
        Console.Clear();
        Console.WriteLine("Выберите принцип работы:");
        Console.WriteLine("1. Однопоточный");
        Console.WriteLine("2. Многопоточный");
        Console.WriteLine("3. Вернуться");
        string choice = Console.ReadLine();

        Console.Clear();
        switch (choice)
        {
          case "1":
            hash.SingleThreadBruteHash();
            break;
          case "2":
            Console.Write("Введите количество потоков:");
            int threadCount = Int16.Parse(Console.ReadLine());
            if(threadCount > 26) 
            {
              threadCount = 26;
            }
            hash.MultiThreadBruteHash(threadCount);
            break;
          case "3":
            ShowThreadCommands();
            break;
          default:
            Console.Clear();
            Console.WriteLine("Неверное значение");
            break;
        }

        //Console.WriteLine("Нажмите любую клавишу для повторного выбора");
        Console.ReadKey();
      }

    }
  }
}
