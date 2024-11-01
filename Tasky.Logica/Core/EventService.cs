using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasky.Logica.Core;
public interface IEventService<TEventArgs> : IObservable<TEventArgs> where TEventArgs : EventArgs
{
    void Publish(TEventArgs eventArgs);
}
public class EventService<TEventArgs> : IEventService<TEventArgs> where TEventArgs : EventArgs
{
    private readonly List<IObserver<TEventArgs>> _observers = new();

    public IDisposable Subscribe(IObserver<TEventArgs> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }

    public void Publish(TEventArgs eventArgs)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(eventArgs);
        }
    }

    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<TEventArgs>> _observers;
        private readonly IObserver<TEventArgs> _observer;

        public Unsubscriber(List<IObserver<TEventArgs>> observers, IObserver<TEventArgs> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
