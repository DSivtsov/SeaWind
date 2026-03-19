namespace Application.Common;

public class RetryRequest<T>
{
    private int[] _delays = [20, 50];
    private readonly Func<Task<T?>> _request;
    private string _textError;

    public RetryRequest(Func<Task<T?>> request, string textError)
    {
        _request = request;
        _textError = textError;
    }

    public async Task<T> Retry()
    {
        int count = 0;
        do
        {
            var rez = await _request();
            if (rez is not null)
            {
                return rez;
            }
            await Task.Delay(_delays[count++]);
        }
        while (count < _delays.Length);

        throw new InvalidOperationException(_textError);
    }
}
