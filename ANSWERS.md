# Answers to the Technical Questions

1. **How long did you spend on the coding assignment? What would you add to your solution if you had more time? If you didn't spend much time on the coding assignment then use this as an opportunity to explain what you would add.**

    I spent about 10 hours on the coding assignment. If I had more time, I would add the following features:
    - Add a feature to allow users specify the currencies they want the cryptocurrency quote to be converted to.
    - Add a feature to allow users fetch the latest quotes for all cryptocurrencies.
    - Add the 24-hour percentage change in price for each cryptocurrency.
    - If I had more time, I would also use `IOption` to read the configuration only once and use it throughout the application, instead of reading the configuration every time a request is made.

2. **What was the most useful feature that was added to the latest version of your language of choice? Please include a snippet of code that shows how you've used it.**

    I wanted to go with C# 13, but I cannot find any interesting feature that was added to the latest version of C#. So, I will go with C# 12. I found two features that were added to C# 12 to be very useful, which I have used in this project. They are:

    - *Primary Constructors*: This feature allows you to define a class and its constructor in a single line of code. This makes the code cleaner and more readable, as the variables declared in the constructor are automatically assigned to the class properties and can be accessed anywhere in the class.

        ```csharp
        public class MyClass(string name, int age)
        {
            public void MyMethod()
            {
                Console.WriteLine($"Name: {name}, Age: {age}");
            }
        }
        ```

    - *Collection Expressions*: This feature allows you to initialize a collection with a single line of code, using the `[]` syntax.

        ```csharp
        List<int> numbers = [1, 2, 3, 4, 5]; // New way
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5 }; // Old way
        ```

3. **How would you track down a performance issue in production? Have you ever had to do this?**

    To track down a performance issue in production, I would follow these steps:

   1. *Identify the Problem*: The first step is to identify the problem. This can be done by monitoring the system and looking for any signs of performance degradation, such as slow response times, high CPU or memory usage. This can be done using monitoring tools such as Prometheus and Grafana.
   2. *Analyze the Logs*: Once the problem has been identified, the next step is to analyze the logs to determine the root cause of the issue. This can lead to identifying the specific code or system component that is causing errors or performance issues.
   3. *Profile the Code and Analyze Bottlenecks*: Once the root cause has been identified, the next step is to profile the code to identify bottlenecks and areas of improvement. This can be done using profiling tools such as dotTrace.
   4. *Fix the Issue*: Once the bottlenecks have been identified, the next step is to fix the issue by optimizing the code or system configuration. Also, if the issue is a result of a bug, it should be fixed and deployed to production. Alternatively, if a permanent fix is not possible at the moment, a temporary fix can be applied to mitigate the issue, such as increasing the resources allocated to the system or implementing caching and rate limiting.
   5. *Monitor and Test*: After the issue has been fixed, the system should be monitored to ensure that the performance has improved and that the issue does not reoccur. Additionally, load testing can be performed to ensure that the system can handle the expected load and that the performance is acceptable.

    I have not personally had to track down a performance issue in production, but I have monitored and analyzed logs to make sure that the system is running smoothly after deployment.

4. **What was the latest technical book you have read or tech conference you have been to? What did you learn?**

    I have read a few technical books recently for my course work and personal development. Here are some of the books I have read:

    - *Computer Networks - A Systems Approach*: This is the main textbook for my computer networks course. I learned about the different layers of the OSI model, the TCP/IP protocol suite, and how data is transmitted over the network.
    - *Operating System Concepts*: This is the main textbook for my operating systems course. I learned about the different components of an operating system, such as process management, memory management, and file systems.
    - *C# in a Nutshell*: This is a reference book for C# programming. I learned about the different features of C# and how to use them in my projects.

5. **What do you think about this technical assessment?**

    I think this technical assessment was well-designed and challenging. It tested my knowledge of C# programming, API integration, and software design principles. However, to be honest, I think these kinds of assessments are not a good measure of a developer's skills and abilities in today's world, as LLMs such as ChatGPT can easily generate code that implements the required functionalities.

6. **Please, describe yourself using JSON.**

    ```json
    {
        "name": "Pasha Barahimi",
        "age": 22,
        "location": "Tehran, Iran",
        "education": {
            "degree": "B.Sc. in Computer Engineering",
            "school": "University of Tehran"
        },
        "skills": {
            "languages": ["C++", "C#", "Python", "Golang", "Java", "Bash", "Rust"],
            "frameworks and technologies": ["Git", "Docker", "Kubernetes", "Makefile", "Ansible", "Django"],
            "databases": ["PostgreSQL", "Redis", "Prometheus"]
        },
        "interests": ["Software Development", "Security", "Systems"],
        "hobbies": ["Coding", "Watching Movies", "Playing 8-ball Pool"]
    }
    ```
