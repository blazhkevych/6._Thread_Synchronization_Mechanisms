# 6. Механизмы синхронизации потоков
Механизмы синхронизации потоков
Создайте оконное приложение, использующее механизм мьютексов. В
коде приложения должно быть три потока. Первый поток генерирует набор 
случайных чисел и записывает их в файл. Второй поток ожидает, когда первый
закончит своё исполнение, после чего анализирует содержимое файла и 
создаёт новый файл, в котором должны быть собраны только простые числа 
из первого файла. Третий поток ожидает, когда закончится второй поток,
после чего создаёт новый файл, в котором должны быть собраны все простые 
числа из второго файла у которых последняя цифра равна 7. 
Предусмотреть, чтобы оконное приложение могло запускаться только в 
трёх копиях. При попытке запустить четвертую копию необходимо отображать 
информационное сообщение и закрывать приложение.

# 6. Thread Synchronization Mechanisms
Thread Synchronization Mechanisms
Create a windowed application that uses the mutex mechanism. IN
The application code must have three threads. The first thread generates a set
random numbers and writes them to a file. The second thread is waiting for the first
finishes its execution, after which it analyzes the contents of the file and
creates a new file in which only prime numbers should be collected
from the first file. The third thread is waiting for the second thread to finish.
after which it creates a new file, in which all simple
numbers from the second file whose last digit is 7.
Provide that a windowed application can only be launched in
three copies. When you try to run the fourth copy, you need to display
informational message and close the application.
