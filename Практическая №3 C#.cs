using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}

public class TaskItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public string Status { get; set; }
}

public class TaskManager
{
    private List<User> users = new List<User>();
    private User currentUser;

    public async Task RegisterAsync(string username, string password)
    {
        await Task.Run(() =>
        {
            if (users.Exists(u => u.Username == username))
            {
                Console.WriteLine("Пользователь уже существует!");
                return;
            }
            users.Add(new User { Username = username, Password = password });
            Console.WriteLine("Регистрация пользователя прошла успешно");
        });
    }

   
    public async Task<bool> LoginAsync(string username, string password)
    {
        return await Task.Run(() =>
        {
            var user = users.Find(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                currentUser = user;
                Console.WriteLine("Авторизация прошла успешно");
                return true;
            }
            Console.WriteLine("Неверное имя пользователя или пароль!");
            return false;
        });
    }

    
    public async Task CreateTaskAsync(string title, string description, string priority, string status)
    {
        await Task.Run(() =>
        {
            if (currentUser == null)
            {
                Console.WriteLine("Необходимо войти в систему!");
                return;
            }
            currentUser.Tasks.Add(new TaskItem { Title = title, Description = description, Priority = priority, Status = status });
            Console.WriteLine("Задача создана!");
        });
    }

    public async Task EditTaskAsync(int taskIndex, string title, string description, string priority, string status)
    {
        await Task.Run(() =>
        {
            if (currentUser == null)
            {
                Console.WriteLine("Необходимо войти в систему!");
                return;
            }
            if (taskIndex >= 0 && taskIndex < currentUser.Tasks.Count)
            {
                var task = currentUser.Tasks[taskIndex];
                task.Title = title;
                task.Description = description;
                task.Priority = priority;
                task.Status = status;
                Console.WriteLine("Задача отредактирована!");
            }
            else
            {
                Console.WriteLine("Неверный номер задачи!");
            }
        });
    }

    public async Task DeleteTaskAsync(int taskIndex)
    {
        await Task.Run(() =>
        {
            if (currentUser == null)
            {
                Console.WriteLine("Необходимо войти в систему!");
                return;
            }
            if (taskIndex >= 0 && taskIndex < currentUser.Tasks.Count)
            {
                currentUser.Tasks.RemoveAt(taskIndex);
                Console.WriteLine("Задача успешно удалена!");
            }
            else
            {
                Console.WriteLine("Неверный номер задачи!");
            }
        });
    }

    
    public async Task ShowTasksAsync()
    {
        await Task.Run(() =>
        {
            if (currentUser == null)
            {
                Console.WriteLine("Необходимо войти в систему!");
                return;
            }
            if (currentUser.Tasks.Count == 0)
            {
                Console.WriteLine("Задачи отсутствуют!");
                return;
            }
            for (int i = 0; i < currentUser.Tasks.Count; i++)
            {
                var task = currentUser.Tasks[i];
                Console.WriteLine($"Задача {i}: Заголовок - {task.Title}, Описание - {task.Description}, Приоритет - {task.Priority}, Статус - {task.Status}");
            }
        });
    }

    public User CurrentUser => currentUser; 
}

class Program
{
    static async Task Main(string[] args)
    {
        TaskManager taskManager = new TaskManager();

        while (true)
        {
            Console.WriteLine("\n1. Регистрация");
            Console.WriteLine("2. Авторизация");
            Console.WriteLine("3. Выход");
            Console.Write("Выберите действие: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Введите имя пользователя: ");
                string username = Console.ReadLine();
                Console.Write("Введите пароль: ");
                string password = Console.ReadLine();
                await taskManager.RegisterAsync(username, password);
            }
            else if (choice == "2")
            {
                Console.Write("Введите имя пользователя: ");
                string username = Console.ReadLine();
                Console.Write("Введите пароль: ");
                string password = Console.ReadLine();
                if (await taskManager.LoginAsync(username, password))
                {
                    await TaskManagementMenu(taskManager);
                }
            }
            else if (choice == "3")
            {
                break; 
            }
            else
            {
                Console.WriteLine("Неверный выбор!");
            }
        }
    }

    static async Task TaskManagementMenu(TaskManager taskManager)
    {
        while (true)
        {
            Console.WriteLine("\n1. Создать задачу");
            Console.WriteLine("2. Редактировать задачу");
            Console.WriteLine("3. Удалить задачу");
            Console.WriteLine("4. Показать задачи");
            Console.WriteLine("5. Выйти из системы");
            Console.Write("Выберите действие: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.Write("Введите заголовок задачи: ");
                string title = Console.ReadLine();
                Console.Write("Введите описание задачи: ");
                string description = Console.ReadLine();
                Console.Write("Введите приоритет задачи (низкий, средний, высокий): ");
                string priority = Console.ReadLine();
                Console.Write("Введите статус задачи (недоступна, в процессе, завершена): ");
                string status = Console.ReadLine();
                await taskManager.CreateTaskAsync(title, description, priority, status);
            }
            else if (choice == "2")
            {
                Console.Write("Введите номер задачи для редактирования: ");
                if (int.TryParse(Console.ReadLine(), out int taskIndex))
                {
                    Console.Write("Введите новый заголовок задачи: ");
                    string title = Console.ReadLine();
                    Console.Write("Введите новое описание задачи: ");
                    string description = Console.ReadLine();
                    Console.Write("Введите новый приоритет задачи (низкий, средний, высокий): ");
                    string priority = Console.ReadLine();
                    Console.Write("Введите новый статус задачи (недоступна, в процессе, завершена): ");
                    string status = Console.ReadLine();
                    await taskManager.EditTaskAsync(taskIndex, title, description, priority, status);
                }
                else
                {
                    Console.WriteLine("Неверный номер задачи!");
                }
            }
            else if (choice == "3")
            {
                Console.Write("Введите номер задачи для удаления: ");
                if (int.TryParse(Console.ReadLine(), out int taskIndex))
                {
                    await taskManager.DeleteTaskAsync(taskIndex);
                }
                else
                {
                    Console.WriteLine("Некорректный номер задачи!");
                }
            }
            else if (choice == "4")
            {
                await taskManager.ShowTasksAsync();
            }
            else if (choice == "5")
            {
                break;
            }
            else
            {
                Console.WriteLine("Неверный выбор!");
            }
        }
    }
}
