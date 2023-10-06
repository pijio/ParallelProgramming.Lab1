// See https://aka.ms/new-console-template for more information

using ParallelProgramming.Lab1.App;

var manager = new ProducerConsumerManager<char>(16);

char ProduceAction() => Console.ReadKey().KeyChar;
var digitDetector = new Predicate<char>(char.IsDigit);
var symbolDetector = new Predicate<char>(ch => !char.IsLetterOrDigit(ch));
var alphabetDetector = new Predicate<char>(char.IsLetter);

var producer = new Thread(() =>
{
   while (true)
   {
      manager.Produce(ProduceAction);
   }
});
var consumer1 = new Thread(() =>
{
   while (true)
   {
      var consumeRes = manager.Consume(digitDetector);
      if(consumeRes.Item2)
         Console.WriteLine($"Консьюмер 1 получил символ {consumeRes.Item1}");    
   }
});
var consumer2 = new Thread(() =>
{
   while (true)
   {
      var consumeRes = manager.Consume(symbolDetector);
      if(consumeRes.Item2)
         Console.WriteLine($"Консьюмер 2 получил символ {consumeRes.Item1}");  
   }
});
var consumer3 = new Thread(() =>
{
   while (true)
   {
      var consumeRes = manager.Consume(alphabetDetector);
      if(consumeRes.Item2)
         Console.WriteLine($"Консьюмер 3 получил символ {consumeRes.Item1}");  
   }
});

var cts = new CancellationTokenSource();
var token = cts.Token;
producer.Start();
consumer1.Start();
consumer2.Start();
consumer3.Start();