namespace ParallelProgramming.Lab1.App;

public class RingBuffer<T> 
{
    /// <summary>
    /// Последовательность записи
    /// </summary>
    private int _writeSequence;

    /// <summary>
    /// Последовательность чтения
    /// </summary>
    private int _readSequence;
    
    /// <summary>
    /// Данные в буфере
    /// </summary>
    private readonly T[] _data;

    /// <summary>
    /// Вместимость буфера
    /// </summary>
    private int _capacity;

    public RingBuffer(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("Вместимость не может быть меньше или равна нулю");
        
        _capacity = capacity;
        _writeSequence = -1;
        _readSequence = 0;
        _data = new T[capacity];
    }

    /// <summary>
    /// Заполнен ли буфер
    /// </summary>
    private bool IsFull() => (_writeSequence - _readSequence + 1) == _capacity;

    /// <summary>
    /// Пуст ли буфер
    /// </summary>
    private bool IsEmpty() => _writeSequence < _readSequence;

    /// <summary>
    /// Записать с перезаписью если индекс занят
    /// </summary>
    /// <param name="element">Элемент для вставки</param>
    /// <returns>true если запись удалась, false если нет</returns>
    public bool PutOverwriting(T element)
    {
        if (IsFull())
            return false;
        
        _data[++_writeSequence % _capacity] = element;
        return true;
    }

    /// <summary>
    /// Получить элемент из буфера
    /// </summary>
    public T? GetElement()
    {
        var element = IsEmpty() ? default : _data[_readSequence % _capacity];
        UpdateReadSequence();
        return element;
    }

    /// <summary>
    /// Восстановить счетчик последовательности чтения
    /// </summary>
    public void RestoreReadSequence() => _readSequence--;
    
    /// <summary>
    /// Обновить счетчик последовательности чтения
    /// </summary>
    public void UpdateReadSequence() => _readSequence++;

    /// <summary>
    /// Считать элемент без изменения буфера
    /// </summary>
    /// <returns>Кортеж 1 элемент - сам элемент, 2 элемент - флаг успешности извлечения</returns>
    public (T?, bool) GetElementNoChange()
    {
        var isEmpty = IsEmpty();
        return (isEmpty ? default : _data[_readSequence % _capacity], !isEmpty);
    }
}