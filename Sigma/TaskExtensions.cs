using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sigma
{
    public static class TaskExtensions
    {
        public static async Task<TEventArgs> EventToTask<TEventArgs, THandler>(
            Converter<EventHandler<TEventArgs>, THandler> convert,
            Action<THandler> subscribe,
            Action<THandler> unsubscribe)
        {
            return await EventToTask(() => { }, convert, subscribe, unsubscribe, CancellationToken.None);
        }

        public static async Task<TEventArgs> EventToTask<TEventArgs, THandler>(
            Action initialize,
            Converter<EventHandler<TEventArgs>, THandler> convert,
            Action<THandler> subscribe,
            Action<THandler> unsubscribe)
        {
            return await EventToTask(initialize, convert, subscribe, unsubscribe, CancellationToken.None);
        }

        public static async Task<TEventArgs> EventToTask<TEventArgs, THandler>(
            Action initialize,
            Converter<EventHandler<TEventArgs>, THandler> convert,
            Action<THandler> subscribe,
            Action<THandler> unsubscribe,
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<TEventArgs>();
            var handler = new EventHandler<TEventArgs>((s, e) => tcs.TrySetResult(e));
            var handlerConverted = convert(handler);
            var task = new EAPTask<TEventArgs, THandler>(tcs, handlerConverted);
            initialize();
            return await task.Start(subscribe, unsubscribe, cancellationToken);
        }
    }

    internal class EAPTask<TEventArgs, TEventHandler>
    {
        internal EAPTask(
            TaskCompletionSource<TEventArgs> completionSource,
            TEventHandler eventHandler)
        {
            this.completionSource = completionSource;
            this.eventHandler = eventHandler;
        }

        internal EAPTask<TEventArgs, TOtherEventHandler> WithHandlerConversion<TOtherEventHandler>(
            Converter<TEventHandler, TOtherEventHandler> converter)
            where TOtherEventHandler : class
        {
            return new EAPTask<TEventArgs, TOtherEventHandler>(completionSource, converter(eventHandler));
        }

        internal async Task<TEventArgs> Start(
            Action<TEventHandler> subscribe,
            Action<TEventHandler> unsubscribe,
            CancellationToken cancellationToken)
        {
            subscribe(eventHandler);
            try
            {
                using (cancellationToken.Register(() => completionSource.SetCanceled()))
                {
                    return await completionSource.Task;
                }
            }
            finally
            {
                unsubscribe(eventHandler);
            }
        }

        private readonly TaskCompletionSource<TEventArgs> completionSource;
        private readonly TEventHandler eventHandler;
    }
}
