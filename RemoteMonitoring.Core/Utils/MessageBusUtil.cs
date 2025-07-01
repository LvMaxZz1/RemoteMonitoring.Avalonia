using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using RemoteMonitoring.Core.Base;

namespace RemoteMonitoring.Core.Utils;

public static class MessageBusUtil
{
    /// <summary>
    ///  Sends a single message using the specified Type and contract.
    ///  Consider using RegisterMessageSource instead if you will be sending messages in response to other changes such as property changes or events. 
    /// </summary>
    /// <param name="message"> The actual message to send. </param>
    /// <param name="contract">  A unique string to distinguish messages with identical types (i.e. "MyCoolViewModel") - if the message type is only used for one purpose, leave this as null. </param>
    /// <typeparam name="T"> T — The type of the message to send. </typeparam>
    public static void SendMessage<T>(T message, string contract) where T : IMessageBusModel
    {
        MessageBus.Current.SendMessage(message, contract);
    }

    /// <summary>
    ///  Listen provides an Observable that will fire whenever a Message is provided for this object via RegisterMessageSource or SendMessage. 
    /// </summary>
    /// <param name="scheduler"> Scheduler to notify observers on.  IScheduler scheduler = RxApp.MainThreadScheduler</param>
    /// <param name="onNext">  Action to invoke for each element in the observable sequence. </param>
    /// <param name="contract"></param>
    /// <typeparam name="T"></typeparam>
    public static void ListenMessage<T>(IScheduler scheduler, Action<T> onNext, string contract)  where T : IMessageBusModel
    {
        MessageBus.Current.Listen<T>(contract).ObserveOn(scheduler).Subscribe(onNext);
    }
}