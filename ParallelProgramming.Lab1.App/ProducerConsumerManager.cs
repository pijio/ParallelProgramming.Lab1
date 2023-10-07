namespace ParallelProgramming.Lab1.App;

public class ProducerConsumerManager<T>
{
    private SemaphoreSlim _full;
    private SemaphoreSlim _empty;
    private SemaphoreSlim _binary;
    private RingBuffer<T> _buffer; 

    public ProducerConsumerManager(int capacity)
    {
        _buffer = new RingBuffer<T>(capacity);
        _binary = new SemaphoreSlim(1, 1);
        _full = new SemaphoreSlim(0, capacity);
        _empty = new SemaphoreSlim(capacity, capacity);
    }

    /// <summary>
    /// Родить элемент по делегату
    /// </summary>
    /// <param name="produceFunc">Роженица</param>
    public void Produce(Func<T> produceFunc) => Produce(produceFunc.Invoke());

    /// <summary>
    /// Передать новорожденного консьюмерам через буфер
    /// </summary>
    /// <param name="producedElement">Новорожденный</param>
    public void Produce(T producedElement)
    {
        _empty.Wait();
        _binary.Wait();

        _buffer.PutOverwriting(producedElement);

        _binary.Release();
        
        _full.Release();
    }

    /// <summary>
    /// Прочитать из буфера
    /// </summary>
    /// <param name="extractionPredicate">Проверка элемента на подходящесть</param>
    /// <returns>Кортеж где 1 элемент - элемент из буфера, 2 - подходит ли он под предикат</returns>
    public (T?, bool) Consume(Predicate<T> extractionPredicate)
    {
        _full.Wait();
        _binary.Wait();

        var getRes = _buffer.GetElement();
        _binary.Release();
        
        var checkPred = extractionPredicate.Invoke(getRes);
        if (checkPred)
            _empty.Release();
        else
        {
            _buffer.RestoreReadSequence();
            _full.Release();
        }
        
        return (getRes, checkPred);
    }
}