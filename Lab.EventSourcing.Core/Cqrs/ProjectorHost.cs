using System;
using System.Collections.Concurrent;

namespace Lab.EventSourcing.Core
{
    public class ProjectorHost : IProjectorHost
    {
        private readonly ConcurrentDictionary<Type, dynamic> _projectors =
                new ConcurrentDictionary<Type, dynamic>();

        public void Add<TModel>(IProjector<TModel> projector)
            where TModel : EventSourcingModel<TModel>
        {
            if (_projectors.ContainsKey(typeof(TModel)))
                throw new ArgumentException($"Projector for {projector.GetType()} already registered.", nameof(projector));

            _projectors.TryAdd(typeof(TModel), projector);
        }

        public void InvokeProjector<TModel>(TModel model) 
            where TModel : EventSourcingModel<TModel>
        {
            if (model is null)
                throw new ArgumentException("A domain model must be provided.");

            if (!_projectors.ContainsKey(typeof(TModel)))
                throw new InvalidOperationException($"There is no projector available for {typeof(TModel)}.");

            ((IProjector<TModel>)_projectors[typeof(TModel)]).Execute(model);
        }
    }
}
